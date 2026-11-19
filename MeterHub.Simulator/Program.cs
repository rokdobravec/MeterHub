using MeterHub.Core;
using System.Net;
using System.Net.Sockets;
using System.Text;


var meter1 = new MeterHub.Core.Meter("ISK-001");
var meter2 = new MeterHub.Core.Meter("ISK-002");

// var Reading1 = new Reading
// {
//     MeterSerial = "ISK-001",
//     ObisCode = "1-0:1.8.0",
//     Timestamp = DateTime.Now,
//     Value = 7
// };
// var Reading2 = new Reading
// {
//     MeterSerial = "ISK-001",
//     ObisCode = "1-0:1.8.0",
//     Timestamp = DateTime.Now,
//     Value = 2.5
// };
// var Reading3 = new Reading
// {
//     MeterSerial = "ISK-001",
//     ObisCode = "1-0:1.8.0",
//     Timestamp = DateTime.Now,
//     Value = 0.5
// };
// var badReading = new Reading
// {
//     MeterSerial = "ISK-001",
//     ObisCode = "1-0:1.8.0",
//     Timestamp = DateTime.Now,
//     Value = -5
// };
// 
// 
// var Reading21 = new Reading
// {
//     MeterSerial = "ISK-002",
//     ObisCode = "1-0:1.8.0",
//     Timestamp = DateTime.Now,
//     Value = 7.5
// };
// var Reading22 = new Reading
// {
//     MeterSerial = "ISK-002",
//     ObisCode = "1-0:1.8.0",
//     Timestamp = DateTime.Now,
//     Value = 2.5
// };
// var Reading23 = new Reading
// {
//     MeterSerial = "ISK-002",
//     ObisCode = "1-0:1.8.0",
//     Timestamp = DateTime.Now,
//     Value = 10
// };
// var badReading1 = new Reading
// {
//     MeterSerial = "ISK-002",
//     ObisCode = "1-0:1.8.0",
//     Timestamp = DateTime.Now,
//     Value = -15
// };

// try
// {
//     double[] values1 = { 7, 2.5, 0.5 };
//     foreach (var v in values1)
//     {
//         meter1.AddReading(CreateReading("ISK-001", v));
//     }
// 
//     double[] values2 = { 7.5, 2.5, 10 };
//     foreach (var v in values2)
//     {
//         meter2.AddReading(CreateReading("ISK-002", v));
//     }
// 
//     Console.WriteLine($"{meter1.SerialNumber}: {meter1.TotalConsumption} kWh");
//     Console.WriteLine($"{meter2.SerialNumber}: {meter2.TotalConsumption} kWh");
// }
// catch (ArgumentException ex)
// {
//     Console.WriteLine($"Error adding reading: {ex.Message}");
// }


// TCP strežnik
var listener = new TcpListener(IPAddress.Loopback, 5000);
listener.Start();
Console.WriteLine("Posluša na vratih 5000...");

while (true)
{
    using TcpClient client = await listener.AcceptTcpClientAsync();
    NetworkStream stream = client.GetStream();

    double value = Random.Shared.NextDouble() * 10; // random 0-10 number
    string text = $"ISK-001;1-0:1.8.0;{DateTime.Now:s};{value:f2}";

    byte[] bytes = Encoding.UTF8.GetBytes(text);

    await Task.Delay(Random.Shared.Next(0, 5000)); // send after random delay
    await stream.WriteAsync(bytes, 0, bytes.Length);  // send the reading to the client
    Console.WriteLine($"Poslano: {text}");
}


// Method to create a reading for a specific meter
static Reading CreateReading(string meterSerial, double value)
{
    return new Reading
    {
        MeterSerial = meterSerial,
        ObisCode = "1-0:1.8.0",
        Timestamp = DateTime.Now,
        Value = value
    };
}


// static async Task<double> ReadMeterAsync(string serial, int delayMs)
// {
//     Console.WriteLine($"Berem {serial}...");
//     await Task.Delay(delayMs);
//     Console.WriteLine($"{serial} odgovoril");
//     return 5.0;
// }
// 
// 
// var a = await ReadMeterAsync("ISK-001", 1000);
// var b = await ReadMeterAsync("ISK-001", 1500);
// var c = await ReadMeterAsync("ISK-001", 2000);
// 
// // var sw = System.Diagnostics.Stopwatch.StartNew();
// var t1 = ReadMeterAsync("ISK-001", 1000);
// var t2 = ReadMeterAsync("ISK-001", 1500);
// var t3 = ReadMeterAsync("ISK-001", 2000);
// 
// var results = await Task.WhenAll(t1, t2, t3);  // waits for all tasks to be done
// 
// // Console.WriteLine($"Trajalo: {sw.ElapsedMilliseconds} ms");
