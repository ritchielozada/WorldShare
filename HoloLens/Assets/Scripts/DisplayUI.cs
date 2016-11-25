using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using HoloToolkit.Unity;
using UnityEngine.UI;

public class DisplayUI : Singleton<DisplayUI>
{    
    [SerializeField]
    private Text DisplayText;

    private Queue<string> msgQueue;

    void Start()
    {       
        msgQueue = new Queue<string>();
        msgQueue.Enqueue("READY");
    }

    private void DisplayTextString()
    {
        StringBuilder sb = new StringBuilder();
        foreach (var s in msgQueue)
        {
            sb.AppendLine(s);
        }
        DisplayText.text = sb.ToString();
    }

    public void AppendText(string msg)
    {
        while (msgQueue.Count >= 8)
        {
            msgQueue.Dequeue();
        }
        msgQueue.Enqueue(msg);
        DisplayTextString();
    }
}
