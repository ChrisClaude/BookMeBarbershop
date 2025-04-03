using System;
using BookMe.Application.Entities;

namespace BookMe.Application.Events;

public class EntityUpdatedEvent<TEntity> : IEvent where TEntity : BaseEntity
{
    public TEntity Entity { get; }

    public EntityUpdatedEvent(TEntity entity)
    {
        Entity = entity;
    }
}
