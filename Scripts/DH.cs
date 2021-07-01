using System;
using System.IO;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;
using Random = System.Random;

public class DH:MonoBehaviour
{
    public GameObject thisPanel;
    //public Button btnBack;
    static byte[] dataToEncrypt;
    public InputField IDa, IDb, TimeEncode, PublicKeyA, PublicKeyB, PrivateKeyA, PrivateKeyB,
        RandomA, RandomB, Ya, Yb, Kab, Kba, SignA, SignB, BynA, AynB, TimeDecode;
    public Button creatBtn,signABtn,BverficaABtn, AverficaBBtn,btnback;
    BigInteger sushu = BigInteger.Parse("87178291199");
    BigInteger G, x, y;
    string time;
    private void Start()
    {
        creatBtn.onClick.AddListener(CreateKeys);
        signABtn.onClick.AddListener(sign);
        BverficaABtn.onClick.AddListener(BverA);
        AverficaBBtn.onClick.AddListener(AverB);
        btnback.onClick.AddListener(back);
    }
    private void Update()
    {
        TimeEncode.text = DateTime.Now.ToString();
    }

    void CreateKeys()
    {
        GetPublicAndPrivateKey(PublicKeyA,PrivateKeyA);
        GetPublicAndPrivateKey(PublicKeyB, PrivateKeyB);
        CreateRandomA();
    }

    void GetPublicAndPrivateKey(InputField publickey,InputField privatekey)
    {
        BigInteger numericBase = 2;
        Random l = new Random();
        int L = l.Next(511, 1023);
        BigInteger p_1 = BigInteger.Pow(numericBase, L);
        BigInteger p = BigInteger.Pow(numericBase, L + 1);
        BigInteger P = 0;

        for (; p_1 < p; p_1++)
        {
            if (IsProbablePrime(p_1))
            {
                P = p_1;
                break;
            }
        }

        BigInteger q_1 = BigInteger.Pow(numericBase, 159);
        BigInteger q = BigInteger.Pow(numericBase, 160);

        BigInteger H = 0;
        BigInteger ex = 0;
        for (BigInteger i = 1; i < P - 1; i++)
        {
            ex = BigInteger.Divide((BigInteger.Subtract(P, 1)), q);

            H = BigInteger.ModPow(i, ex, p);
            if (H > 1)
            {
                H = i;
                break;
            }
        }

        BigInteger G = BigInteger.ModPow(H, ex, p);

        long unixTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        Random x = new Random();
        long num = x.Next(10000000, 10000000 * 3);
        BigInteger X = BigInteger.Divide(q, num * 100) * unixTime;
        BigInteger Y = BigInteger.ModPow(G, X, p);
        publickey.text = Y.ToString();
        //Console.WriteLine(Convert.ToBase64String(Y.ToByteArray()));

        privatekey.text = (Convert.ToBase64String(X.ToByteArray()));
    }
    void CreateRandomA()
    {
        Random g = new Random();
        int gg = g.Next(2, 999999999);
        G = BigInteger.Multiply(gg,g.Next(2, 999999));
        x = BigInteger.Multiply(gg, g.Next(2, 9999));
        RandomA.text = x.ToString();
        BigInteger ya = BigInteger.ModPow(G, x, sushu);
        Ya.text = ya.ToString();
    }
    public bool IsProbablePrime(BigInteger source)
    {
        int certainty = 2;
        if (source == 2 || source == 3)
            return true;
        if (source < 2 || source % 2 == 0)
            return false;

        BigInteger d = source - 1;
        int s = 0;

        while (d % 2 == 0)
        {
            d /= 2;
            s += 1;
        }

        RandomNumberGenerator rng = RandomNumberGenerator.Create();
        byte[] bytes = new byte[source.ToByteArray().LongLength];
        BigInteger a;

        for (int i = 0; i < certainty; i++)
        {
            do
            {
                rng.GetBytes(bytes);
                a = new BigInteger(bytes);
            }
            while (a < 2 || a >= source - 2);

            BigInteger x = BigInteger.ModPow(a, d, source);
            if (x == 1 || x == source - 1)
                continue;

            for (int r = 1; r < s; r++)
            {
                x = BigInteger.ModPow(x, 2, source);
                if (x == 1)
                    return false;
                if (x == source - 1)
                    break;
            }

            if (x != source - 1)
                return false;
        }

        return true;
    }
    void sign()
    {
        if(IDa.text == string.Empty)
        {
            IDa.text = "请输入用户名！";
            return;
        }
        time = DateTime.Now.ToString();
        string content = IDa.text + "-" + Ya.text + "|" + time;
        encode(content,SignA);

    }

    void BverA()
    {
        decode(BynA);
        CreateRandomB();
        Kba.text = BigInteger.ModPow(G, BigInteger.Multiply(x, y), sushu).ToString();
        string content = IDa.text + "-" + Ya.text + "|" + time;
        encode(content, SignB);
    }

    void AverB()
    {
        decode(AynB);
        Kab.text = BigInteger.ModPow(G, BigInteger.Multiply(y, x), sushu).ToString();
        decodeTime();
    }
    void decodeTime()
    {
        TimeDecode.text = time;
    }

    void CreateRandomB()
    {
        Random g = new Random();
        int gg = g.Next(2, 999999999);
        y = BigInteger.Multiply(gg, g.Next(2, 9999));
        RandomB.text = x.ToString();
        BigInteger yb = BigInteger.ModPow(G, y, sushu);
        Yb.text = yb.ToString();
    }
    void encode(string input,InputField sign)
    {
        string data = input;
        dataToEncrypt = Encoding.ASCII.GetBytes(data); //将消息解码为字节 

        using (RSACryptoServiceProvider RSA = new RSACryptoServiceProvider())
        {
            byte[] encryptedData = DSA.RSAEncryption(dataToEncrypt, RSA.ExportParameters(false), false); //私钥加密 
            //Console.WriteLine($"HashCode: {RSA.ExportParameters(false).GetHashCode()}");
            string hashBox = RSA.ExportParameters(false).GetHashCode().ToString();
            sign.text = Convert.ToBase64String(encryptedData);

        }

    }

    void decode(InputField input)
    {
        //string[] data = Regex.Split(output.text, "\\s+", RegexOptions.IgnoreCase);
        //string datas = ""
        string data = Encoding.Default.GetString(dataToEncrypt);

        using (RSACryptoServiceProvider RSA = new RSACryptoServiceProvider())
        {
            byte[] encryptedData = DSA.RSAEncryption(dataToEncrypt, RSA.ExportParameters(false), false);
            byte[] decryptedData = DSA.RSADecryption(encryptedData, RSA.ExportParameters(true), false); //公钥解密 
            //foreach (var a in decryptedData)
            //    Debug.Log(a + " ");

            input.text = Encoding.Default.GetString(decryptedData);
            if (DSA.YesOrNo(data, Encoding.Default.GetString(decryptedData)))
            {
                input.text = "签名正确";
            }
            else
            {
                input.text = "签名不正确";
            }

            //Console.WriteLine($"Encrypted data: {Encoding.Default.GetString(decryptedData)}");
        }
    }

    void back()
    {
        thisPanel.SetActive(false);
    }
}
