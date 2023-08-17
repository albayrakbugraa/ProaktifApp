using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComtradeApp.Models
{
    public class RelayInformation
    {
        [Column("Id")]
        public int ID { get; set; }
        [Column("TM_No")]
        public string TmNo { get; set; }
        public string? kV { get; set; }
        [Column("Hucre_No")]
        public string? HucreNo { get; set; }
        [Column("Tm_kV_Hucre")]
        public string TmKvHucre { get; set; }
        [Column("Fider_Adi")]
        public string? FiderName { get; set; }
        public string? IP { get; set; }
        [Column("Role_Model")]
        public string? RoleModel { get; set; }
        public int Port { get; set; }
        public string User { get; set; }
        public string Password { get; set; }
        public string? Path { get; set; }
        public ICollection<Disturbance> Disturbances { get; set; }
    }
}
