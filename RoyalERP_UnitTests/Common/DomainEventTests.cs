using FluentAssertions;
using MediatR;
using Moq;
using RoyalERP.Common.Domain;
using System;
using System.Threading.Tasks;
using Xunit;

namespace RoyalERP_UnitTests.Common;

public class DomainEventTests {

    public record TestEvent(Guid AggregateId) : DomainEvent(AggregateId);

    [Fact]
    public void AddEvent_Should_AddEventToCollection() {

        // Arrange
        var evnt = new TestEvent(Guid.NewGuid());

        var mock = new Mock<IPublisher>();
        mock.Setup(x => x.Publish(evnt, default)).Returns(Task.CompletedTask);
        var publisher = mock.Object;

        // Act
        evnt.Publish(publisher).Wait();

        // Assert
        evnt.IsPublished.Should().BeTrue();

    }

}
