using System.ComponentModel.DataAnnotations.Schema;

namespace Desk.Models
{
    public class UserGroup
    {
        [ForeignKey(nameof(User))]
        public int UserId { get; set; }
        public User? User { get; set; }


        [ForeignKey(nameof(Group))]
        public int GroupId { get; set; }
        public Group? Group { get; set; }
    }
}
