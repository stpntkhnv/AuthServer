using System.ComponentModel.DataAnnotations;

namespace AuthServer.ViewModel
{
    public class AuthViewModel
    {
        [Required]public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        [Required]public string Password { get; set; }
        [Required]public string ReturnUrl { get; set; }
    }
}