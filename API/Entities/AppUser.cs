using System.ComponentModel.DataAnnotations;

namespace API.Entities
{
  public class AppUser
  {
    public int Id { get; set; }

    [Required, StringLength(100)]
    public string UserName { get; set; }
    [Required,MaxLength(100)]
    public string PasswordHash { get; set; } //100
    [Required, MaxLength(100)]
    public string PasswordSalt { get; set; } //100

  }
}
