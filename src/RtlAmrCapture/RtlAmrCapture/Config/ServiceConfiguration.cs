﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RtlAmrCapture.Config
{
    public class ServiceConfiguration
    {
        public string? FullPathToRtlAmr { get; set; }
        public string? RtlAmrArguments { get; set; }

        public DataBaseConnections[]? Connections { get; set; }

    }

    public class DataBaseConnections
    {
        public string? ConnectionStringName { get; set; }
        public string? Type { get; set; }
    }
}
