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

        public int? UserId { get; set; }

        [StringLength(50)]
        public string Username { get; set; }

        [StringLength(1000)]
        public string Password { get; set; }

        [StringLength(100)]
        public string MethodName { get; set; }

        public int LogType { get; set; }

        public bool LogResult { get; set; }

        public DateTime LogDate { get; set; }

        [StringLength(50)]
        public string IpAdress { get; set; }

        public string ExceptionMessage { get; set; }

        public string Environment { get; set; }

        public User User { get; set; }
    }
}
