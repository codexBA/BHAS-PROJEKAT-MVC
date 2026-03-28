using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BHAS.Models
{
    public class CreateUserViewModel
    {
        [Required, Display(Name = "Ime i prezime")]
        public string FullName { get; set; }

        [Required, EmailAddress, Display(Name = "Email")]
        public string Email { get; set; }

        [Required, StringLength(100, MinimumLength = 6)]
        [DataType(DataType.Password), Display(Name = "Lozinka")]
        public string Password { get; set; }

        [DataType(DataType.Password), Display(Name = "Potvrda lozinke")]
        [Compare("Password", ErrorMessage = "Lozinke se ne podudaraju.")]
        public string ConfirmPassword { get; set; }

        [Required, Display(Name = "Uloga")]
        public string Role { get; set; }

        [Display(Name = "Odjel")]
        public int? DepartmentID { get; set; }
    }

    public class EditUserViewModel
    {
        public string Id { get; set; }

        [Required, Display(Name = "Ime i prezime")]
        public string FullName { get; set; }

        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required, Display(Name = "Uloga")]
        public string Role { get; set; }

        [Display(Name = "Odjel")]
        public int? DepartmentID { get; set; }
    }

    public class UserListItem
    {
        public string Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public IList<string> Roles { get; set; }
        public string DepartmentName { get; set; }
    }
}
