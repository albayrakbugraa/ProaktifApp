using ProaktifArizaTahmini.CORE.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProaktifArizaTahmini.BLL.Models.DTOs
{
    public class MyDataDTO
    {
        public int ID { get; set; }
        public string TmNo { get; set; }
        public string? kV { get; set; }
        public string? HucreNo { get; set; }
        public string TmKvHucre
        {
            get { return $"{TmNo}_{kV}_{HucreNo}"; }
            set { }
        }
        public string? FiderName { get; set; }
        public string? IP { get; set; }
        public string? RoleModel { get; set; }
        public int Port { get; set; }
        public string User { get; set; }
        public string Password { get; set; }
        public string? Path { get; set; }
        public ICollection<Disturbance> Disturbances { get; set; }
    }
}
