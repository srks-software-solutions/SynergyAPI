using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Globalization;
using System.Web.Security;
using System.ComponentModel;

namespace SRKSSynergy.Models
{
    public class UsersContext : DbContext
    {
        public UsersContext()
            : base("SRKS_Synergy")
        {
        }

        public DbSet<UserProfile> UserProfiles { get; set; }
    }

    [Table("UserProfile")]
    public class UserProfile
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int UserId { get; set; }
        public string UserName { get; set; }
    }

    public class UserLogin
    {
        [Key]
        public Guid UserID { get; set; }

        [Required(ErrorMessage = "First Name is Required")]
        [Display(Name = "First Name")]
        [StringLength(40, MinimumLength = 1, ErrorMessage = "Minimum Length should be 1 and Maximum is 40")]
        public string FirstName { get; set; }

        [Display(Name = "Middle Name")]
        [StringLength(40, MinimumLength = 1,ErrorMessage = "Minimum Length should be 1 and Maximum is 40")]
        public string MiddleName { get; set; }

        [Display(Name = "Last Name")]
        [StringLength(40, MinimumLength = 1, ErrorMessage = "Minimum Length should be 1 and Maximum is 40")]
        public string LastName { get; set; }

        [StringLength(3, MinimumLength = 2, ErrorMessage = "Minimum Length should be 2 and Maximum is 3")]
        [DefaultValue(91)]
        public string Isd1 { get; set; }

        [StringLength(5, MinimumLength = 2, ErrorMessage = "Minimum Length should be 2 and Maximum is 5")]
        public string Std1 { get; set; }

        [StringLength(9, MinimumLength = 5, ErrorMessage = "Minimum Length should be 5 and Maximum is 9")]
        public string PhoneNo { get; set; }

        public int CPID { get; set; }

        public int ZoneID { get; set; }

        [Display(Name = "Security Answer")]
        [StringLength(40, MinimumLength = 4, ErrorMessage = "Minimum Length should be 4 and Maximum is 40")]
        public string Answer { get; set; }

        [Display(Name = "Designation")]
        [StringLength(40, MinimumLength = 2, ErrorMessage = "Minimum Length should be 2 and Maximum is 40")]
        public string Designation { get; set; }

        [Display(Name = "Username")]
        [StringLength(40, MinimumLength = 4, ErrorMessage = "Minimum Length should be 4 and Maximum is 40")]
        public string Username { get; set; }

        //[Required(ErrorMessage = "Email-ID is Required")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string email { get; set; }

        public int IsDeactivate { get; set; }

        public int IsZoneManager { get; set; }

        public string ContactName
        {

            get
            {
                return FirstName + " " + MiddleName + " " + LastName;
            }
        }

        //public virtual ChannelPartners ChannelPartners { get; set; }
    }

    public class LocalPasswordModel
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Current password")]
        public string OldPassword { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }

    public class ChangePasswordModel
    {
        public string UserName { get; set; }

        public String ID { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Current password")]
        public string OldPassword { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }

    public class ForgotPasswordModel
    {
        [Required]
        [Display(Name = "User name")]
        public string UserName { get; set; }

        [DataType(DataType.EmailAddress)]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Question is Required")]
        [Display(Name = "Question")]
        public string Question { get; set; }

        [Required(ErrorMessage = "Answer is Required")]
        [Display(Name = "Answer")]
        [StringLength(100, MinimumLength = 4)]
        public string Answer { get; set; }
    }

    public class LoginModel
    {
        [Required]
        [Display(Name = "User name")]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }

    public class RegisterModel
    {
        [Required]
        [Display(Name = "User name")]
        [StringLength(40, MinimumLength = 4, ErrorMessage = "Minimum Length should be 4 and Maximum is 40")]
        public string UserName { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        //[Required(ErrorMessage = "Email is required")]
        [DataType(DataType.EmailAddress)]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Question is Required")]
        [Display(Name = "Question")]
        public string Question { get; set; }

        [Required(ErrorMessage = "Answer is Required")]
        [Display(Name = "Answer")]
        [StringLength(40, MinimumLength = 4, ErrorMessage = "Minimum Length should be 4 and Maximum is 40")]
        public string Answer { get; set; }
    }

    public class UserRegistration
    {
        public RegisterModel RegisterModel { get; set; }
        public UserLogin UserLogin { get; set; }
    }
}
