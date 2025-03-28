public class SpeedTestResults
{
    public string type { get; set; }
    public DateTime timestamp { get; set; }
    public Ping ping { get; set; }
    public Download download { get; set; }
    public Upload upload { get; set; }
    public double packetLoss { get; set; }
    public string isp { get; set; }
    public Interface @interface { get; set; }
    public Server server { get; set; }
    public Result result { get; set; }
}

