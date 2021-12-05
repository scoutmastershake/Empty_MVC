using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Empty_MVC.Models
{
    [Table("Users")]
    public class UsersModel
    {       
        [Column("ID")]
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Required]
        public int? ID { get; set; }
        [Column("UserName")]
        public string? UserName { get; set; }
        [Column("Password")]
        public string Password { get; set; }
        [Column("Role")]
        public string Role { get; set; }
        [Column("Email")]
        public string? Email { get; set; }

        
    }
}
