using System.ComponentModel.DataAnnotations;
using eDereva.Core.Contracts.Responses;
using eDereva.Core.Enums;

namespace eDereva.Core.Entities
{
    /// <summary>
    /// Represents a user entity.
    /// </summary>
    public class User
    {
        /// <summary>
        /// Gets or sets the National ID Number.
        /// </summary>
        [MaxLength(20), Key]
        public string NIN { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the first name of the user.
        /// </summary>
        [MaxLength(50)]
        public string FirstName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the middle name of the user (optional).
        /// </summary>
        [MaxLength(50)]
        public string? MiddleName { get; set; }

        /// <summary>
        /// Gets or sets the last name of the user.
        /// </summary>
        [MaxLength(50)]
        public string LastName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the gender of the user.
        /// </summary>
        public Sex Sex { get; set; }

        /// <summary>
        /// Gets or sets the date of birth of the user.
        /// </summary>
        [DataType(DataType.Date)]
        [Range(typeof(DateTime), "1/1/1900", "12/31/2100", ErrorMessage = "Date of birth must be between 1/1/1900 and 12/31/2100.")]
        public DateTime DateOfBirth { get; set; }

        /// <summary>
        /// Gets or sets the phone number of the user.
        /// </summary>
        [Phone]
        [MaxLength(15)]
        public string PhoneNumber { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the email address of the user.
        /// </summary>
        [EmailAddress]
        [MaxLength(100)]
        public string? Email { get; set; }

        /// <summary>
        /// Gets or sets the password of the user.
        /// </summary>
        [MinLength(6)]
        public string Password { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the roles associated with the user.
        /// </summary>
        public ICollection<Role>? Roles { get; set; }
    }

    /// <summary>
    /// Provides extension methods for the <see cref="User"/> class.
    /// </summary>
    public static class UserExtensions
    {
        /// <summary>
        /// Gets the age of the user.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <returns>The age of the user.</returns>
        public static int GetAge(this User user)
        {
            var today = DateTime.Today;
            var age = today.Year - user.DateOfBirth.Year;
            if (user.DateOfBirth.Date > today.AddYears(-age))
            {
                age--;
            }
            return age;
        }

        /// <summary>
        /// Maps the user to a <see cref="UserData"/> object.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <returns>A <see cref="UserData"/> object.</returns>
        public static UserData MaptoUserData(this User user)
        {
            return new UserData
            {
                NIN = user.NIN,
                FirstName = user.FirstName,
                MiddleName = user.MiddleName ?? string.Empty,
                LastName = user.LastName,
                Age = user.GetAge(),
                Sex = user.Sex.ToString(),
                PhoneNumber = user.PhoneNumber,
                Email = user.Email!
            };
        }
    }
}
