using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;
using System.Security.Cryptography;
using UnityEngine.UI;
using System.Text.RegularExpressions;

public class DSA : MonoBehaviour
{
    public Button backBtn, encodeBtn, decodeBtn;
    public GameObject thisPanel;
    public InputField input, output,hashBox;
    static byte[] dataToEncrypt;
    public GameObject selfText;
    // Start is called before the first frame update
    void Start()
    {
        backBtn.onClick.AddListener(back);
        encodeBtn.onClick.AddListener(encode);
        decodeBtn.onClick.AddListener(decode);
    }

    void encode()
    {
        string data = input.text;
        string Signature = "";
        if (input.text == "")
        {
            input.text = "请输入内容";
            return;
        }
        dataToEncrypt = Encoding.ASCII.GetBytes(data); //将消息解码为字节 

        using (RSACryptoServiceProvider RSA = new RSACryptoServiceProvider())
        {
            byte[] encryptedData = RSAEncryption(dataToEncrypt, RSA.ExportParameters(false), false); //私钥加密 
            //Console.WriteLine($"HashCode: {RSA.ExportParameters(false).GetHashCode()}");
            hashBox.text = RSA.ExportParameters(false).GetHashCode().ToString();
            Signature = Convert.ToBase64String(encryptedData);
            //foreach (var a in encryptedData)
            //    Signature += (a);

            output.text = Signature;
        }

    }

    void decode()
    {
        //string[] data = Regex.Split(output.text, "\\s+", RegexOptions.IgnoreCase);
        //string datas = ""
        string data = Encoding.Default.GetString(dataToEncrypt);
        //var encryptedData = Functions.RSAEncryption(dataToEncrypt, RSA.ExportParameters(false), false); //私钥加密 
        //Debug.Log(encryptedData.Length);

        if (output.text == "")
        {
            output.text = "请输入内容";
            return;
        }
        using (RSACryptoServiceProvider RSA = new RSACryptoServiceProvider())
        {
            byte[] encryptedData = RSAEncryption(dataToEncrypt, RSA.ExportParameters(false), false);
            byte[] decryptedData = RSADecryption(encryptedData, RSA.ExportParameters(true), false); //公钥解密 
            //foreach (var a in decryptedData)
            //    Debug.Log(a + " ");

            input.text = Encoding.Default.GetString(decryptedData);
            if (YesOrNo(data, Encoding.Default.GetString(decryptedData)))
            {
                input.text = "签名正确,签名内容是：" + "\n" + Encoding.Default.GetString(decryptedData);
            }
            else
            {
                input.text = "签名不正确,签名内容是：" + "\n" + Encoding.Default.GetString(decryptedData);
            }

            //Console.WriteLine($"Encrypted data: {Encoding.Default.GetString(decryptedData)}");
        }
    }

    public static byte[] RSAEncryption(byte[] dataToEncrypt, RSAParameters rsaKeyInfo, bool doOAEPPadding)
    {
        try
        {
            byte[] encryptedData;
            using (RSACryptoServiceProvider RSA = new RSACryptoServiceProvider()) //instancja Providera
            {
                RSA.ImportParameters(rsaKeyInfo); //插入公钥所需的 RSA 数据条目 
                encryptedData = RSA.Encrypt(dataToEncrypt, doOAEPPadding);
            }
            return encryptedData;
        }
        catch (CryptographicException e)
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

    public static bool YesOrNo(string encData, string decData)
    {
        if (encData.Equals(decData))
            return true;
        else
            return false;
    }

    private static bool CheckBool(bool b)
    {
        if (b)
            return true;
        else
            return false;
    }

    void back()
    {
        thisPanel.SetActive(false);
        selfText.SetActive(true);
    }
}
