using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComtradeApp.Models
{
    public class MyData
    {
        [Column("Id")]
        public int ID { get; set; }
        [Column("Avcilar_Tm")]
        public string AvcilarTM { get; set; }
        public string? kV { get; set; }
        [Column("Hucre_No")]
        public string? HucreNo { get; set; }
        [Column("Tm_kV_Hucre")]
        public string TmKvHucre
        {
            get { return $"{AvcilarTM}_{kV}_{HucreNo}"; }
            set { }
        }
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
