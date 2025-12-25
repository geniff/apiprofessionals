using System.ComponentModel.DataAnnotations.Schema;

namespace apiprofessionals.Models
{
    public class UserModel
    {
        public int Id { get; set; }
        [Column("full_name")]
        public string FullName { get; set; }
        public string Email { get; set; }
        public string? Phone { get; set; }
        public string? Role { get; set; }
        [Column("created_at")]
        public DateTime? CreatedAt { get; set; }
        [Column("updated_at")]
        public DateTime? UpdatedAt { get; set; }
        [Column("password_hash")]
        public string? PasswordHash { get; set; }
    }
}
