using System.ComponentModel.DataAnnotations;

namespace eDereva.Core.Entities
{
    public class Role
    {
        public Guid Id { get; set; }

        // Name: Name of the role
        [MaxLength(50)] // Limit to 50 characters max
        public string Name { get; set; } = string.Empty;


        public string Description { get; set; } = string.Empty;


        // Soft delete flag
        public bool IsDeleted { get; set; }


        // Navigation properties
        public ICollection<User>? Users { get; set; }
        public ICollection<Permission>? Permissions { get; set; }
    }
}
