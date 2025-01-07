namespace StargateAPI.Business.Data
{
    public class Logging
    {
        public int LogId { get; set; }

        public int ModifiedBy { get; set; }

        public string Action { get; set; } = string.Empty;
    }
}
