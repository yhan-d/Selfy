using System.ComponentModel.DataAnnotations;

namespace Selfy.Web.ViewModels;
public class UserProfileViewModel
{
    [Required(ErrorMessage = "Kullanıcı ad alanı gereklidir")]
    [Display(Name = "Kullanıcı Adı")]
    [StringLength(50)]
    public string UserName { get; set; }


    [Required(ErrorMessage = "Ad alanı gereklidir")]
    [Display(Name = "Ad")]
    [StringLength(50)]
    public string Name { get; set; }

    [Required(ErrorMessage = "Soyad alanı gereklidir")]
    [Display(Name = "Soyad")]
    [StringLength(50)]
    public string Surname { get; set; }

    [Required(ErrorMessage = "Email alanı gereklidir")]
    [Display(Name = "Email")]
    [EmailAddress]
    public string Email { get; set; }

    [Display(Name = "Telefon No")]
    [Required(ErrorMessage = "Telefon alanı gereklidir.")]
    [StringLength(10, MinimumLength = 10, ErrorMessage = "En fazla 10 karakter giriniz.")]
    [DataType(DataType.PhoneNumber)]
    [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Geçersiz Numara.")]
    public string Phone { get; set; }

    public DateTime RegisterDate { get; set; }
}
