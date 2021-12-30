using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ASGlass.ViewModels
{
    public class MemberRegisterViewModel
    {
        [Required(ErrorMessage ="Istifadeci adini qeyd eliyin")]
        [StringLength(maximumLength: 20, MinimumLength = 6)]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Ad ve Soyadi qeyd eliyin")]
        [StringLength(maximumLength: 50, MinimumLength = 6)]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Email unvaninizi qeyd eliyin")]
        [StringLength(maximumLength: 100, MinimumLength = 6)]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required(ErrorMessage = "Sifreni qeyd eliyin")]
        [StringLength(maximumLength: 25, MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required (ErrorMessage = "Sifrenin tekrarini qeyd eliyin")]
        [StringLength(maximumLength: 25, MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Compare(nameof(Password))]
        public string ConfirmPassword { get; set; }

        [Required]
        [StringLength(maximumLength: 20, MinimumLength = 6)]
        [DataType(DataType.PhoneNumber)]
        public string Phone { get; set; }

    }
}
