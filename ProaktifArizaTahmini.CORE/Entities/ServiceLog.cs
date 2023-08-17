using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProaktifArizaTahmini.CORE.Entities
{
    public class ServiceLog
    {
        [Column("Id")]
        public int ID { get; set; }
        public string Message { get; set; }
        public string? Exception { get; set; }
        public string ServiceName { get; set; }
        public string LogLevel { get; set; }
        public DateTime TimeStamp { get; set; }
    }
}
