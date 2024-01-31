using System;
using System.Buffers.Text;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ComtradeApp.Helper
{
    /// <summary>
    /// Encryption sınıfı, verileri şifrelemek ve şifrelenmiş verileri çözmek için kullanılan basit bir şifreleme yardımcı sınıfını temsil eder. Bu sınıf, birçok kriptografik işlemi gerçekleştirmek için .NET Framework tarafından sağlanan DESCryptoServiceProvider sınıfını kullanır.
    /// Bu sınıf, basit bir şifreleme işlemi gerçekleştirmek için kullanılabilir, ancak güvenlik açısından daha fazla karmaşıklık ve güvenlik gerektiren uygulamalar için daha güçlü şifreleme yöntemleri tercih edilmelidir.
    /// </summary>
    public class Encryption
    {
        /// <summary>
        /// _key ve _IV özellikleri, şifreleme ve çözme işlemleri için kullanılan anahtar (key) ve başlatma vektörünü (initialization vector) temsil eder. Bu anahtar ve başlatma vektörü, şifrelemenin ve çözmenin tutarlı olmasını sağlamak için kullanılır.
        /// </summary>
        private static byte[] _key;

        private static byte[] _IV;

        static Encryption()
        {
            _key = new byte[] { 186, 135, 9, 220, 254, 101, 67, 33 };
            _IV = new byte[] { 18, 52, 86, 120, 144, 171, 205, 239 };
        }
        /// <summary>
        /// Bu metot, şifrelenmiş veriyi çözmek için kullanılır. İşlem şu adımları izler:
        /// Base64 kodlaması kullanarak şifrelenmiş veriyi byte dizisine dönüştürür.
        /// Çözme işlemi için bir DESCryptoServiceProvider nesnesi oluşturur.
        /// Şifrelenmiş veriyi çözer ve sonucu bir byte dizisine yazar.
        /// Çözülmüş veriyi UTF-8 kodlaması kullanarak bir dizeye dönüştürür.
        /// </summary>
        /// <param name="data"></param>
        /// <returns>Çözülmüş veriyi döndürür.</returns>
        public static string Decrypt(String data)
        {
            byte[] numArray = Convert.FromBase64String(data);
            DESCryptoServiceProvider dESCryptoServiceProvider = new DESCryptoServiceProvider();
            MemoryStream memoryStream = new MemoryStream();
            CryptoStream cryptoStream = new CryptoStream(memoryStream,
                dESCryptoServiceProvider.CreateDecryptor(_key, _IV), CryptoStreamMode.Write);
            cryptoStream.Write(numArray, 0, (int)numArray.Length);
            cryptoStream.FlushFinalBlock();
            return Encoding.UTF8.GetString(memoryStream.ToArray());
        }
        /// <summary>
        /// Bu metot, gelen metni şifrelemek için kullanılır. İşlem şu adımları izler:
        /// Gelen metni byte dizisine dönüştürür.
        /// Şifreleme işlemi için bir DESCryptoServiceProvider nesnesi oluşturur.
        /// Metni şifreler ve sonucu byte dizisine yazar.
        /// Şifrelenmiş veriyi Base64 kodlaması kullanarak bir dizeye dönüştürür.
        /// </summary>
        /// <param name="data"></param>
        /// <returns>Şifrelenmiş veriyi döndürür.</returns>
        public static string Encrypt(String data)
        {
            DESCryptoServiceProvider dESCryptoServiceProvider = new DESCryptoServiceProvider();
            byte[] bytes = Encoding.UTF8.GetBytes(data);
            MemoryStream memoryStream = new MemoryStream();
            CryptoStream cryptoStream = new CryptoStream(memoryStream,
                dESCryptoServiceProvider.CreateEncryptor(_key, _IV), CryptoStreamMode.Write);
            cryptoStream.Write(bytes, 0, (int)bytes.Length);
            cryptoStream.FlushFinalBlock();
            return Convert.ToBase64String(memoryStream.ToArray());
        }
    }
}
