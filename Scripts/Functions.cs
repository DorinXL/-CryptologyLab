using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace DigitalSignature
{
    public static class Functions
    {
        public static byte[] RSAEncryption(byte[] dataToEncrypt, RSAParameters rsaKeyInfo, bool doOAEPPadding)
        {
            try
            {
                byte[] encryptedData;
                using (RSACryptoServiceProvider RSA = new RSACryptoServiceProvider()) //instancja Providera
                {
                    RSA.ImportParameters(rsaKeyInfo); //wprowadzanie danych RSA wymagane do wstawienia klucza publicznego
                    encryptedData = RSA.Encrypt(dataToEncrypt, doOAEPPadding);
                }
                return encryptedData;
            }
            catch (CryptographicException e) //wyswietlanie
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }

        public static byte[] RSADecryption(byte[] dataToDecrypt, RSAParameters rsaKeyInfo, bool doAEPPadding)
        {
            try
            {
                byte[] decryptedData;
                using (RSACryptoServiceProvider RSA = new RSACryptoServiceProvider())
                {
                    RSA.ImportParameters(rsaKeyInfo);  //wprowadzanie danych RSA wymagane do wstawienia klucza publicznego
                    decryptedData = RSA.Decrypt(dataToDecrypt, doAEPPadding);
                }
                return decryptedData;
            }
            catch (CryptographicException e) //wyswietlanie
            {
                Console.WriteLine(e.ToString());
                return null;
            }
        }
    }
}
