using MassTransit;
using MassTransitOutboxTest.OutboxJobs.Worker.Data;
using Microsoft.EntityFrameworkCore;

var builder = Host.CreateApplicationBuilder(args);

var configuration = builder.Configuration;

builder.Services.AddDbContext<OrderDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("OrdersDb"));
});

builder.Services.AddMassTransit(x =>
{
    x.SetKebabCaseEndpointNameFormatter();

    x.AddEntityFrameworkOutbox<OrderDbContext>(o =>
    {
        o.DuplicateDetectionWindow = TimeSpan.FromMicroseconds(30);
        o.QueryDelay = TimeSpan.FromMicroseconds(5);
        o.UsePostgres();
        o.DisableInboxCleanupService();
        o.UseBusOutbox();
    });

    x.UsingRabbitMq((context, cfg) =>
    {
        var port = configuration.GetValue<ushort>("RabbitMq:Port");
        cfg.Host(configuration["RabbitMq:Host"]!, port, configuration["RabbitMq:VirtualHost"]!, hostCfg =>
        {
            hostCfg.Username(configuration["RabbitMq:Username"]!);
            hostCfg.Password(configuration["RabbitMq:Password"]!);
        });
    });
});



var app = builder.Build();


app.Run();
