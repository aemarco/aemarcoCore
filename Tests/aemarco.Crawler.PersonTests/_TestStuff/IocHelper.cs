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

    private static IServiceProvider GetTestContainer(Action<IServiceCollection>? beforeResolve = null)
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
