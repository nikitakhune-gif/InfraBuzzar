using System.ComponentModel.DataAnnotations;

namespace InfraBuzzar.Models
{
    public class User
    {
        [Key]
        public long UserId { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }

        public string Role { get; set; } = "User";

        public int Active { get; set; } = 1;

        public DateTime RegistrationDate { get; set; } = DateTime.Now;
    }
}
