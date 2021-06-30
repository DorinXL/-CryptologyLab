using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Caesar : MonoBehaviour
{
	public InputField input, output, key;
	public GameObject CaesarPanel;
	public Button ExitCaesarBtn,encodeBtn,decodeBtn;
	public GameObject selfText;
	private void Start()
	{
		input.text = "";
		output.text = "";
		key.text = "";
		ExitCaesarBtn.onClick.AddListener(ExitCaesar);
		encodeBtn.onClick.AddListener(encode);
		decodeBtn.onClick.AddListener(decode);
	}
	void encode() 
	{
		if(input.text == "")
		{
			output.text = "请输入要加密的信息！";
			return;
		}
		if(key.text == "")
		{
			output.text = "请输入key！";
			return;
		}

		string message = input.text;
		string ciphertext = "";
		int keynum = int.Parse(key.text);

		for (int i = 0; i < message.Length; i++)
		{
			if (message[i] >= 'A' && message[i] <= 'Z')
			{
				char tmp = (char)('A' + (message[i] - 'A' + keynum) % 26);
				ciphertext += tmp;
			}
			else if (message[i] >= 'a' && message[i] <= 'z') {
				char tmp = (char)('a' + (message[i] - 'a' + keynum) % 26);
				ciphertext += tmp;
			}
			else
			{
				ciphertext += message[i];
			}
		}
		output.text = ciphertext;
	}

	void decode()
	{
		if (output.text == "")
		{
			input.text = "请输入要加密的信息！";
			return;
		}
		if (key.text == "")
		{
			input.text = "请输入key！";
			return;
		}

		string message = output.text;
		string ciphertext = "";
		int keynum = int.Parse(key.text);

		for (int i = 0; i < message.Length; i++)
		{
			if (message[i] >= 'A' && message[i] <= 'Z')
			{
				char tmp = (char)('A' + (message[i] - 'A' - keynum) % 26);
				ciphertext += tmp;
			}
			else if (message[i] >= 'a' && message[i] <= 'z')
			{
				char tmp = (char)('a' + (message[i] - 'a' - keynum) % 26);
				ciphertext += tmp;
			}
			else
			{
				ciphertext += message[i];
			}
		}
		input.text = ciphertext;

	}

	void ExitCaesar()
	{
		CaesarPanel.SetActive(false);
		selfText.SetActive(true);
	}
}

