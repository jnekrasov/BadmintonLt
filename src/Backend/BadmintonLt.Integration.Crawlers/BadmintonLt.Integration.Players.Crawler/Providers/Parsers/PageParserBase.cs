using AngleSharp;

namespace BadmintonLt.Integration.Players.Crawler.Providers.Parsers
{
    public abstract class PageParserBase
    {
        protected IBrowsingContext ParsingContext { get; } =
            BrowsingContext.New(Configuration.Default.WithDefaultLoader());
    }
}