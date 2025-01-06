using Crawler.Presentation.Core;
using Crawler.Presentation.Interfaces;

namespace Crawler.Presentation;

internal class StartUp
{
    static void Main(string[] args)
    {
        IAppRunner app = new AppRunner();

        app.Run();
    }
}

