using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace API.Entities
{
  [Table("Photos")]
  public class Photo
  {
    [Key]
    public int Id { get; set; }
    public string Url { get; set; }
    public bool IsMain { get; set; }
    public int UserId { get; set; }

    [ForeignKey("UserId")]
    public virtual AppUser AppUser { get; set; }
  }

}