using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BHAS.DbFirst
{
    [MetadataType(typeof(EmployeeMetadata))]
    public partial class Employee
    {
        
    }

    public class EmployeeMetadata
    {
        [Required(ErrorMessage = "Ime je obavezno polje.")]
        [StringLength(20, MinimumLength = 3, ErrorMessage = "Od 3 do 20 karaktera")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Prezime je obavezno polje.")]
        [StringLength(20, MinimumLength = 2, ErrorMessage = "od 2 do 20 karaktera")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "JMBG je obavezno polje.")]
        [StringLength(13, MinimumLength = 13, ErrorMessage = "JMBG je duzine 13 karaktera")]
        public string JMBG { get; set; }

        [Required(ErrorMessage = "Odjel je obavezno polje.")]
        public int DepartmentID { get; set; }

        [Required(ErrorMessage = "Pozicija je obavezno polje.")]
        public string Position { get; set; }

        [Required(ErrorMessage = "Plata je obavezno polje.")]
        [Range(5000, 100000, ErrorMessage = "Plata mora biti izmedju 5.000 i 100.000")]
        public decimal Salary { get; set; }

        [Required(ErrorMessage = "Datum zaposlenja je obavezno polje.")]
        public System.DateTime HireDate { get; set; }

        //[Required(ErrorMessage = "Email je obavezno polje.")]

        [EmailAddress(ErrorMessage = "Email adresa nije ispravnog formata")]
        [StringLength (50, MinimumLength = 6, ErrorMessage = "Email adresa ne smije kraca od 6 znakova")]
        public string Email { get; set; }

    }
}
