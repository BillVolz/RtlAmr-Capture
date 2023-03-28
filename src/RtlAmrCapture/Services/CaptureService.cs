using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RtlAmrCapture.Data;
using RtlAmrCapture.Sql;

namespace RtlAmrCapture.Services
{
    public class CaptureService
    {

        private readonly MsSqlDataRepo _dataRepo;

        public CaptureService(MsSqlDataRepo dataRepo)
        {
            _dataRepo = dataRepo;
        }

        public async Task<bool> CapturePacket(RtlAmrData o, CancellationToken cancellationToken)
        {
            await _dataRepo.InsertRecord(o,cancellationToken);
            return true;
        }

        public async Task Initialize(CancellationToken cancellationToken)
        {
            await _dataRepo.TryCreateTable(cancellationToken);
        }
    }
}
