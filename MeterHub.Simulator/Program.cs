using MeterHub.Core;

var meter1 = new MeterHub.Core.Meter("ISK-001");
var meter2 = new MeterHub.Core.Meter("ISK-002");

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


var Reading21 = new Reading
{
    MeterSerial = "ISK-002",
    ObisCode = "1-0:1.8.0",
    Timestamp = DateTime.Now,
    Value = 7.5
};
var Reading22 = new Reading
{
    MeterSerial = "ISK-002",
    ObisCode = "1-0:1.8.0",
    Timestamp = DateTime.Now,
    Value = 2.5
};
var Reading23 = new Reading
{
    MeterSerial = "ISK-002",
    ObisCode = "1-0:1.8.0",
    Timestamp = DateTime.Now,
    Value = 10
};
var badReading1 = new Reading
{
    MeterSerial = "ISK-002",
    ObisCode = "1-0:1.8.0",
    Timestamp = DateTime.Now,
    Value = -15
};

try
{
    meter1.AddReading(Reading1);
    meter1.AddReading(Reading2);
    meter1.AddReading(Reading3);
    // meter1.AddReading(badReading);
    meter2.AddReading(Reading21);
    meter2.AddReading(Reading22);
    meter2.AddReading(Reading23);
    // meter2.AddReading(badReading1);

    Console.WriteLine($"{meter1.SerialNumber}: {meter1.TotalConsumption} kWh");
    Console.WriteLine($"{meter2.SerialNumber}: {meter2.TotalConsumption} kWh");
}
catch (ArgumentException ex)
{
    Console.WriteLine($"Error adding reading: {ex.Message}");
}
