using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
using UnityEditor;
using System.IO;
using UnityEngine.UI;

public class RSACrypt : MonoBehaviour
{
    string privatekey, publickey, data, encrypteddata, signdata;
    public Button encodeBtn,decodeBtn,bakcBtn,signBtn;
    public InputField encodeBox, decodeBox, signInputBox, signOutputBox, privatekeyBox, publickeyBox;
    public GameObject thisPanel;
    public GameObject selfText;
    private void Start()
    {
        /**RSA加密测试,RSA中的密钥对通过SSL工具生成，生成命令如下： 
       * 1 生成RSA私钥： 
       * openssl genrsa -out rsa_private_key.pem 1024 
       *2 生成RSA公钥 
       * openssl rsa -in rsa_private_key.pem -pubout -out rsa_public_key.pem 
       * 
       * 3 将RSA私钥转换成PKCS8格式 
       * openssl pkcs8 -topk8 -inform PEM -in rsa_private_key.pem -outform PEM -nocrypt -out rsa_pub_pk8.pem 
       * 
       * 直接打开rsa_private_key.pem和rsa_pub_pk8.pem文件就可以获取密钥对内容，获取密钥对内容组成字符串时，注意将换行符删除 
       * */
        //rsa_pub_pk8.pem内容
        privatekey = "MIICdwIBADANBgkqhkiG9w0BAQEFAASCAmEwggJdAgEAAoGBAMsrofD5ET//8cozXO6RBNqj/Jt2btGltDZu8NMe0NbKzHjq8gLjCiw1nA/dubESnOrx/oHeNl6q1yjEJZ9U2LVLUU2PIqkLnnGaPOsJM1R72ZKArDKgZT+QelHgXJdOA/TZeS4Ndcms4OUNBCDRQ2uBuQD0FugvF3GRkuynW2yFAgMBAAECgYEAwlEMBOaiqfyIbCTt+Dp5UwhOvP3sBdWfZAR9jt7FTPoP0IKdT0eI3jmz9rTROlub+1XSXrGCfM6XFKVtelNzI1PqEB+QomBhZtwhzSmxrFWCg4q2oeZsqROKlDBDhV8pFhGX9Euo4HxsNJWLcA4Ngt6ZIwV/Drj7uOEA06UxFyECQQD76Fl4rKPOdzC0RBtRZEqxmC32nikwAWz2FqinNzee+tiMeF2OydP1bCTp3R/mo6Li7hqUcV3LjFCf4nFB8K5ZAkEAzniXc7ppAL286XtKlIOVQnxlhL+wDGtbHZ+SppD02OBFoDGPOivYz8yKL7ktgFwfGzRhGKjJXuXgHwmCnvjiDQJAFhgG4OKja1Rg3S6sBrN5KaJjRaIRkrhNSjgqip/5LORrYcaczg09neTiR/Cw/5WSj7y6cBKRW2zvFVbTACmP4QJATgVZzdyKI0KPqXbyhs52T6psPk6lOvwycS5En3a1X2LYTKGNqwC4rEVxjnkeTZwCESio7EWT2q1pFLFmT6Zi3QJBAKwE1Q3l20UikKhDNrAhxv1R3GgLf8d++Oz5nsQL1yL/blwn3/Bm5Zr+S1XYH5Sz7TBitilmFuO2Wy3xI26EQcQ=";
        //rsa_public_key.pem内容
        publickey = "MIGfMA0GCSqGSIb3DQEBAQUAA4GNADCBiQKBgQDLK6Hw+RE///HKM1zukQTao/ybdm7RpbQ2bvDTHtDWysx46vIC4wosNZwP3bmxEpzq8f6B3jZeqtcoxCWfVNi1S1FNjyKpC55xmjzrCTNUe9mSgKwyoGU/kHpR4FyXTgP02XkuDXXJrODlDQQg0UNrgbkA9BboLxdxkZLsp1tshQIDAQAB";
        //加密字符串  
        data = "DorinXL";

        encodeBox.text = data;
        publickeyBox.text = publickey;
        privatekeyBox.text = privatekey;

        encodeBtn.onClick.AddListener(encode);
        decodeBtn.onClick.AddListener(decode);
        signBtn.onClick.AddListener(sign);
        bakcBtn.onClick.AddListener(back);
    }

    /// <summary>  
    /// 签名  
    /// </summary>  
    /// <param name="content">待签名字符串</param>  
    /// <param name="privateKey">私钥</param>  
    /// <param name="input_charset">编码格式</param>  
    /// <returns>签名后字符串</returns>  
    public static string sign(string content, string privateKey, string input_charset)
    {
        byte[] Data = Encoding.GetEncoding(input_charset).GetBytes(content);
        RSACryptoServiceProvider rsa = DecodePemPrivateKey(privateKey);
        SHA1 sh = new SHA1CryptoServiceProvider();
        byte[] signData = rsa.SignData(Data, sh);
        return Convert.ToBase64String(signData);
    }

    /// <summary>  
    /// 验签  
    /// </summary>  
    /// <param name="content">待验签字符串</param>  
    /// <param name="signedString">签名</param>  
    /// <param name="publicKey">公钥</param>  
    /// <param name="input_charset">编码格式</param>  
    /// <returns>true(通过)，false(不通过)</returns>  
    public static bool verify(string content, string signedString, string publicKey, string input_charset)
    {
        bool result = false;
        byte[] Data = Encoding.GetEncoding(input_charset).GetBytes(content);
        byte[] data = Convert.FromBase64String(signedString);
        RSAParameters paraPub = ConvertFromPublicKey(publicKey);
        RSACryptoServiceProvider rsaPub = new RSACryptoServiceProvider();
        rsaPub.ImportParameters(paraPub);
        SHA1 sh = new SHA1CryptoServiceProvider();
        result = rsaPub.VerifyData(Data, sh, data);
        return result;
    }

    /// <summary>  
    /// 加密  
    /// </summary>  
    /// <param name="resData">需要加密的字符串</param>  
    /// <param name="publicKey">公钥</param>  
    /// <param name="input_charset">编码格式</param>  
    /// <returns>明文</returns>  
    public static string encryptData(string resData, string publicKey, string input_charset)
    {
        byte[] DataToEncrypt = Encoding.ASCII.GetBytes(resData);
        string result = encrypt(DataToEncrypt, publicKey, input_charset);
        return result;
    }


    /// <summary>  
    /// 解密  
    /// </summary>  
    /// <param name="resData">加密字符串</param>  
    /// <param name="privateKey">私钥</param>  
    /// <param name="input_charset">编码格式</param>  
    /// <returns>明文</returns>  
    public static string decryptData(string resData, string privateKey, string input_charset)
    {
        byte[] DataToDecrypt = Convert.FromBase64String(resData);
        string result = "";
        for (int j = 0; j < DataToDecrypt.Length / 128; j++)
        {
            byte[] buf = new byte[128];
            for (int i = 0; i < 128; i++)
            {

                buf[i] = DataToDecrypt[i + 128 * j];
            }
            result += decrypt(buf, privateKey, input_charset);
        }
        return result;
    }

    private static string encrypt(byte[] data, string publicKey, string input_charset)
    {
        RSACryptoServiceProvider rsa = DecodePemPublicKey(publicKey);
        SHA1 sh = new SHA1CryptoServiceProvider();
        byte[] result = rsa.Encrypt(data, false);

        return Convert.ToBase64String(result);
    }

    private static string decrypt(byte[] data, string privateKey, string input_charset)
    {
        string result = "";
        RSACryptoServiceProvider rsa = DecodePemPrivateKey(privateKey);
        SHA1 sh = new SHA1CryptoServiceProvider();
        byte[] source = rsa.Decrypt(data, false);
        char[] asciiChars = new char[Encoding.GetEncoding(input_charset).GetCharCount(source, 0, source.Length)];
        Encoding.GetEncoding(input_charset).GetChars(source, 0, source.Length, asciiChars, 0);
        result = new string(asciiChars);
        return result;
    }

    private static RSACryptoServiceProvider DecodePemPublicKey(String pemstr)
    {
        byte[] pkcs8publickkey;
        pkcs8publickkey = Convert.FromBase64String(pemstr);
        if (pkcs8publickkey != null)
        {
            RSACryptoServiceProvider rsa = DecodeRSAPublicKey(pkcs8publickkey);
            return rsa;
        }
        else
            return null;
    }

    private static RSACryptoServiceProvider DecodePemPrivateKey(String pemstr)
    {
        byte[] pkcs8privatekey;
        pkcs8privatekey = Convert.FromBase64String(pemstr);
        if (pkcs8privatekey != null)
        {
            RSACryptoServiceProvider rsa = DecodePrivateKeyInfo(pkcs8privatekey);
            return rsa;
        }
        else
            return null;
    }

    private static RSACryptoServiceProvider DecodePrivateKeyInfo(byte[] pkcs8)
    {
        byte[] SeqOID = { 0x30, 0x0D, 0x06, 0x09, 0x2A, 0x86, 0x48, 0x86, 0xF7, 0x0D, 0x01, 0x01, 0x01, 0x05, 0x00 };
        byte[] seq = new byte[15];

        MemoryStream mem = new MemoryStream(pkcs8);
        int lenstream = (int)mem.Length;
        BinaryReader binr = new BinaryReader(mem);
        byte bt = 0;
        ushort twobytes = 0;

        try
        {
            twobytes = binr.ReadUInt16();
            if (twobytes == 0x8130)
                binr.ReadByte();
            else if (twobytes == 0x8230)
                binr.ReadInt16();
            else
                return null;

            bt = binr.ReadByte();
            if (bt != 0x02)
                return null;

            twobytes = binr.ReadUInt16();

            if (twobytes != 0x0001)
                return null;

            seq = binr.ReadBytes(15);
            if (!CompareBytearrays(seq, SeqOID))
                return null;

            bt = binr.ReadByte();
            if (bt != 0x04)
                return null;

            bt = binr.ReadByte();
            if (bt == 0x81)
                binr.ReadByte();
            else
                if (bt == 0x82)
                binr.ReadUInt16();

            byte[] rsaprivkey = binr.ReadBytes((int)(lenstream - mem.Position));
            RSACryptoServiceProvider rsacsp = DecodeRSAPrivateKey(rsaprivkey);
            return rsacsp;
        }

        catch (Exception)
        {
            return null;
        }

        finally { binr.Close(); }
    }

    private static bool CompareBytearrays(byte[] a, byte[] b)
    {
        if (a.Length != b.Length)
            return false;
        int i = 0;
        foreach (byte c in a)
        {
            if (c != b[i])
                return false;
            i++;
        }
        return true;
    }

    private static RSACryptoServiceProvider DecodeRSAPublicKey(byte[] publickey)
    {
        byte[] SeqOID = { 0x30, 0x0D, 0x06, 0x09, 0x2A, 0x86, 0x48, 0x86, 0xF7, 0x0D, 0x01, 0x01, 0x01, 0x05, 0x00 };
        byte[] seq = new byte[15];
        MemoryStream mem = new MemoryStream(publickey);
        BinaryReader binr = new BinaryReader(mem);
        byte bt = 0;
        ushort twobytes = 0;

        try
        {

            twobytes = binr.ReadUInt16();
            if (twobytes == 0x8130)
                binr.ReadByte();
            else if (twobytes == 0x8230)
                binr.ReadInt16();
            else
                return null;

            seq = binr.ReadBytes(15);
            if (!CompareBytearrays(seq, SeqOID))
                return null;

            twobytes = binr.ReadUInt16();
            if (twobytes == 0x8103)
                binr.ReadByte();
            else if (twobytes == 0x8203)
                binr.ReadInt16();
            else
                return null;

            bt = binr.ReadByte();
            if (bt != 0x00)
                return null;

            twobytes = binr.ReadUInt16();
            if (twobytes == 0x8130)
                binr.ReadByte();
            else if (twobytes == 0x8230)
                binr.ReadInt16();
            else
                return null;

            twobytes = binr.ReadUInt16();
            byte lowbyte = 0x00;
            byte highbyte = 0x00;

            if (twobytes == 0x8102)
                lowbyte = binr.ReadByte();
            else if (twobytes == 0x8202)
            {
                highbyte = binr.ReadByte();
                lowbyte = binr.ReadByte();
            }
            else
                return null;
            byte[] modint = { lowbyte, highbyte, 0x00, 0x00 };
            int modsize = BitConverter.ToInt32(modint, 0);

            byte firstbyte = binr.ReadByte();
            binr.BaseStream.Seek(-1, SeekOrigin.Current);

            if (firstbyte == 0x00)
            {
                binr.ReadByte();
                modsize -= 1;
            }

            byte[] modulus = binr.ReadBytes(modsize);

            if (binr.ReadByte() != 0x02)
                return null;
            int expbytes = (int)binr.ReadByte();
            byte[] exponent = binr.ReadBytes(expbytes);

            RSACryptoServiceProvider RSA = new RSACryptoServiceProvider();
            RSAParameters RSAKeyInfo = new RSAParameters();
            RSAKeyInfo.Modulus = modulus;
            RSAKeyInfo.Exponent = exponent;
            RSA.ImportParameters(RSAKeyInfo);
            return RSA;
        }
        catch (Exception)
        {
            return null;
        }

        finally { binr.Close(); }
    }

    private static RSACryptoServiceProvider DecodeRSAPrivateKey(byte[] privkey)
    {
        byte[] MODULUS, E, D, P, Q, DP, DQ, IQ;

        MemoryStream mem = new MemoryStream(privkey);
        BinaryReader binr = new BinaryReader(mem);
        byte bt = 0;
        ushort twobytes = 0;
        int elems = 0;
        try
        {
            twobytes = binr.ReadUInt16();
            if (twobytes == 0x8130)
                binr.ReadByte();
            else if (twobytes == 0x8230)
                binr.ReadInt16();
            else
                return null;

            twobytes = binr.ReadUInt16();
            if (twobytes != 0x0102)
                return null;
            bt = binr.ReadByte();
            if (bt != 0x00)
                return null;

            elems = GetIntegerSize(binr);
            MODULUS = binr.ReadBytes(elems);

            elems = GetIntegerSize(binr);
            E = binr.ReadBytes(elems);

            elems = GetIntegerSize(binr);
            D = binr.ReadBytes(elems);

            elems = GetIntegerSize(binr);
            P = binr.ReadBytes(elems);

            elems = GetIntegerSize(binr);
            Q = binr.ReadBytes(elems);

            elems = GetIntegerSize(binr);
            DP = binr.ReadBytes(elems);

            elems = GetIntegerSize(binr);
            DQ = binr.ReadBytes(elems);

            elems = GetIntegerSize(binr);
            IQ = binr.ReadBytes(elems);

            RSACryptoServiceProvider RSA = new RSACryptoServiceProvider();
            RSAParameters RSAparams = new RSAParameters();
            RSAparams.Modulus = MODULUS;
            RSAparams.Exponent = E;
            RSAparams.D = D;
            RSAparams.P = P;
            RSAparams.Q = Q;
            RSAparams.DP = DP;
            RSAparams.DQ = DQ;
            RSAparams.InverseQ = IQ;
            RSA.ImportParameters(RSAparams);
            return RSA;
        }
        catch (Exception)
        {
            return null;
        }
        finally { binr.Close(); }
    }

    private static int GetIntegerSize(BinaryReader binr)
    {
        byte bt = 0;
        byte lowbyte = 0x00;
        byte highbyte = 0x00;
        int count = 0;
        bt = binr.ReadByte();
        if (bt != 0x02)
            return 0;
        bt = binr.ReadByte();

        if (bt == 0x81)
            count = binr.ReadByte();
        else
            if (bt == 0x82)
        {
            highbyte = binr.ReadByte();
            lowbyte = binr.ReadByte();
            byte[] modint = { lowbyte, highbyte, 0x00, 0x00 };
            count = BitConverter.ToInt32(modint, 0);
        }
        else
        {
            count = bt;
        }

        while (binr.ReadByte() == 0x00)
        {
            count -= 1;
        }
        binr.BaseStream.Seek(-1, SeekOrigin.Current);
        return count;
    }

    private static RSAParameters ConvertFromPublicKey(string pemFileConent)
    {

        byte[] keyData = Convert.FromBase64String(pemFileConent);
        if (keyData.Length < 162)
        {
            throw new ArgumentException("pem file content is incorrect.");
        }
        byte[] pemModulus = new byte[128];
        byte[] pemPublicExponent = new byte[3];
        Array.Copy(keyData, 29, pemModulus, 0, 128);
        Array.Copy(keyData, 159, pemPublicExponent, 0, 3);
        RSAParameters para = new RSAParameters();
        para.Modulus = pemModulus;
        para.Exponent = pemPublicExponent;
        return para;
    }

    private static RSAParameters ConvertFromPrivateKey(string pemFileConent)
    {
        byte[] keyData = Convert.FromBase64String(pemFileConent);
        if (keyData.Length < 609)
        {
            throw new ArgumentException("pem file content is incorrect.");
        }

        int index = 11;
        byte[] pemModulus = new byte[128];
        Array.Copy(keyData, index, pemModulus, 0, 128);

        index += 128;
        index += 2;
        byte[] pemPublicExponent = new byte[3];
        Array.Copy(keyData, index, pemPublicExponent, 0, 3);

        index += 3;
        index += 4;
        byte[] pemPrivateExponent = new byte[128];
        Array.Copy(keyData, index, pemPrivateExponent, 0, 128);

        index += 128;
        index += ((int)keyData[index + 1] == 64 ? 2 : 3);
        byte[] pemPrime1 = new byte[64];
        Array.Copy(keyData, index, pemPrime1, 0, 64);

        index += 64;
        index += ((int)keyData[index + 1] == 64 ? 2 : 3);//346  
        byte[] pemPrime2 = new byte[64];
        Array.Copy(keyData, index, pemPrime2, 0, 64);

        index += 64;
        index += ((int)keyData[index + 1] == 64 ? 2 : 3);
        byte[] pemExponent1 = new byte[64];
        Array.Copy(keyData, index, pemExponent1, 0, 64);

        index += 64;
        index += ((int)keyData[index + 1] == 64 ? 2 : 3);
        byte[] pemExponent2 = new byte[64];
        Array.Copy(keyData, index, pemExponent2, 0, 64);

        index += 64;
        index += ((int)keyData[index + 1] == 64 ? 2 : 3);
        byte[] pemCoefficient = new byte[64];
        Array.Copy(keyData, index, pemCoefficient, 0, 64);

        RSAParameters para = new RSAParameters();
        para.Modulus = pemModulus;
        para.Exponent = pemPublicExponent;
        para.D = pemPrivateExponent;
        para.P = pemPrime1;
        para.Q = pemPrime2;
        para.DP = pemExponent1;
        para.DQ = pemExponent2;
        para.InverseQ = pemCoefficient;
        return para;
    }
    
    void encode()
    {
        if (encodeBox.text == "")
        {
            encodeBox.text = "请输入要加密的内容！";
            return;
        }
        else
        {
            data = encodeBox.text;
        }

        if (publickeyBox.text == "")
        {
            publickeyBox.text = "请输入私钥";
            return;
        }
        else
        {
            publickey = publickeyBox.text;
        }
        decodeBox.text = RSACrypt.encryptData(data, publickey, "UTF-8");
    }

    void decode()
    {
        if (decodeBox.text == "")
        {
            decodeBox.text = "请输入要加密的内容！";
            return;
        }
        else
        {
            encrypteddata = decodeBox.text;
        }
        if (privatekeyBox.text == "")
        {
            privatekeyBox.text = "请输入私钥";
            return;
        }
        else
        {
            privatekey = privatekeyBox.text;
        }
        encodeBox.text = RSACrypt.decryptData(encrypteddata, privatekey, "UTF-8");
    }

    void sign()
    {
        if (signInputBox.text == "")
        {
            signInputBox.text = "请输入要加密的签名字符串";
            return;
        }
        else
        {
            signdata = signInputBox.text;
        }
        if(privatekeyBox.text == "")
        {
            privatekeyBox.text = "请输入私钥";
            return;
        }
        else
        {
            privatekey = privatekeyBox.text;
        }
        signOutputBox.text =  RSACrypt.sign(signdata, privatekey, "UTF-8");
    }

    void back()
    {
        thisPanel.SetActive(false);
        selfText.SetActive(true);
    }
}

    //static void Main(string[] args)
    //{


        ////解密  
        //string endata = "VbOtmCuc8+8gKXbDyYHAVG6Hnoa7lCphMZePUmxaM7zQ0XD7oTcRY59xNo9xOpwG5YwjbEiHKcuANuKNBlPLkieX5yGSS8iOd1cki+DujqUtffHQicMegubh4vaPhvhBnOflUejtZH1xi2r2a2t8v9xQO+vmZc7gNJ91+3gtFuc=";
        //string datamw = RSACrypt.decryptData(endata, privatekey, "UTF-8");
        //Console.WriteLine("静态加密后的字符串为：" + endata);
        //Console.WriteLine("解密后的字符串内容：" + datamw);

        //签名  
    //    string signdata = "20180522201658IMFINE";
    //    Console.WriteLine("签名前的字符串内容：" + signdata);
    //    string sign = RSACrypt.sign(signdata, privatekey, "UTF-8");
    //    Console.WriteLine("签名后的字符串：" + sign);

    //    string sourceSign = "VEAgvbHavdbOPYSP8jqxJPYTv/FV/Nl3MClHFBN4qvDRM3ixbOKpUY2P1w99edC29C4Q7qNY99jKYJucRM21mBf8RgEnsBqZVIzqnJYafIQ0AFCL7BpNAORM7uns+NxFj2Zse6Kr61lSpEvie1GCzo+iuYYvzPlMlz+W6nScp2A=";
    //    Console.WriteLine("生成的签名:" + sourceSign);
    //    bool realSign1 = RSACrypt.verify("20180522201658IMFINE", sign, publickey, "UTF-8");
    //    Console.WriteLine(realSign1);

    //    bool realSign2 = RSACrypt.verify("20180522201658IMFINEHAHA", sign, publickey, "UTF-8");
    //    Console.WriteLine(realSign2);
    //    Console.ReadLine();
    //}
