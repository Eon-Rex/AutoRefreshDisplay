using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Web;
using System.IO;
using System.Text;


namespace AmberProject.App_Code
{
    public class Encryption
    {
        public string EncryptPassword(string plainText)
        {
            System.String passPhrase = "Pas5pr@se";
            System.String saltValue = "s@1tValue";
            System.Int32 passwordIterations = 2;
            System.String initVector = "@1B2c3D4e5F6g7H8";
            System.Int32 keySize = 32;
            System.Text.Encoding asciiEncoding;
            System.Text.Encoding utf8Encoding;
            System.Byte[] initVectorBytes;
            System.Byte[] saltValueBytes;
            System.Byte[] plainTextBytes;
            System.Byte[] keyBytes;
            System.Byte[] cipherTextBytes;
            System.Security.Cryptography.Rfc2898DeriveBytes password;
            System.Security.Cryptography.RijndaelManaged symmetricKey;
            System.Security.Cryptography.ICryptoTransform encryptor;
            System.Security.Cryptography.ICryptoTransform decryptor;
            System.IO.MemoryStream memoryStream;
            System.Security.Cryptography.CryptoStream cryptoStream;
            System.String cipherText;
            System.Exception e;
            try
            {
                //new InteropPermission(InteropKind.ClrInterop).assert();



                asciiEncoding = System.Text.Encoding.ASCII;
                utf8Encoding = System.Text.Encoding.UTF8;
                initVectorBytes = asciiEncoding.GetBytes(initVector);
                saltValueBytes = asciiEncoding.GetBytes(saltValue);
                plainTextBytes = utf8Encoding.GetBytes(plainText);
                password = new System.Security.Cryptography.Rfc2898DeriveBytes(
                    passPhrase,
                    saltValueBytes,
                    passwordIterations);



                keyBytes = password.GetBytes(keySize);
                symmetricKey = new System.Security.Cryptography.RijndaelManaged();
                symmetricKey.Mode = System.Security.Cryptography.CipherMode.CBC;
                encryptor = symmetricKey.CreateEncryptor(
                    keyBytes,
                    initVectorBytes);



                memoryStream = new System.IO.MemoryStream();
                cryptoStream = new System.Security.Cryptography.CryptoStream(memoryStream,
                    encryptor,
                    System.Security.Cryptography.CryptoStreamMode.Write);



                cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
                cryptoStream.FlushFinalBlock();



                cipherTextBytes = memoryStream.ToArray();
                cipherText = System.Convert.ToBase64String(cipherTextBytes);



                memoryStream.Close();
                cryptoStream.Close();
                return cipherText;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }



        }



        public static string Encryppassword(System.String cipherText)
        {



            System.String passPhrase = "Pas5pr@se";
            System.String saltValue = "s@1tValue";
            System.Int32 passwordIterations = 2;
            System.String initVector = "@1B2c3D4e5F6g7H8";
            System.Int32 keySize = 32;
            System.Text.Encoding asciiEncoding;

            System.Byte[] initVectorBytes;
            System.Byte[] saltValueBytes;
            System.Byte[] plainTextBytes;
            System.Byte[] cipherTextBytes;
            System.Byte[] keyBytes;
            System.Security.Cryptography.RijndaelManaged symmetricKey;
            System.Security.Cryptography.ICryptoTransform encryptor;
            System.IO.MemoryStream memoryStream;
            asciiEncoding = System.Text.Encoding.ASCII;
            saltValueBytes = asciiEncoding.GetBytes(saltValue);
            System.Security.Cryptography.Rfc2898DeriveBytes password;
            System.Security.Cryptography.CryptoStream cryptoStream;
            System.String plainText;
            plainTextBytes = System.Convert.FromBase64String(cipherText);



            initVectorBytes = asciiEncoding.GetBytes(initVector);



            System.Int32 plainTextBytesLength, encrypterbytecount;




            password = new System.Security.Cryptography.Rfc2898DeriveBytes(
                passPhrase,
                saltValueBytes,
                passwordIterations);
            keyBytes = password.GetBytes(keySize);
            symmetricKey = new System.Security.Cryptography.RijndaelManaged();
            symmetricKey.Mode = CipherMode.CBC;




            encryptor = symmetricKey.CreateEncryptor(
                 keyBytes,
                 initVectorBytes);




            memoryStream = new System.IO.MemoryStream();
            cryptoStream = new System.Security.Cryptography.CryptoStream(memoryStream,
                encryptor,



                System.Security.Cryptography.CryptoStreamMode.Write);




            plainTextBytesLength = plainTextBytes.Length;
            cryptoStream.Write(plainTextBytes, 0, plainTextBytesLength);
            cryptoStream.FlushFinalBlock();




            cipherTextBytes = memoryStream.ToArray();
            cipherText = System.Convert.ToBase64String(cipherTextBytes);





            memoryStream.Close();
            cryptoStream.Close();




            return cipherText;

        }




        public string DecryptPassword(System.String cipherText)
        {
            System.String passPhrase = "Pas5pr@se";
            System.String saltValue = "s@1tValue";
            System.Int32 passwordIterations = 2;
            System.String initVector = "@1B2c3D4e5F6g7H8";
            System.Int32 keySize = 32;
            System.Text.Encoding asciiEncoding;
            System.Text.Encoding utf8Encoding;
            System.Byte[] initVectorBytes;
            System.Byte[] saltValueBytes;
            System.Byte[] plainTextBytes;
            System.Byte[] keyBytes;
            System.Byte[] cipherTextBytes;
            System.Security.Cryptography.Rfc2898DeriveBytes password;
            System.Security.Cryptography.RijndaelManaged symmetricKey;
            System.Security.Cryptography.ICryptoTransform encryptor;
            System.Security.Cryptography.ICryptoTransform decryptor;
            System.IO.MemoryStream memoryStream;
            System.Security.Cryptography.CryptoStream cryptoStream;
            System.String plainText;
            System.Exception e;
            asciiEncoding = System.Text.Encoding.ASCII;
            utf8Encoding = System.Text.Encoding.UTF8;
            initVectorBytes = asciiEncoding.GetBytes(initVector);
            saltValueBytes = asciiEncoding.GetBytes(saltValue);
            cipherTextBytes = System.Convert.FromBase64String(cipherText);
            System.Int32 plainTextBytesLength, cipherTextBytesLength, decryptedByteCount;
            password = new System.Security.Cryptography.Rfc2898DeriveBytes(
                passPhrase,
                saltValueBytes,
                passwordIterations);



            keyBytes = password.GetBytes(keySize);
            symmetricKey = new System.Security.Cryptography.RijndaelManaged();
            symmetricKey.Mode = CipherMode.CBC;
            decryptor = symmetricKey.CreateDecryptor(
                keyBytes,
                initVectorBytes);



            memoryStream = new System.IO.MemoryStream(cipherTextBytes);
            cryptoStream = new System.Security.Cryptography.CryptoStream(
                memoryStream,
                decryptor,
                System.Security.Cryptography.CryptoStreamMode.Read);



            cipherTextBytesLength = cipherTextBytes.Length;
            plainTextBytes = System.Convert.FromBase64String(cipherText);
            plainTextBytesLength = plainTextBytes.Length;
            decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytesLength);



            plainText = utf8Encoding.GetString(plainTextBytes, 0, decryptedByteCount);
            memoryStream.Close();
            cryptoStream.Close();
            return plainText;
            //Console.WriteLine(plainText);
        }
    }
}