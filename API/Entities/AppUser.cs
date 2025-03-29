using System.ComponentModel.DataAnnotations;

namespace API.Entities
{
  public class AppUser
  {
    public int Id { get; set; }

    [Required, StringLength(100)]
    public string UserName { get; set; }
  }
}
