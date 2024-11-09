using System.ComponentModel.DataAnnotations;

namespace eDereva.Core.Entities
{
    public class Otp
    {
        public Guid Id { get; set; }

        [MaxLength(6)]
        public string Code { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public DateTime ExpiresAt { get; set; }
    }

}
