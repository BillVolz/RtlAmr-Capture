namespace RtlAmrCapture.Data;

public class RtlAmrData
{
    public DateTimeOffset Time { get; set; }
    public int Offset { get; set; }
    public int Length { get; set; }
    public string Type { get; set; }
    public RtlAmrMessage Message { get; set; }
}