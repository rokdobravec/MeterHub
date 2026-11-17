using MeterHub.Core;

var Meter = new Meter("ISK-001");

var Reading1 = new Reading
{
    MeterSerial = "ISK-001",
    ObisCode = "1-0:1.8.0",
    Timestamp = DateTime.Now,
    Value = 7
};
var Reading2 = new Reading
{
    MeterSerial = "ISK-001",
    ObisCode = "1-0:1.8.0",
    Timestamp = DateTime.Now,
    Value = 2.5
};
var Reading3 = new Reading
{
    MeterSerial = "ISK-001",
    ObisCode = "1-0:1.8.0",
    Timestamp = DateTime.Now,
    Value = 0.5
};
var badReading = new Reading
{
    MeterSerial = "ISK-001",
    ObisCode = "1-0:1.8.0",
    Timestamp = DateTime.Now,
    Value = -5
};

try
{
    Meter.AddReading(Reading1);
    Meter.AddReading(Reading2);
    Meter.AddReading(Reading3);
    Meter.AddReading(badReading);
}
catch (ArgumentException ex)
{
    Console.WriteLine($"Error adding reading: {ex.Message}");
}
