using System.Reflection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using RoadmapGenerator.API.Endpoints;

namespace RoadmapGenerator.API.Extensions;

public static class EndpointRegistration
{
    public static IServiceCollection AddEndpoints(this IServiceCollection services, Assembly assembly)
    {
        var endpointTypes = assembly
            .DefinedTypes
            .Where(type => type is { IsAbstract: false, IsInterface: false } &&
                           typeof(IEndpoint).IsAssignableFrom(type));

        foreach (var type in endpointTypes)
        {
            services.TryAddEnumerable(ServiceDescriptor.Transient(typeof(IEndpoint), type));
        }

        return services;
    }

    public static IApplicationBuilder MapEndpoints(this WebApplication app, RouteGroupBuilder? group = null)
    {
        var endpoints = app.Services.GetRequiredService<IEnumerable<IEndpoint>>();
        var builder = (IEndpointRouteBuilder?)group ?? app;

        foreach (var endpoint in endpoints)
        {
            endpoint.MapEndpoint(builder);
        }

        return app;
    }
}