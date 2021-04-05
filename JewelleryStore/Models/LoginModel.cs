using JewelleryStore.Common;
using System.ComponentModel.DataAnnotations;

namespace JewelleryStore.Models
{
    public class LoginModel
    {
        [Required(ErrorMessage = ErrorMessageConstants.EmptyUsername)]
        public string Username { get; set; }

        [Required(ErrorMessage = ErrorMessageConstants.EmptyPassword)]
        public string Password { get; set; }
    }
}
