using UnityEngine;
using System;
using System.Security.Cryptography;
using System.Text;
using UnityEngine.UI;


public class SHA1code : MonoBehaviour
{

    public InputField InputBox, OutputBox;
    public Button backBtn, encodeBtn;
    public GameObject thisPanel;
    public GameObject selfText;
    private void Start()
    {
        backBtn.onClick.AddListener(Back);
        encodeBtn.onClick.AddListener(encode);
        InputBox.text = null;
        OutputBox.text = null;
    }

    void encode()
    {
        if(InputBox.text == "")
        {
            OutputBox.text = "请输入要加密的内容！！";
            return;
        }
        else
        {
            OutputBox.text = SHA1Input(InputBox.text);
            return;
        }
    }

    public static string SHA1Input(string content)
    {
        return SHA1Encode(content, Encoding.UTF8);
    }

    public static string SHA1Encode(string content, Encoding encode)
    {
        try
        {
            SHA1 sha1 = new SHA1CryptoServiceProvider();
            byte[] bytes_in = encode.GetBytes(content);
            byte[] bytes_out = sha1.ComputeHash(bytes_in);
            sha1.Dispose();
            string result = BitConverter.ToString(bytes_out);
            result = result.Replace("-", "");
            return result.ToUpper();
        }
        catch (Exception ex)
        {
            throw new Exception("SHA1加密出错：" + ex.Message);
        }

    }

    void Back()
    {
        thisPanel.SetActive(false);
        selfText.SetActive(true);
    }
}
