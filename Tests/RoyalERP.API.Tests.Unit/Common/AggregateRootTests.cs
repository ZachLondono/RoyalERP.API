using FluentAssertions;
using MediatR;
using Moq;
using RoyalERP.Common.Domain;
using System;
using System.Threading.Tasks;
using Xunit;

namespace RoyalERP_UnitTests.Common;

public class AggregateRootTests {

    private class TestEntity : AggregateRoot {
        public TestEntity(Guid id, int version) : base(id, version) { }
        public void DoStuff() => AddEvent(new TestEvent(Guid.NewGuid()));
    }

    public record TestEvent(Guid AggregateId) : DomainEvent(AggregateId);

    [Fact]
    public void PublishEvents_Should_UpdateVersion() {

        // Arrange
        int initialVersion = 0;
        var entity = new TestEntity(Guid.NewGuid(), initialVersion);

        var evnt = new TestEvent(Guid.NewGuid());
        var mock = new Mock<IPublisher>();
        mock.Setup(x => x.Publish(evnt, default)).Returns(Task.CompletedTask);
        var publisher = mock.Object;

        // Act
        entity.DoStuff();
        entity.PublishEvents(publisher).Wait();

        // Assert
        entity.Version.Should().Be(initialVersion + 1);

    }

}
