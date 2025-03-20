using Events;
using MassTransit;

namespace MassTransitOutboxTest.Worker.Consumers
{
    public class SendOrderToProviderWhenOrderCreated : IConsumer<OrderCreated>
    {
        private readonly ILogger<SendOrderToProviderWhenOrderCreated> _logger;

        public SendOrderToProviderWhenOrderCreated(ILogger<SendOrderToProviderWhenOrderCreated> logger)
        {
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<OrderCreated> context)
        {

            _logger.LogInformation("Order consumed. OrderId: {OrderId}", context.Message.OrderId);

            await Task.CompletedTask;
        }
    }
}
