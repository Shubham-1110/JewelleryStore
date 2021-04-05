using System.ComponentModel.DataAnnotations;

namespace JewelleryStore.Entities
{
    public class User
    {
        [Key]
        public int UserId { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }

        public bool IsPrivileged { get; set; }
    }
}
