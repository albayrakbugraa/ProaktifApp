using ProaktifArizaTahmini.CORE.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProaktifArizaTahmini.BLL.Models.DTOs
{
    public class RelayInformationDTO
    {
        public int ID { get; set; }
        public string TmNo { get; set; }
        private string _kV;
        public string? kV
        {
            get { return _kV; }
            set
            {
                if (value != null)
                {
                    if (value.EndsWith("kV"))
                    {
                        _kV = value;
                    }
                    else
                    {
                        _kV = value + "kV";
                    }
                }
                else
                {
                    _kV = null;
                }
            }
        }

        public string HucreNo { get; set; }
        public string TmKvHucre
        {
            get { return $"{TmNo}_{kV}_{HucreNo}"; }
            set { }
        }
        public string FiderName { get; set; }
        public string IP { get; set; }
        public int TimeDifference { get; set; }
        public string RoleModel { get; set; }
        public int Port { get; set; }
        public string User { get; set; }
        public string Password { get; set; }
        public string Path { get; set; }
        public bool Status { get; set; }
        public ICollection<Disturbance> Disturbances { get; set; }
    }
}
