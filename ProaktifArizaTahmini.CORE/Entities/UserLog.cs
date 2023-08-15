using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProaktifArizaTahmini.CORE.Entities
{
    public class UserLog
    {
        [Column("Id")]
        public int ID { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Username { get; set; }
        public string MethodName { get; set; }
        public DateTime LogDate { get; set; }
        public int? UserId { get; set; }
        public User User { get; set; }
    }
}
