using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Security.Cryptography;
using System.Text;
using System.IO;
using UnityEngine.UI;

public class DES : MonoBehaviour
{
    public Button backBtn,encodeBtn,decodeBtn;
    public GameObject thisPanel;
    public InputField input, output, key;
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
        string encryptString = input.text;
        string sKey = key.text;
        if (input.text == "")
        {
            input.text = "请输入内容";
            return;
        }
        if (sKey.Length != 8)
        {
            key.text = "请输入八位字符串";
            return;
        }
        output.text = DesEncrypt(encryptString, sKey);
    }

    void decode()
    {
        string decryptString = output.text;
        string sKey = key.text;
        if (output.text == "")
        {
            output.text = "请输入内容";
            return;
        }
        if(sKey.Length != 8)
        {
            key.text = "请输入八位字符串";
            return;
        }
        input.text = DesDencrypt(decryptString, sKey);
    }
    /// <summary>
    /// DES加密（对应java版）
    /// </summary>
    /// <param name="encryptString"></param>
    /// <param name="sKey"></param>
    /// <returns></returns>
    public static string DesEncrypt(string encryptString, string sKey)
    {

        byte[] keyBytes = Encoding.UTF8.GetBytes(sKey.Substring(0, 8));//只需要前8位即可
        byte[] keyIV = keyBytes;
        byte[] inputByteArray = Encoding.UTF8.GetBytes(encryptString);

        DESCryptoServiceProvider desProvider = new DESCryptoServiceProvider();

        // java 默认的是ECB模式，PKCS5padding；c#默认的CBC模式，PKCS7padding 所以这里我们默认使用ECB方式
        desProvider.Mode = CipherMode.ECB;
        MemoryStream memStream = new MemoryStream();
        CryptoStream crypStream = new CryptoStream(memStream, desProvider.CreateEncryptor(keyBytes, keyIV), CryptoStreamMode.Write);

        crypStream.Write(inputByteArray, 0, inputByteArray.Length);
        crypStream.FlushFinalBlock();
        return Convert.ToBase64String(memStream.ToArray());
    }

    /// <summary>
    /// DES解密方法
    /// </summary>
    /// <param name="decryptString"></param>
    /// <param name="sKey"></param>
    /// <returns></returns>
    public static string DesDencrypt(string decryptString, string sKey)
    {
        byte[] keyBytes = Encoding.UTF8.GetBytes(sKey.Substring(0, 8));
        byte[] keyIV = keyBytes;
        byte[] inputByteArray = Convert.FromBase64String(decryptString);

        DESCryptoServiceProvider desProvider = new DESCryptoServiceProvider();

        // java 默认的是ECB模式，PKCS5padding；c#默认的CBC模式，PKCS7padding 所以这里我们默认使用ECB方式
        desProvider.Mode = CipherMode.ECB;
        MemoryStream memStream = new MemoryStream();
        CryptoStream crypStream = new CryptoStream(memStream, desProvider.CreateDecryptor(keyBytes, keyIV), CryptoStreamMode.Write);

        crypStream.Write(inputByteArray, 0, inputByteArray.Length);
        crypStream.FlushFinalBlock();
        return Encoding.Default.GetString(memStream.ToArray());

    }

    void back()
    {
        thisPanel.SetActive(false);
        selfText.SetActive(true);
    }

    //static public void Main()
    //{
    //    string msg = "123456";
    //    string key = "12345678";
    //    string mmsg = "ED5wLgc3Mnw=";
    //    Console.WriteLine(DesEncrypt(msg, key));
    //    Console.WriteLine(DesDencrypt(mmsg, key));
    //}

}
