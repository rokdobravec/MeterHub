using System;
using System.Collections.Generic;
using System.Text;

namespace MeterHub.Core
{
    public class Reading
    {
        required public string MeterSerial { get; set; }
        required public string ObisCode { get; set; }
        public DateTime Timestamp { get; set; }
        public double Value { get; set; }
    }
}
