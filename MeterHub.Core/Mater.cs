using System;
using System.Collections.Generic;
using System.Text;

namespace MeterHub.Core
{
    public class Meter
    {
        public string SerialNumber { get; }
        private readonly List<Reading> _readings = new();
        public double TotalConsumption => _readings.Sum(r => r.Value);

        public Meter(string serialNumber)
        {
            SerialNumber = serialNumber;
        }

        public void AddReading(Reading reading)
        {
            if (reading.Value < 0)
                throw new ArgumentException("Reading value cannot be negative");
            if (reading.MeterSerial != SerialNumber)
                throw new ArgumentException("Reading does not belong to this meter");
            _readings.Add(reading);
        }
    }
}
