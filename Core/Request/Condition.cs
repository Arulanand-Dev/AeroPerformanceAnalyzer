namespace AeroMetrics.Core.Request
{
    public class Condition
    {
        public int Channel { get; set; }
        public string Operator { get; set; }
        public double Value { get; set; }
    }
}
