using BookMe.Application.Entities;

namespace BookMe.Application.Events;

public class EntityDeletedEvent<TEntity> : IEvent where TEntity : BaseEntity
{
    public TEntity Entity { get; }

    public EntityDeletedEvent(TEntity entity)
    {
        Entity = entity;
    }

}
