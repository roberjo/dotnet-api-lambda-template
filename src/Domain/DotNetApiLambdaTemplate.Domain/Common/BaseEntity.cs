using System.ComponentModel.DataAnnotations;

namespace DotNetApiLambdaTemplate.Domain.Common;

/// <summary>
/// Base entity class providing common properties for all domain entities
/// </summary>
/// <typeparam name="TId">The type of the entity's identifier</typeparam>
public abstract class BaseEntity<TId> where TId : notnull
{
    /// <summary>
    /// Unique identifier for the entity
    /// </summary>
    [Key]
    public TId Id { get; protected set; } = default!;

    /// <summary>
    /// Date and time when the entity was created
    /// </summary>
    public DateTime CreatedAt { get; protected set; }

    /// <summary>
    /// Date and time when the entity was last updated
    /// </summary>
    public DateTime UpdatedAt { get; protected set; }

    /// <summary>
    /// User or system that created the entity
    /// </summary>
    public string CreatedBy { get; protected set; } = string.Empty;

    /// <summary>
    /// User or system that last updated the entity
    /// </summary>
    public string UpdatedBy { get; protected set; } = string.Empty;

    /// <summary>
    /// Soft delete flag
    /// </summary>
    public bool IsDeleted { get; protected set; }

    /// <summary>
    /// Version for optimistic concurrency control
    /// </summary>
    [Timestamp]
    public byte[] RowVersion { get; protected set; } = Array.Empty<byte>();

    protected BaseEntity() { }

    protected BaseEntity(TId id)
    {
        Id = id;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Sets the creation metadata
    /// </summary>
    /// <param name="createdBy">User or system that created the entity</param>
    protected void SetCreated(string createdBy)
    {
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
        CreatedBy = createdBy;
        UpdatedBy = createdBy;
    }

    /// <summary>
    /// Sets the update metadata
    /// </summary>
    /// <param name="updatedBy">User or system that updated the entity</param>
    protected void SetUpdated(string updatedBy)
    {
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = updatedBy;
    }

    /// <summary>
    /// Marks the entity as deleted
    /// </summary>
    /// <param name="deletedBy">User or system that deleted the entity</param>
    public virtual void MarkAsDeleted(string deletedBy)
    {
        IsDeleted = true;
        SetUpdated(deletedBy);
    }

    /// <summary>
    /// Restores a soft-deleted entity
    /// </summary>
    /// <param name="restoredBy">User or system that restored the entity</param>
    public virtual void Restore(string restoredBy)
    {
        IsDeleted = false;
        SetUpdated(restoredBy);
    }

    /// <summary>
    /// Equality comparison based on Id
    /// </summary>
    public override bool Equals(object? obj)
    {
        if (obj is not BaseEntity<TId> other)
            return false;

        if (ReferenceEquals(this, other))
            return true;

        if (GetType() != other.GetType())
            return false;

        return Id.Equals(other.Id);
    }

    /// <summary>
    /// Hash code based on Id
    /// </summary>
    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }

    /// <summary>
    /// Equality operator
    /// </summary>
    public static bool operator ==(BaseEntity<TId>? left, BaseEntity<TId>? right)
    {
        return Equals(left, right);
    }

    /// <summary>
    /// Inequality operator
    /// </summary>
    public static bool operator !=(BaseEntity<TId>? left, BaseEntity<TId>? right)
    {
        return !Equals(left, right);
    }
}