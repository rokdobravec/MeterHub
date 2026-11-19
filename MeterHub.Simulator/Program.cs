using MeterHub.Core;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Buffers.Binary;


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

    // sending plain text
    // double value = Random.Shared.NextDouble() * 10;          // random value 0–10 kWh
    // string text = $"ISK-001;1-0:1.8.0;{DateTime.Now:s};{value:F2}";
    // byte[] bytes = Encoding.UTF8.GetBytes(text);
    // await Task.Delay(Random.Shared.Next(0, 5000));            // simulate a slow meter
    // await stream.WriteAsync(bytes);
    // Console.WriteLine($"Sent: {text}");

    // random value in Wh (whole number, 0–9999) than listener will divide by 1000 to get kWh
    uint valueWh = (uint)Random.Shared.Next(0, 10000);

    // empty 23-byte packet
    byte[] packet = new byte[23];

    //start byte
    packet[0] = 0x7E;

    // bytes 1–4: meter ID (1 = ISK-001)
    BinaryPrimitives.WriteUInt32BigEndian(packet.AsSpan(1), 1);

    // bytes 5–10: OBIS code 1-0:1.8.0.255
    packet[5] = 1;
    packet[6] = 0;
    packet[7] = 1;
    packet[8] = 8;
    packet[9] = 0;
    packet[10] = 255;

    // bytes 11–18: timestamp (seconds since 1 Jan 1970)
    long unixTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
    BinaryPrimitives.WriteInt64BigEndian(packet.AsSpan(11), unixTime);

    // bytes 19–22: value in Wh
    BinaryPrimitives.WriteUInt32BigEndian(packet.AsSpan(19), valueWh);

    await Task.Delay(Random.Shared.Next(0, 5000));   // simulate a slow meter
    await stream.WriteAsync(packet);                 // send the packet
    Console.WriteLine($"Sent: {Convert.ToHexString(packet)}  ({valueWh} Wh)");
}


// OLD - Method to create a reading for a specific meter
// static Reading CreateReading(string meterSerial, double value)
// {
//     return new Reading
//     {
//         MeterSerial = meterSerial,
//         ObisCode = "1-0:1.8.0",
//         Timestamp = DateTime.Now,
//         Value = value
//     };
// }


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
