using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Common.Extensions;

public static class Extensions
{
    public static TSettings ConfigureSettings<TSettings>(this IHostApplicationBuilder builder) where TSettings : class
    {
        var section = builder.Configuration.GetSection(typeof(TSettings).Name);
        builder.Services.Configure<TSettings>(section);
        return section.Get<TSettings>()!;
    }
}