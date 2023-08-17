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

        [Column("TC_Kimlik")]
        [StringLength(20)]
        public string? TcIdentificationNumber { get; set; }

        [Column("Kullanici_Adi")]
        [StringLength(50)]
        public string Username { get; set; }

        [Column("Sifre")]
        [StringLength(1000)]
        public string Password { get; set; }

        [Column("İsim")]
        [StringLength(50)]
        public string? Name { get; set; }

        [Column("Soyisim")]
        [StringLength(50)]
        public string? Surname { get; set; }

        [StringLength(200)]
        public string? Email { get; set; }

        [Column("Telefon")]
        [StringLength(20)]
        public string? Phone { get; set; }

        [Column("Cep_Telefonu")]
        [StringLength(20)]
        public string? Mobile { get; set; }

        [Column("Şirket")]
        [StringLength(100)]
        public string? Company { get; set; }

        [Column("Departman")]
        [StringLength(100)]
        public string? Departure { get; set; }

        [Column("Unvan")]
        [StringLength(100)]
        public string? Title { get; set; }

        [Column("Yönetici")]
        [StringLength(500)]
        public string? Manager { get; set; }

        [Column("Dogum_Tarihi")]
        public DateTime? BirthDate { get; set; }

        [Column("Adres")]
        [StringLength(500)]
        public string? Adress { get; set; }

        [Column("Resim")]
        [StringLength(1000)]
        public string? Image { get; set; }

        [StringLength(50)]
        public string? Status { get; set; }
        public int? UserTypeId { get; set; }
        public UserType UserType { get; set; }

        [Column("Son_Giris_Tarihi")]
        public DateTime? LastLoginDate { get; set; }
        public bool? IsActive { get; set; }
        public ICollection<UserLog> UserLogs { get; set; }

    }
}
