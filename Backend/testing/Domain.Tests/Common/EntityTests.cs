using Domain.Common;

namespace Domain.Tests.Common;

public class EntityTests
{
    [Fact]
    public void CanAddDomainEvents()
    {
        // ************ ARRANGE ************

        TestDouble sut = new TestDouble();

        DomainEvent ev = new DomainEvent();

        // ************ ACT ************

        sut.Call_AddDomainEvent(ev);
        IEnumerable<IDomainEvent> result = sut.DomainEvents;
        
        // ************ ASSERT ************

        Assert.Single(result);
        Assert.Contains(result, e =>
            ReferenceEquals(e, ev));
    }

    class DomainEvent : IDomainEvent
    {
        
    }

    class TestDouble : Entity
    {
        public void Call_AddDomainEvent(IDomainEvent domainEvent)
        {
            base.AddDomainEvent(domainEvent);
        }
    }
}