using API.Extensions;
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
    /* new Properties*/
    public DateTime DateOfBirth { get; set; }
    public string KnownAs { get; set; }
    public DateTime Created { get; set; } = DateTime.UtcNow;
    public DateTime LastActive { get; set; } = DateTime.UtcNow;
    public string Gender { get; set; }
    public string Introduction { get; set; }
    public string LookingFor { get; set; }
    public string Interests { get; set; }
    public string City { get; set; }
    public string Country { get; set; }
    public List<Photo> Photos { get; set; } = new(); // Core 6 = new();

    //public int GetAge()
    //{
    //  return DateOfBirth.CalculateAge();
    //}

  }
}
