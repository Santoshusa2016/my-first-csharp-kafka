using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Post.Query.Domain.Entities
{
    [Table("Comment",Schema ="dbo")]
    public class CommentEntity
    {
        [Key]
        public Guid CommentId { get; set; }
        public string Username { get; set; }
        public DateTime CommentDate { get; set; }
        public string Comment { get; set; }
        public bool IsEdited { get; set; }
        public Guid PostId { get; set; }

        [System.Text.Json.Serialization.JsonIgnore] //to ignore navigation prop in Comments & to shield from circular reference exception
        public virtual PostEntity Post { get; set; }
    }
}