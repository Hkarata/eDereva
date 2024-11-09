using System.ComponentModel.DataAnnotations;
using eDereva.Core.Enums;
using eDereva.Core.Interfaces;

namespace eDereva.Core.Entities
{
    public class User : ISoftDelete
    {
        public Guid Id { get; set; }

        // NIN: National ID Number
        [MaxLength(20)]
        public string NIN { get; set; } = string.Empty;

        // FirstName: First name of the user
        [MaxLength(50)] // Limit to 50 characters max
        public string FirstName { get; set; } = string.Empty;

        // MiddleName: Middle name of the user (optional)
        [MaxLength(50)] // Limit to 50 characters max
        public string MiddleName { get; set; } = string.Empty;

        // LastName: Last name of the user
        [MaxLength(50)] // Limit to 50 characters max
        public string LastName { get; set; } = string.Empty;

        // Sex: Gender of the user
        public Sex Sex { get; set; }

        // DateOfBirth: User's date of birth
        [DataType(DataType.Date)] // Specifies the type of data for date validation
        [Range(typeof(DateTime), "1/1/1900", "12/31/2100", ErrorMessage = "Date of birth must be between 1/1/1900 and 12/31/2100.")]
        public DateTime DateOfBirth { get; set; }

        // PhoneNumber: User's phone number
        [Phone] // Ensures phone number is in a valid format
        [MaxLength(15)] // Limits phone number to 15 characters max
        public string PhoneNumber { get; set; } = string.Empty;

        // Email: User's email address
        [EmailAddress] // Validates the email format
        [MaxLength(100)] // Limit email length to 100 characters
        public string? Email { get; set; }

        // Password: User's password (ensure it's hashed or encrypted in practice)
        [MinLength(6)] // Minimum password length
        public string Password { get; set; } = string.Empty;

        // Soft delete flag
        public bool IsDeleted { get; set; }

        // Navigation properties
        public ICollection<Role>? Roles { get; set; }
    }
}
