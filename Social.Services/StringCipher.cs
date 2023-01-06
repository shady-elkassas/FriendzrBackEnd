using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace Social.Sercices

{
    public class StringCipher
    {

        public static string EncryptString(string objText)
        {
            string KeyCode = "djdndh ddd ddhdb dbhgdd ";
            string objKeycode = KeyCode;
            byte[] objInitVectorBytes = Encoding.UTF8.GetBytes("HR$2pIjHR$2pIj12");
            byte[] objPlainTextBytes = Encoding.UTF8.GetBytes(objText);
            PasswordDeriveBytes objPassword = new PasswordDeriveBytes(objKeycode, null);
            byte[] objKeyBytes = objPassword.GetBytes(256 / 8);
            RijndaelManaged objSymmetricKey = new RijndaelManaged();
            objSymmetricKey.Mode = CipherMode.CBC;
            ICryptoTransform objEncryptor = objSymmetricKey.CreateEncryptor(objKeyBytes, objInitVectorBytes);
            MemoryStream objMemoryStream = new MemoryStream();
            CryptoStream objCryptoStream = new CryptoStream(objMemoryStream, objEncryptor, CryptoStreamMode.Write);
            objCryptoStream.Write(objPlainTextBytes, 0, objPlainTextBytes.Length);
            objCryptoStream.FlushFinalBlock();
            byte[] objEncrypted = objMemoryStream.ToArray();
            objMemoryStream.Close();
            objCryptoStream.Close();
            return Convert.ToBase64String(objEncrypted);
        }

        public static string TryDecryptString(string EncryptedText)
        {
            try
            {
                string KeyCode = "djdndh ddd ddhdb dbhgdd ";
                string Key = KeyCode;
                byte[] objInitVectorBytes = Encoding.ASCII.GetBytes("HR$2pIjHR$2pIj12");
                byte[] objDeEncryptedText = Convert.FromBase64String(EncryptedText);
                PasswordDeriveBytes objPassword = new PasswordDeriveBytes(Key, null);
                byte[] objKeyBytes = objPassword.GetBytes(256 / 8);
                RijndaelManaged objSymmetricKey = new RijndaelManaged();
                objSymmetricKey.Mode = CipherMode.CBC;
                ICryptoTransform objDecryptor = objSymmetricKey.CreateDecryptor(objKeyBytes, objInitVectorBytes);
                MemoryStream objMemoryStream = new MemoryStream(objDeEncryptedText);
                CryptoStream objCryptoStream = new CryptoStream(objMemoryStream, objDecryptor, CryptoStreamMode.Read);
                byte[] objPlainTextBytes = new byte[objDeEncryptedText.Length];
                int objDecryptedByteCount = objCryptoStream.Read(objPlainTextBytes, 0, objPlainTextBytes.Length);
                objMemoryStream.Close();
                objCryptoStream.Close();
                return Encoding.UTF8.GetString(objPlainTextBytes, 0, objDecryptedByteCount);
            }
            catch
            {
                return null;
            }
        }
    }
}
