using System.Security.Cryptography;
using System.Text;

namespace ProaktifArizaTahmini.UI.Helper
{
    public class Encryption
    {
        private static byte[] _key;

        private static byte[] _IV;

        static Encryption()
        {
            _key = new byte[] { 186, 135, 9, 220, 254, 101, 67, 33 };
            _IV = new byte[] { 18, 52, 86, 120, 144, 171, 205, 239 };
        }

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
