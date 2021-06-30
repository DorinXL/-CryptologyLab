using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class menu : MonoBehaviour
{
    public GameObject thisPanel;
    public Button back;
    // Start is called before the first frame update
    void Start()
    {
        back.onClick.AddListener(backToMenu);
    }

    void backToMenu()
    {
        thisPanel.SetActive(false);
    }

}
