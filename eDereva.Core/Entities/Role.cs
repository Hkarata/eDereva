﻿using System.ComponentModel.DataAnnotations;

namespace eDereva.Core.Entities
{
    /// <summary>
    /// Represents a role entity.
    /// </summary>
    public class Role
    {
        /// <summary>
        /// Gets or sets the unique identifier for the role.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the name of the role.
        /// </summary>
        [MaxLength(50)] // Limit to 50 characters max
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the description of the role.
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets a value indicating whether the role is soft deleted.
        /// </summary>
        public bool IsDeleted { get; set; }

        /// <summary>
        /// Gets or sets the collection of users associated with the role.
        /// </summary>
        public ICollection<User>? Users { get; set; }

        /// <summary>
        /// Gets or sets the collection of permissions associated with the role.
        /// </summary>
        public ICollection<Permission>? Permissions { get; set; }
    }
}
