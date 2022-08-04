using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace RoyalERP_IntegrationTests;

public sealed partial class ManufacturingTests {
    public class FakePublisher : IPublisher {
        public Task Publish(object notification, CancellationToken cancellationToken = default) {
            return Task.CompletedTask;
        }

        public Task Publish<TNotification>(TNotification notification, CancellationToken cancellationToken = default) where TNotification : INotification {
            return Task.CompletedTask;
        }
    }

}