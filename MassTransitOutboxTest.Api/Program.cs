using Events;
using MassTransit;
using MassTransitOutboxTest.Api.Data;
using MassTransitOutboxTest.Api.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var configuration = builder.Configuration;




builder.Services.AddDbContext<OrderDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("OrdersDb"));
});


builder.Services.AddMassTransit(config =>
{
    config.SetKebabCaseEndpointNameFormatter();

    config.AddEntityFrameworkOutbox<OrderDbContext>(o =>
    {
        o.UsePostgres();
        o.UseBusOutbox(x => x.DisableDeliveryService());
    });

    config.UsingRabbitMq((context, cfg) =>
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

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();


app.MapPost("/orders", async (OrderDbContext context, IPublishEndpoint publish) =>
{
    var orderId = Guid.NewGuid();

    Order order = new Order
    {
        Id = orderId,
        CreatedAt = DateTime.UtcNow,
    };

    context.Orders.Add(order);

    await publish.Publish(new OrderCreated(orderId));

    await context.SaveChangesAsync();
});





app.Run();