using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProaktifArizaTahmini.CORE.Entities
{
    public class Disturbance
    {
        [Column("Id")]
        public int ID { get; set; }
        [Column("TM_No")]
        public string TmNo { get; set; }
        public string kV { get; set; }
        [Column("Hucre_No")]
        public string HucreNo { get; set; }
        [Column("Tm_KV_Hucre")]
        public string TmKvHucre { get; set; }
        [Column("Fider_Adi")]
        public string FiderName { get; set; }
        public string IP { get; set; }
        [Column("Role_Model")]
        public string RoleModel { get; set; }
        [Column("Ariza_Saati")]
        public DateTime FaultTime { get; set; }
        [Column("Cfg_Dosya_Yolu")]
        public string CfgFilePath { get; set; }
        [Column("Cfg_Dosyasi", TypeName = "CLOB")]
        public string CfgFileData { get; set; }
        [Column("Dat_Dosya_Yolu")]
        public string DatFilePath { get; set; }
        [Column("Dat_Dosyasi", TypeName = "BLOB")]
        public byte[] DatFileData { get; set; }
        [Column("Comtrade_Dosya_Ismi")]
        public string ComtradeName { get; set; }
        [Column("Rms_Dosya_Yolu")]
        public string? RmsDataPath { get; set; }
        [Column("Instant_Dosya_Yolu")]
        public string? InstantDataPath { get; set; }
        public bool Status { get; set; }
        [Column("SFTP_Durumu")]
        public bool sFtpStatus { get; set; }
        [Column("Gönderilme_Tarihi")]
        public DateTime PutTime { get; set; }
        public int MyDataId { get; set; }
        public MyData MyData { get; set; }
    }
}
