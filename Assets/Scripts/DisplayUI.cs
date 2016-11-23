using UnityEngine;
using System.Collections;
using HoloToolkit.Unity;
using UnityEngine.UI;

public class DisplayUI : Singleton<DisplayUI>
{    
    [SerializeField]
    private Text DisplayText;

    void Start()
    {
        DisplayText.text = "READY\n";
    }

    public void AppendText(string msg)
    {
        DisplayText.text += msg + "\n";
    }
}
