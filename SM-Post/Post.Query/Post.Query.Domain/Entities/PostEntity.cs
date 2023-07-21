using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Post.Query.Domain.Entities;

[Table("Post", Schema = "dbo")]
public class PostEntity
{
    //table columns
    [Key]
    public Guid PostId { get; set; }

    public string Author { get; set; }
    public DateTime DatePosted { get; set; }
    public string Message { get; set; }
    public int Likes { get; set; }

    //composition via navigation properties in EFCore
    public virtual ICollection<CommentEntity> Comments { get; set; }
}
