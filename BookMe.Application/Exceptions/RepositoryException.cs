using System;
using System.Runtime.Serialization;

namespace BookMe.Application.Exceptions;

/// <summary>
/// Represents errors that occur during repository operations
/// </summary>
[Serializable]
public class RepositoryException : Exception
{
    /// <summary>
    /// Gets or sets the name of the entity type that caused the exception
    /// </summary>
    public string EntityTypeName { get; }

    /// <summary>
    /// Gets or sets the operation that caused the exception
    /// </summary>
    public string Operation { get; }

    /// <summary>
    /// Initializes a new instance of the RepositoryException class
    /// </summary>
    public RepositoryException()
        : base("An error occurred during the repository operation.")
    {
    }

    /// <summary>
    /// Initializes a new instance of the RepositoryException class with a specified error message
    /// </summary>
    /// <param name="message">The message that describes the error</param>
    public RepositoryException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the RepositoryException class with a specified error message
    /// and a reference to the inner exception that is the cause of this exception
    /// </summary>
    /// <param name="message">The message that describes the error</param>
    /// <param name="innerException">The exception that is the cause of the current exception</param>
    public RepositoryException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    /// <summary>
    /// Initializes a new instance of the RepositoryException class with detailed information
    /// </summary>
    /// <param name="message">The message that describes the error</param>
    /// <param name="entityTypeName">The name of the entity type that caused the exception</param>
    /// <param name="operation">The operation that caused the exception</param>
    /// <param name="innerException">The exception that is the cause of the current exception</param>
    public RepositoryException(string message, string entityTypeName, string operation, Exception innerException)
        : base(message, innerException)
    {
        EntityTypeName = entityTypeName;
        Operation = operation;
    }

    /// <summary>
    /// Initializes a new instance of the RepositoryException class with serialized data
    /// </summary>
    /// <param name="info">The SerializationInfo that holds the serialized object data</param>
    /// <param name="context">The StreamingContext that contains contextual information about the source or destination</param>
    protected RepositoryException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
        EntityTypeName = info.GetString(nameof(EntityTypeName));
        Operation = info.GetString(nameof(Operation));
    }

    /// <summary>
    /// Sets the SerializationInfo with information about the exception
    /// </summary>
    /// <param name="info">The SerializationInfo that holds the serialized object data</param>
    /// <param name="context">The StreamingContext that contains contextual information about the source or destination</param>
    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        base.GetObjectData(info, context);
        info.AddValue(nameof(EntityTypeName), EntityTypeName);
        info.AddValue(nameof(Operation), Operation);
    }
}
