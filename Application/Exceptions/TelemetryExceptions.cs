namespace AeroMetrics.Application.Exceptions
{
    public class NoTelemetryDataException : Exception
    {
        public NoTelemetryDataException(string message) : base(message) { }
        public NoTelemetryDataException(string message, Exception innerException) : base(message, innerException) { }
    }

    public class TelemetryDataProcessingException : Exception
    {
        public TelemetryDataProcessingException(string message) : base(message) { }
        public TelemetryDataProcessingException(string message, Exception innerException) : base(message, innerException) { }
    }

    public class TelemetryDataRetrievalException : Exception
    {
        public TelemetryDataRetrievalException(string message) : base(message) { }
        public TelemetryDataRetrievalException(string message, Exception innerException) : base(message, innerException) { }
    }

    public class TelemetryDataFormatException : Exception
    {
        public TelemetryDataFormatException(string message) : base(message) { }
        public TelemetryDataFormatException(string message, Exception innerException) : base(message, innerException) { }
    }

    public class InvalidConditionException : Exception
    {
        public InvalidConditionException(string message) : base(message) { }
        public InvalidConditionException(string message, Exception innerException) : base(message, innerException) { }
    }
}