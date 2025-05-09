using System;
using BookMe.Application.Entities;

namespace BookMe.Application.Extensions;

public static class AuditableExtensions
{
    public static void SetAuditableProperties(this IAuditable auditable, Guid userId, AuditEventType auditEventType)
    {
        if (auditEventType == AuditEventType.Created)
        {
            auditable.CreatedAt = DateTime.Now;
            auditable.CreatedBy = userId;
            return;
        }


        auditable.UpdatedAt = DateTime.Now;
        auditable.UpdatedBy = userId;
    }
}

public enum AuditEventType { Created, Modified }
