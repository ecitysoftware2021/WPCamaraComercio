using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace WPFCCMedellin.Classes
{
    public static class Encryptor
    {
        /***
         * 
         * Encripto el texto utilizando una llave AES de 128bits y 
         * un cifrado de bloqueo de cadena, y retorno un string codificado en base 64
         */
        public static String Encrypt(String plainText)
        {
            try
            {
                var plainBytes = Encoding.UTF8.GetBytes(plainText);
                var encryptBity = GetRijndaelManaged(Assembly.GetExecutingAssembly().EntryPoint.DeclaringType.Namespace).CreateEncryptor().TransformFinalBlock(plainBytes, 0, plainBytes.Length);
                return Convert.ToBase64String(encryptBity);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static String Decrypt(String encryptedText)
        {
            try
            {
                var encryptedBytes = Convert.FromBase64String(encryptedText);
                var decryptByte = GetRijndaelManaged(Assembly.GetExecutingAssembly().EntryPoint.DeclaringType.Namespace).CreateDecryptor().TransformFinalBlock(encryptedBytes, 0, encryptedBytes.Length);
                return Encoding.UTF8.GetString(decryptByte);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        //Creo el objeto RM con la configuracion necesario para ser compatible con Multired
        private static RijndaelManaged GetRijndaelManaged(String secretKey)
        {
            try
            {
                var keyBytes = new byte[16];
                var secretKeyBytes = Encoding.UTF8.GetBytes(secretKey);
                Array.Copy(secretKeyBytes, keyBytes, Math.Min(keyBytes.Length, secretKeyBytes.Length));
                return new RijndaelManaged
                {
                    Mode = CipherMode.CBC,
                    Padding = PaddingMode.PKCS7,
                    KeySize = 128,
                    BlockSize = 128,
                    Key = keyBytes,
                    IV = keyBytes
                };
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
