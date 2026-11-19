using System.Buffers.Binary;

namespace MeterHub.Core;

public static class PacketParser
{
    public const int PacketLength = 23;
    public const byte StartByte = 0x7E;

    public static Reading Parse(byte[] packet)
    {
        // validation
        if (packet.Length != PacketLength)
            throw new ArgumentException($"Packet must be {PacketLength} bytes, got {packet.Length}");

        // error if first byte isnt staring byte
        if(packet[0] != StartByte)
            throw new ArgumentException($"Packet must start with {StartByte:X2}, got {packet[0]:X2}"); // X2 = hex with 2 digits

        // meter ID (bytes 1–4) -> "ISK-001"
        uint meterId = BinaryPrimitives.ReadUInt32BigEndian(packet.AsSpan(1));
        string serial = $"ISK-{meterId:D3}";      // D3 = always 3 digits: 1 -> 001

        // OBIS code (bytes 5–10) -> "1-0:1.8.0"
        string obisCode = $"{packet[5]}-{packet[6]}:{packet[7]}.{packet[8]}.{packet[9]}";

        // timestamp (bytes 11–18) -> DateTime
        long unixTime = BinaryPrimitives.ReadInt64BigEndian(packet.AsSpan(11));
        DateTime timestamp = DateTimeOffset.FromUnixTimeSeconds(unixTime).LocalDateTime;

        // value (bytes 19–22) -> kWh
        uint valueWh = BinaryPrimitives.ReadUInt32BigEndian(packet.AsSpan(19));
        double valueKwh = valueWh / 1000.0;

        // build the Reading
        return new Reading
        {
            MeterSerial = serial,
            ObisCode = obisCode,
            Timestamp = timestamp,
            Value = valueKwh
        };
    }
}