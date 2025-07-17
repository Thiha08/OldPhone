namespace OldPhone.Core.Shared.Entities
{ 
    /// <summary>
    /// Base entity class providing common properties for all domain entities
    /// </summary>
    public abstract class BaseEntity<TKey>
    {
        /// <summary>
        /// Unique identifier for the entity
        /// </summary>
        public TKey Id { get; set; }

        public DateTime CreatedAt { get; set; }
       
        public DateTime? UpdatedAt { get; set; }
    }
} 