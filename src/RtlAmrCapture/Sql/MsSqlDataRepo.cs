using Microsoft.Extensions.Options;
using RtlAmrCapture.Config;
using RtlAmrCapture.Data;
using System.Data.SqlClient;

namespace RtlAmrCapture.Sql
{
    public class MsSqlDataRepo
    {
        private readonly ServiceConfiguration _serviceConfiguration;
        private readonly IConfiguration _configuration;
        private readonly ILogger<MsSqlDataRepo> _logger;

        public MsSqlDataRepo(IOptions<ServiceConfiguration> serviceConfiguration, IConfiguration configuration,
            ILogger<MsSqlDataRepo> logger)
        {
            _configuration = configuration;
            _logger = logger;
            _serviceConfiguration = serviceConfiguration.Value;
        }

        public async Task TryCreateTable(CancellationToken cancellationToken)
        {
            if (_serviceConfiguration.Connections == null)
            {
                _logger.LogError("No connection string defined.");
                return;
            }

            foreach (var conn in _serviceConfiguration.Connections)
            {
                if (string.IsNullOrEmpty(conn.ConnectionStringName))
                {
                    _logger.LogError("Null or empty connection string name found.");
                    continue;
                }

                var connectionString = _configuration.GetConnectionString(conn.ConnectionStringName);
                if (connectionString == null)
                {
                    _logger.LogError("Connection string was not found {connectionStringName}",
                        conn.ConnectionStringName);
                    continue;
                }

                await using var c = new SqlConnection(connectionString);
                await c.OpenAsync(cancellationToken);
                await using var cmd = c.CreateCommand();
                cmd.CommandText = CreateTableSql;
                await cmd.ExecuteNonQueryAsync(cancellationToken);
            }
        }

        public async Task InsertRecord(RtlAmrData rad, CancellationToken cancellationToken)
        {
            if (_serviceConfiguration.Connections == null)
            {
                _logger.LogError("No connection string defined.");
                return;
            }

            foreach (var conn in _serviceConfiguration.Connections)
            {
                if (string.IsNullOrEmpty(conn.ConnectionStringName))
                {
                    _logger.LogError("Null or empty connection string name found.");
                    continue;
                }

                var connectionString = _configuration.GetConnectionString(conn.ConnectionStringName);
                if (connectionString == null)
                {
                    _logger.LogError("Connection string was not found {connectionStringName}",
                        conn.ConnectionStringName);
                    continue;
                }

                await using var c = new SqlConnection(connectionString);
                await c.OpenAsync(cancellationToken);
                await using var cmd = c.CreateCommand();
                cmd.CommandText = InsertSql;
                cmd.Parameters.AddRange(GetParameters(rad, cmd).ToArray());
                await cmd.ExecuteNonQueryAsync(cancellationToken);
            }

        }

        private List<SqlParameter> GetParameters(RtlAmrData rad, SqlCommand command)
        {
            var pars = new List<SqlParameter>();

            pars.Add(new SqlParameter("stamp", rad.Time));
            pars.Add(new SqlParameter("Type", rad.Type));
            pars.Add(new SqlParameter("ProtocolId", rad.Message.ProtocolID));
            pars.Add(new SqlParameter("EndpointType", rad.Message.EndpointType));
            pars.Add(new SqlParameter("EndpointId", rad.Message.EndpointID));
            pars.Add(new SqlParameter("Consumption", rad.Message.Consumption));

            return pars;
        }

        private const string InsertSql =
            @"Insert into RtlamrRaw([Timestamp],Type,ProtocolId,EndpointType,EndpointId,Consumption) 
                VALUES(@stamp,@Type,@ProtocolId,@EndpointType,@EndpointId,@Consumption)";

        private const string CreateTableSql = @"IF OBJECT_ID(N'[dbo].[RtlamrRaw]', N'U') IS NULL
                        Begin
	                        CREATE TABLE [dbo].[RtlamrRaw](
		                    [RtlamrRawId] [bigint] IDENTITY(1,1) NOT NULL,
		                    [Timestamp] [DateTimeOffset](7) NOT NULL,
		                    [Type] [nvarchar](255) NOT NULL,
		                    ProtocolId [int] NOT NULL,
		                    EndpointType [int] NOT NULL,
		                    EndpointId [int] NOT NULL,
		                    Consumption [bigint] NOT NULL);
            
			            CREATE CLUSTERED INDEX [ClusteredIndex-EndpointId-Timestamp] ON [dbo].[RtlamrRaw]
                        (
	                        [Timestamp] ASC,
	                        [EndpointId] ASC
                        )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
                    End; ";
    }
}