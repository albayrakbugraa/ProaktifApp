using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProaktifArizaTahmini.CORE.Entities
{
    public class User
    {
        [Column("Id")]
        public int ID { get; set; }

        [StringLength(20)]
        public string TcKimlik { get; set; }

        [Required]
        [StringLength(50)]
        public string Username { get; set; }

        [Required]
        [StringLength(1000)]
        public string Password { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        [Required]
        [StringLength(50)]
        public string Surname { get; set; }

        [StringLength(200)]
        public string Email { get; set; }

        [StringLength(20)]
        public string Phone { get; set; }

        [StringLength(20)]
        public string Mobile { get; set; }

        [StringLength(100)]
        public string Company { get; set; }

        [StringLength(100)]
        public string Departure { get; set; }

        [StringLength(100)]
        public string Title { get; set; }

        [StringLength(500)]
        public string Manager { get; set; }

        public DateTime? BirthDate { get; set; }

        [StringLength(500)]
        public string Adress { get; set; }

        [StringLength(1000)]
        public string Image { get; set; }

        [StringLength(50)]
        public string Status { get; set; }

        public int? UserTypeId { get; set; }
        public UserType UserType { get; set; }

        public DateTime? LastLoginDate { get; set; }
        public bool? IsActive { get; set; }
        public ICollection<UserLog> UserLogs { get; set; }

    }
}
