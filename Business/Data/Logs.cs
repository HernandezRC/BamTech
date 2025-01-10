namespace StargateAPI.Business.Data
{
    public class Logs
    {
        public int Id {  get; set; }
        public DateTime LogDateTime { get; set; }
        public string LogLevel { get; set; } = string.Empty;
        public string LogMessage { get; set; } = string.Empty;
        public string LogException { get; set; } = string.Empty;
    }
}
