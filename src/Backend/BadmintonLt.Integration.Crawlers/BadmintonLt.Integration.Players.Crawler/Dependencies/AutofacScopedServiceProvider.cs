using System;
using System.Collections.Generic;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using BadmintonLt.Integration.Players.Crawler.Domain.Contracts.Integration;
using BadmintonLt.Integration.Players.Crawler.Domain.Contracts.Persistence;
using BadmintonLt.Integration.Players.Crawler.Domain.Contracts.Providers;
using BadmintonLt.Integration.Players.Crawler.Integration;
using BadmintonLt.Integration.Players.Crawler.Persistence;
using BadmintonLt.Integration.Players.Crawler.Providers;
using BadmintonLt.Integration.Players.Crawler.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Watts.Shared.Functions.DependencyInjection.Contracts;

namespace BadmintonLt.Integration.Players.Crawler.Dependencies
{
    public class AutofacScopedServiceProvider: IScopedServiceProvider
    {
        public IServiceProvider CurrentFor(IConfiguration configuration)
        {
            var service = new ServiceCollection();

            service.AddScoped(s => configuration);
            service.AddScoped<IPlayersRepository>(
                c => new PlayersTableStorageRepository(
                    c.GetService<IConfiguration>().GetConnectionString("StorageConnectionString")));
            service.AddScoped<IPlayersIntegration>(
                c => new PlayersServiceBusIntegration(
                    c.GetService<IConfiguration>().GetConnectionString("MessageBusConnectionString"),
                    c.GetService<IConfiguration>()["PlayersIntegrationTopicName"]));

            service.AddScoped<IPlayersProvider, BadmintonLtPlayersProvider>();
            service.AddScoped<PlayersService>();

            var builder = new ContainerBuilder();
            builder.Populate(service);
            return new AutofacServiceProvider(builder.Build());
        }
    }
}