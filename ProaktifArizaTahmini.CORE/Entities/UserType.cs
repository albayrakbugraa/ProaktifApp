using ProaktifArizaTahmini.CORE.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProaktifArizaTahmini.CORE.Entities
{
    public class UserType
    {
        [Column("Id")]
        public int ID { get; set; }        
        public UserTypeNames UserTypeName { get; set; }

        [StringLength(500)]
        public string Description { get; set; }

        public virtual ICollection<User> Users { get; set; }
    }
}
