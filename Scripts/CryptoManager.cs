using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CryptoManager : MonoBehaviour
{
    [Header("按钮")]
    public Button CaesarBtn, HillBtn, SHA1Btn, LFSRBtn, RSABtn, DESBtn, AESBtn,DSABtn;
    [Header("界面")]
    public GameObject CaesarPanel, HillPanel, SHA1Panel, LFSRPanel, RSAPanel, DESPanel, AESPanel,DSAPanel;
    public GameObject myself;
    // Start is called before the first frame update
    void Start()
    {
        CaesarBtn.onClick.AddListener(Caesar);
        HillBtn.onClick.AddListener(Hill);
        LFSRBtn.onClick.AddListener(LFSR);
        SHA1Btn.onClick.AddListener(SHA1);
        RSABtn.onClick.AddListener(RSA);
        DESBtn.onClick.AddListener(DES);
        AESBtn.onClick.AddListener(AES);
        DSABtn.onClick.AddListener(DSA);
    }

    void Caesar()
    {
        CaesarPanel.SetActive(true);
        myself.SetActive(false);
    }

    void Hill()
    {
        HillPanel.SetActive(true);
        myself.SetActive(false);
    }

    void SHA1()
    {
        SHA1Panel.SetActive(true);
        myself.SetActive(false);
    }

    void RSA()
    {
        RSAPanel.SetActive(true);
        myself.SetActive(false);
    }

    void LFSR()
    {
        LFSRPanel.SetActive(true);
        myself.SetActive(false);
    }

    void DES()
    {
        DESPanel.SetActive(true);
        myself.SetActive(false);
    }

    void AES()
    {
        AESPanel.SetActive(true);
        myself.SetActive(false);
    }

    void DSA()
    {
        DSAPanel.SetActive(true);
        myself.SetActive(false);
    }
}
