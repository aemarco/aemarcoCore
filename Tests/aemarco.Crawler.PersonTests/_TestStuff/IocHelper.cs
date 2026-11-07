using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace aemarco.Crawler.PersonTests._TestStuff;

internal static class IocHelper
{

    public static T Resolve<T>(Action<IServiceCollection>? beforeResolve = null)
        where T : notnull
    {
        var sp = GetTestContainer(beforeResolve);
        var result = sp.GetRequiredService<T>();
        return result;
    }
    public static T ResolveKeyed<T>(object? key, Action<IServiceCollection>? beforeResolve = null)
        where T : notnull
    {
        var sp = GetTestContainer(beforeResolve);
        var result = sp.GetRequiredKeyedService<T>(key);
        return result;
    }

    //one provider for all plain resolves, so tests running in parallel share one rate limiting (one pace per domain)
    private static readonly IServiceProvider SharedContainer = BuildContainer(null);
    private static IServiceProvider GetTestContainer(Action<IServiceCollection>? beforeResolve) =>
        beforeResolve is null
            ? SharedContainer
            : BuildContainer(beforeResolve);

    private static IServiceProvider BuildContainer(Action<IServiceCollection>? beforeResolve)
    {
        var sc = new ServiceCollection()
            //tests register the real thing
            .AddPersonCrawler();

        //tests always use the NullLogger
        sc.AddSingleton(typeof(ILogger<>), typeof(NullLogger<>));

        beforeResolve?.Invoke(sc);

        var result = sc.BuildServiceProvider();
        return result;
    }

}
