using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Desk.Models
{
    public class UserSetting
    {
        public int Id { get; set; }

        [Display(Name = "Beállítás neve")]
        public string? Name { get; set; }

        [Display(Name = "Beállítás értéke")]
        public string? Value { get; set; }

        [ForeignKey(nameof(User))]
        public int UserId { get; set; }
        public User? User { get; set; }
    }
}
