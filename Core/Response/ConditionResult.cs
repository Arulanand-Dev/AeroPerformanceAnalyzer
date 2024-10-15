namespace AeroMetrics.Core.Response
{
    public class ConditionResult
    {
        public double Time { get; set; }
        public int Channel { get; set; }
        public string Condition { get; set; } = string.Empty;
    }

}
