using System;
namespace BadmintonLt.Integration.Players.Crawler.Providers.Exceptions
{
    public class ParsingSourceFormatException: Exception
    {
        public string SourceUrl { get; }

        public string ProviderName { get; }

        public ParsingSourceFormatException(
            string sourceUrl, 
            string providerName, 
            string message, 
            Exception innerException = null)
            : base(message, innerException)
        {
            SourceUrl = sourceUrl;
            ProviderName = providerName;
        }
    }
}
