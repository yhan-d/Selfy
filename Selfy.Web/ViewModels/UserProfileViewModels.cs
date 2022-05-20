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


    public DateTime RegisterDate { get; set; }
}
