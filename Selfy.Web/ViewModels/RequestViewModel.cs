using System.ComponentModel.DataAnnotations;

namespace Selfy.Web.ViewModels
{
    public class RequestViewModel
    {
        [Display(Name = "Ad")]
        [Required(ErrorMessage = "Ad alanı gereklidir.")]
        [StringLength(50)]
        public string Name { get; set; }

        [Display(Name = "Soyad")]
        [Required(ErrorMessage = "Soyad alanı gereklidir.")]
        [StringLength(50)]
        public string Surname { get; set; }

        [EmailAddress]
        [Required(ErrorMessage = "E-Posta alanı gereklidir.")]
        public string Email { get; set; }

        [Display(Name = "Adres")]
        [Required(ErrorMessage = "Adres alanı gereklidir.")]
        public string Address { get; set;}
        public DateTime Date { get;}= DateTime.Now;

        [Display(Name = "Talep")]
        [Required(ErrorMessage = "Talep alanını doldurunuz.")]
        public string TextOfRequest { get; set; }

        [Display(Name = "Marka")]
        [Required(ErrorMessage = "Marka alanı gereklidir.")]
        public string Brand { get; set; }

        [Display(Name = "Kategori")]
        [Required(ErrorMessage = "Kategori alanı gereklidir.")]
        public string Category { get; set; }


    }
}
