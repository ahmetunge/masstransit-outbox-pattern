using MassTransit;
using MassTransitOutboxTest.Worker.Consumers;
using Microsoft.Extensions.Configuration;

var builder = Host.CreateApplicationBuilder(args);

var configuration = builder.Configuration;


builder.Services.AddMassTransit(x =>
{
    x.SetKebabCaseEndpointNameFormatter();

    x.AddConsumer<SendOrderToProviderWhenOrderCreated>();

    x.UsingRabbitMq((context, cfg) =>
    {
        var port = configuration.GetValue<ushort>("RabbitMq:Port");
        cfg.Host(configuration["RabbitMq:Host"]!, port, configuration["RabbitMq:VirtualHost"]!, hostCfg =>
        {
            hostCfg.Username(configuration["RabbitMq:Username"]!);
            hostCfg.Password(configuration["RabbitMq:Password"]!);
        });

        cfg.ReceiveEndpoint("order-created", e =>
        {
            e.ConfigureConsumer<SendOrderToProviderWhenOrderCreated>(context);
        });

        cfg.ConfigureEndpoints(context);
    });
});


var app = builder.Build();



app.Run();


/*
        services.AddMassTransit(x =>
        {
            x.AddConsumer<TestConsumer>();

            x.UsingRabbitMq((context, cfg) =>
            {
                var port = configuration.GetValue<ushort>("RabbitMq:Port");
                cfg.Host(configuration["RabbitMq:Host"]!, port, configuration["RabbitMq:VirtualHost"]!, hostCfg =>
                {
                    hostCfg.Username(configuration["RabbitMq:Username"]!);
                    hostCfg.Password(configuration["RabbitMq:Password"]!);
                });

#if DEBUG
                cfg.UseConcurrencyLimit(1); // Concurrency limit in debug mode
#endif

                cfg.PrefetchCount = 16;
                cfg.UseConcurrencyLimit(16);

                cfg.UseMessageRetry(configurator =>
                {
                    configurator.Incremental(20, TimeSpan.FromMilliseconds(50), TimeSpan.FromMilliseconds(100));
                    configurator.Ignore<ApplicationException>();
                });

                cfg.ReceiveEndpoint("test-command", ep =>
                {
                    ep.Consumer<TestConsumer>(context);
                    cfg.UseMessageRetry(configurator =>
                    {
                        configurator.Exponential(30, TimeSpan.FromMilliseconds(50), TimeSpan.FromHours(1), TimeSpan.FromSeconds(10));
                        configurator.Ignore<ApplicationException>();
                    });
                });
            });
        });
 
 */