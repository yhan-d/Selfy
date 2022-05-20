using System.ComponentModel.DataAnnotations;

namespace Selfy.Web.ViewModels;

public class ResetPasswordViewModel
{
    [Required(ErrorMessage = "Yeni şifre alanı gereklidir.")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "Şifreniz minimum 6 karakterli olmalıdır!")]
    [DataType(DataType.Password)]
    [Display(Name = "Yeni Şifre")]
    public string NewPassword { get; set; }

    [Required(ErrorMessage = "Şifre tekrar alanı gereklidir.")]
    [DataType(DataType.Password)]
    [Display(Name = "Yeni Şifre Tekrar")]
    [Compare(nameof(NewPassword), ErrorMessage = "Şifreler uyuşmuyor")]
    public string ConfirmNewPassword { get; set; }
    public string Code { get; set; }
    public string UserId { get; set; }
}
    


