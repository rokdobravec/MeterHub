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
                using var timeout = CancellationTokenSource.CreateLinkedTokenSource(stoppingToken); //linked token
                timeout.CancelAfter(TimeSpan.FromSeconds(3)); // cancel after 3s if no responce

                using var client = new TcpClient();
                await client.ConnectAsync("127.0.0.1", 5000, timeout.Token); // connect to the simulator

                using var reader = new StreamReader(client.GetStream());
                string text = await reader.ReadToEndAsync(timeout.Token); // read the reading sent by the simulator

                string[] parts = text.Split(';');
                var reading = Reading(parts);

                logger.LogInformation("Prebrano: {Serial} = {Value} kWh ob {Time}",
                    reading.MeterSerial, reading.Value, reading.Timestamp);


            }
            catch (OperationCanceledException) when (!stoppingToken.IsCancellationRequested)
            {
                logger.LogWarning("Stevec ni odgovoril na zahtevo za prekinitev. Prekinjam izvajanje.");
            }
            catch (SocketException ex)
            {
                logger.LogError("Povezava ni uspela: {Message}", ex.Message);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Nepričakovana napaka pri branju števca");
            }

            await Task.Delay(5000, stoppingToken);
        }
    }
    // Method to create a reading from the received text
    private static Reading Reading(string[] parts)
    {
        return new Reading
        {
            MeterSerial = parts[0],
            ObisCode = parts[1],
            Timestamp = (DateTime.Parse(parts[2])),
            Value = double.Parse(parts[3], CultureInfo.InvariantCulture)
        };
    }
}
