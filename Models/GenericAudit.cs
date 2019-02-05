using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EFCore.Models
{

      public interface IGenericAudit
    {
        /// <summary>
        /// The Primary Key
        /// </summary>
        int GenericAuditId { get; set; }

        /// <summary>
        /// The Primary Key for the entity
        /// </summary>
        string PrimaryKey { get; set; }

        /// <summary>
        /// The type of the entity
        /// </summary>
        string EntityType { get; set; }

        /// <summary>
        /// All the audit data as json
        /// </summary>
        string AuditData { get; set; }

        /// <summary>
        /// The action took: "Created", "Updated", "Removed"
        /// </summary>
        string Action { get; set; }

        /// <summary>
        /// The date the IAuditable table was updated
        /// </summary>
        DateTime AuditDateUtc { get; set; }

        /// <summary>
        /// The identity of the updator
        /// </summary>
        string AuditIdentity { get; set; }

        /// <summary>
        /// The identity display name of the updator
        /// </summary>
        string AuditIdentityDisplayName { get; set; }

        /// <summary>
        /// The HTTP call CorrelationId
        /// </summary>
        string CorrelationId { get; set; }

        /// <summary>
        /// How long the EF call took
        /// </summary>
        int MSDuration { get; set; }

        /// <summary>
        /// The amount of EF entities updated
        /// </summary>
        int NumObjectsEffected { get; set; }

        /// <summary>
        /// If the EF call completed successfully
        /// </summary>
        bool Success { get; set; }

        /// <summary>
        /// Any errors from EF/DBMS
        /// </summary>
        string ErrorMessage { get; set; }
    }
     public class GenericAudit : IGenericAudit
    {
        [Key]
        public int GenericAuditId { get; set; }

        [Required]
        public string PrimaryKey { get; set; }

        [Required]
        public string EntityType { get; set; }

        [Required]
        public string Action { get; set; }

        public DateTime AuditDateUtc { get; set; }

        [Required]
        public string AuditIdentity { get; set; }

        [Required]
        public string AuditIdentityDisplayName { get; set; }

        [Required]
        public string CorrelationId { get; set; }

        public int MSDuration { get; set; }

        public int NumObjectsEffected { get; set; }

        public bool Success { get; set; }

        public string ErrorMessage { get; set; }

        [Required]
        public string AuditData { get; set; }
    }
}
