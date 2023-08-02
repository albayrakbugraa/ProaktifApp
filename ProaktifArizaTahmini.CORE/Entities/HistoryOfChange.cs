using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProaktifArizaTahmini.CORE.Entities
{
    public class HistoryOfChange
    {
        [Column("Id")]
        public int ID { get; set; }
        [Column("New_IP")]
        public string NewIP { get; set; }
        [Column("Old_IP")]
        public string OldIP { get; set; }
        [Column("Degistirilme_Tarihi")]
        public DateTime ChangedDate { get; set; }
        public int MyDataId { get; set; }
        public MyData MyData { get; set; }
    }
}
