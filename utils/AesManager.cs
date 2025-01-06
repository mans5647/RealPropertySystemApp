using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.Xml;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Printing.IndexedProperties;

namespace RealPropertySystemApp.utils
{
    public class AesManager
    {
        
        public static byte[] Encrypt(string data, byte[] ivSeq, byte[] key)
        {
            byte[] encrypted = null;
            using (Aes aes = Aes.Create())
            {
                aes.Key = key;
                aes.IV = ivSeq;
                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                using (MemoryStream stream = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(stream, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            //Write all data to the stream.
                            
                            swEncrypt.Write(data);
                        }
                        encrypted = stream.ToArray();
                    }
                }
            }

            return encrypted;
        }
        public static string Decrypt(byte[] data, byte[] ivSeq, byte[] key)
        {
            string decrypted = null;
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = key;
                aesAlg.IV = ivSeq;


                // Create a decryptor to perform the stream transform.
                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for decryption.
                using (MemoryStream msDecrypt = new MemoryStream(data))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {

                            // Read the decrypted bytes from the decrypting stream
                            // and place them in a string.
                            decrypted = srDecrypt.ReadToEnd();

                        }
                    }
                }
            }

            return decrypted;
        }
    }
}
