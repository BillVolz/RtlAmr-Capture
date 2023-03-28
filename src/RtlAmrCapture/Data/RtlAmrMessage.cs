namespace RtlAmrCapture.Data;

public class RtlAmrMessage
{
    public int FrameSync { get; set; }
    public int ProtocolID { get; set; }
    public int EndpointType { get; set; }
    public int EndpointID { get; set; }
    public int Consumption { get; set; }
    public int Tamper { get; set; }
    public int PacketCRC { get; set; }
}