using System.ComponentModel.DataAnnotations;

namespace LoginApi.Auth.Models;

public class LoginRequest
{
    [Required, EmailAddress, MaxLength(254)]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string Password { get; set; } = string.Empty;
}
