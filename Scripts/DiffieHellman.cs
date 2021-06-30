using UnityEngine;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine.UI;
using SecureKeyExchange;

public class DiffieHellman : MonoBehaviour, IDisposable
{
    public Button backBtn, encodeBtn, decodeBtn;
    public GameObject thisPanel;
    public InputField input, output, aliceKey,bobKey;
    DH bob = new DH();
    DH alice = new DH();
    // Start is called before the first frame update
    void Start()
    {
        backBtn.onClick.AddListener(back);
        encodeBtn.onClick.AddListener(encode);
        decodeBtn.onClick.AddListener(decode);
    }

    #region Private Fields
    private Aes aes = null;
    private ECDiffieHellmanCng diffieHellman = null;

    private readonly byte[] publicKey;
    #endregion

    #region Constructor

    void encode()
    {
        //var bob = new DiffieHellman();
        //var alice = new DiffieHellman();
        string text = input.text;
        string msg = "";
        if (input.text == "")
        {
            input.text = "请输入内容";
            return;
        }

        // Bob uses Alice's public key to encrypt his message.
        byte[] secretMessage = bob.Encrypt(alice.PublicKey, text);
        for (int i = 0; i < secretMessage.Length; i++)
        {
            msg += secretMessage[i];
        }
        output.text = msg;
    }

    void decode()
    {
        //var bob = new DiffieHellman();
        //var alice = new DiffieHellman();
        byte[] secretMessage = System.Text.Encoding.UTF8.GetBytes(output.text);
        if (output.text == "")
        {
            output.text = "请输入内容";
            return;
        }

        // Alice uses Bob's public key and IV to decrypt the secret message.
        string decryptedMessage = alice.Decrypt(bob.PublicKey, secretMessage, bob.IV);
        input.text = decryptedMessage;
    }

    //public DiffieHellman()
    //{
    //    this.aes = new AesCryptoServiceProvider();

    //    this.diffieHellman = new ECDiffieHellmanCng
    //    {
    //        KeyDerivationFunction = ECDiffieHellmanKeyDerivationFunction.Hash,
    //        HashAlgorithm = CngAlgorithm.Sha256
    //    };

    //    // This is the public key we will send to the other party
    //    this.publicKey = this.diffieHellman.PublicKey.ToByteArray();
    //}
    #endregion

    #region Public Properties
    public byte[] PublicKey
    {
        get
        {
            return this.publicKey;
        }
    }

    public byte[] IV
    {
        get
        {
            return this.aes.IV;
        }
    }
    #endregion

    #region Public Methods
    public byte[] Encrypt(byte[] publicKey, string secretMessage)
    {
        byte[] encryptedMessage;
        var key = CngKey.Import(publicKey, CngKeyBlobFormat.EccPublicBlob);
        var derivedKey = this.diffieHellman.DeriveKeyMaterial(key); // "Common secret"

        this.aes.Key = derivedKey;

        using (var cipherText = new MemoryStream())
        {
            using (var encryptor = this.aes.CreateEncryptor())
            {
                using (var cryptoStream = new CryptoStream(cipherText, encryptor, CryptoStreamMode.Write))
                {
                    byte[] ciphertextMessage = Encoding.UTF8.GetBytes(secretMessage);
                    cryptoStream.Write(ciphertextMessage, 0, ciphertextMessage.Length);
                }
            }

            encryptedMessage = cipherText.ToArray();
        }

        return encryptedMessage;
    }

    public string Decrypt(byte[] publicKey, byte[] encryptedMessage, byte[] iv)
    {
        string decryptedMessage;
        var key = CngKey.Import(publicKey, CngKeyBlobFormat.EccPublicBlob);
        var derivedKey = this.diffieHellman.DeriveKeyMaterial(key);

        this.aes.Key = derivedKey;
        this.aes.IV = iv;

        using (var plainText = new MemoryStream())
        {
            using (var decryptor = this.aes.CreateDecryptor())
            {
                using (var cryptoStream = new CryptoStream(plainText, decryptor, CryptoStreamMode.Write))
                {
                    cryptoStream.Write(encryptedMessage, 0, encryptedMessage.Length);
                }
            }

            decryptedMessage = Encoding.UTF8.GetString(plainText.ToArray());
        }

        return decryptedMessage;
    }
    #endregion

    #region IDisposable Members
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            if (this.aes != null)
                this.aes.Dispose();

            if (this.diffieHellman != null)
                this.diffieHellman.Dispose();
        }
    }
    #endregion

    void back()
    {
        thisPanel.SetActive(false);
    }
}
