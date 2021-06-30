using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text.RegularExpressions;
using UnityEngine.UI;

public class LFSR : MonoBehaviour
{
	int[] a = new int[31];
	public Button backBtn, encodeBtn, decodeBtn;
	public GameObject thisPanel;
	public InputField input, output;
	public GameObject selfText;
	// Start is called before the first frame update
	void Start()
    {
		SecretKeyCreat();
		backBtn.onClick.AddListener(Back);
		encodeBtn.onClick.AddListener(encode);
		decodeBtn.onClick.AddListener(decode);
	}
	
	public string SecretKeyCreat()
	{
		a[0] = 1;
		a[1] = 1;
		a[2] = 0;
		a[3] = 0;
		a[4] = 1;
		for (int k = 5; k < 31; ++k)
		{
			a[k] = (a[k - 2] + a[k - 5]) % 2;
		}
		string result = "";
		for (int jj = 0; jj < 31; ++jj)
		{
			result += (a[jj] + " ");
		}
		return result;
	}

	public void encode()
	{
		if(input.text == "")
		{
			output.text = "请输入要加密的内容！！";
			return;
		}
		string msg = input.text;
		int i = 0, j = 0;
		string result = "";
		for (int len = 0; len < msg.Length; len++)
		{
			int sum = 0;
			for (j = 0; j < 8; ++j)
			{
				sum += (int)Math.Pow(2, (7 - j)) * a[(i + j) % 31];
			}
			if (i + j > 32)
			{
				i = (i + j - 1) % 31 + 1;
			}
			else
			{
				i = i + 8;
			}
			//Console.WriteLine(sum);
			//tmp.Add(((int)msg[len] ^ sum));
			result += (((int)msg[len]) ^ sum).ToString() + " ";
		}
		output.text = result;
	}

	public void decode()
	{
		if (output.text == "")
		{
			output.text = "请输入要解密的内容！！";
			return;
		}
		string msg = output.text;
		int i = 0, j = 0;
		int tmpchar = 0;
		string res = "";
		string[] tmp = Regex.Split(msg, "\\s+", RegexOptions.IgnoreCase);
		if (!IsInt(tmp[0]))
		{
			output.text = "请输入正确的内容";
			return;
		}
		//Debug.Log(IsInt(tmp[0]) + tmp[0]);
		for (int len = 0; len < tmp.Length; len++)
		{
			if (tmp[len] != "")
			{
				int sum = 0;
				for (j = 0; j < 8; ++j)
				{
					sum += (int)Math.Pow(2, (7 - j)) * a[(i + j) % 31];
				}
				if (i + j > 32)
				{
					i = (i + j - 1) % 31 + 1;
				}
				else
				{
					i = i + 8;
				}
				tmpchar = (int.Parse(tmp[len]) ^ sum);
				res += (char)tmpchar;
			}
		}
		input.text = res;
	}


	void Back()
	{
		thisPanel.SetActive(false);
		selfText.SetActive(true);
	}

	public bool IsInt(string str)
	{
		try
		{
			int a = Convert.ToInt32(str);
			return true;
		}
		catch
		{
			return false;
		}
	}
}
