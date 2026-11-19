using System.Globalization;
using System.Net.Sockets;
using MeterHub.Core;

namespace MeterHub.Collector;

public class Worker(ILogger<Worker> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var timeout = CancellationTokenSource.CreateLinkedTokenSource(stoppingToken); // linked token
                timeout.CancelAfter(TimeSpan.FromSeconds(3)); // cancel after 3s if no response

                using var client = new TcpClient();
                await client.ConnectAsync("127.0.0.1", 5000, timeout.Token); // connect to the simulator

                // text packet
                // using var reader = new StreamReader(client.GetStream());
                // string text = await reader.ReadToEndAsync(timeout.Token);
                // string[] parts = text.Split(';');
                // var reading = Reading(parts);

                // binary packet
                var stream = client.GetStream();
                byte[] buffer = new byte[PacketParser.PacketLength];
                await stream.ReadExactlyAsync(buffer, timeout.Token); // read exactly 23 bytes

                logger.LogDebug("Received: {Hex}", Convert.ToHexString(buffer));
                var reading = PacketParser.Parse(buffer);

                logger.LogInformation("Prebrano: {Serial} = {Value} kWh ob {Time}",
                    reading.MeterSerial, reading.Value, reading.Timestamp);
            }
            catch (OperationCanceledException) when (!stoppingToken.IsCancellationRequested)
            {
                logger.LogWarning("Meter did not respond within 3 seconds");
            }
            catch (SocketException ex)
            {
                logger.LogError("Connection failed: {Message}", ex.Message);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unexpected error while reading meter");
            }

            await Task.Delay(5000, stoppingToken);
        }
    }

    // OLD: parse text reading – no longer used
    // private static Reading Reading(string[] parts)
    // {
    //     return new Reading
    //     {
    //         MeterSerial = parts[0],
    //         ObisCode = parts[1],
    //         Timestamp = DateTime.Parse(parts[2]),
    //         Value = double.Parse(parts[3], CultureInfo.InvariantCulture)
    //     };
    // }
}