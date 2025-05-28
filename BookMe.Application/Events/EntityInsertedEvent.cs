using BookMe.Application.Entities;

namespace BookMe.Application.Events;

public class EntityInsertedEvent<TEntity> : IEvent where TEntity : BaseEntity
{
    public TEntity Entity { get; }

    public EntityInsertedEvent(TEntity entity)
    {
        Entity = entity;
    }

}
