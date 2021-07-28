using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MenuController : MonoBehaviour
{
    public TextMeshProUGUI text;

    public void StartGame()
    {
        GameManager.Instance.LoadScene(0.1f, "Main");
    }
    public void OnClick()
    {
        
    }
}
