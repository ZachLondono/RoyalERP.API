using FluentAssertions;
using MediatR;
using Moq;
using RoyalERP.API.Common.Domain;
using System;
using System.Threading.Tasks;
using Xunit;

namespace RoyalERP.API.Tests.Unit.Common;

public class EntityTests {

    private class TestEntity : Entity {
        public TestEntity(Guid id) : base(id) { }
        public void DoStuff() => AddEvent(new TestEvent(Guid.NewGuid()));
    }

    public record TestEvent(Guid AggregateId) : DomainEvent(AggregateId);

    [Fact]
    public void AddEvent_Should_AddEventToCollection() {

        // Arrange
        var entity = new TestEntity(Guid.NewGuid());
        var evnt = new TestEvent(Guid.NewGuid());

        // Act
        entity.DoStuff();

        // Assert
        entity.Events.Should().HaveCount(1);
        entity.Events.Should().ContainSingle(x => x is TestEvent);

    }

    [Fact]
    public void PublishEvents_Should_PublishAllEventes() {

        // Arrange
        var entity = new TestEntity(Guid.NewGuid());
        var evnt = new TestEvent(Guid.NewGuid());
        var mock = new Mock<IPublisher>();
        mock.Setup(x => x.Publish(evnt, default)).Returns(Task.CompletedTask);
        var publisher = mock.Object;

        // Act
        entity.DoStuff();
        entity.DoStuff();
        entity.PublishEvents(publisher).Wait();

        // Assert
        entity.Events.Should().HaveCount(2);
        entity.Events.Should().AllSatisfy(x => x.IsPublished.Should().BeTrue());

    }

}
