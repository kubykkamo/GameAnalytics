namespace GameAnalytics.Infrastructure.Exceptions
{
    public class ExternalApiLimitException : Exception
    {
        public ExternalApiLimitException(string message ) : base( message ) 
        {
        }
    }
}