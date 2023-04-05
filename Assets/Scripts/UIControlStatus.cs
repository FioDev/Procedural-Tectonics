using System.Collections;
using System.Collections.Generic;
using System.Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIControlStatus : MonoBehaviour
{
    public TextMeshProUGUI framerateText;
    public TextMeshProUGUI statusText;
    public TextMeshProUGUI progressText;

    public int ticks = 0;
    public float framerate = 0f;
    public string status = "";

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        framerate = 1 / Time.deltaTime;
        if (status == "running")
        {
            ticks++;
        }

        if (framerateText != null)
        {
            framerateText.text = framerate.ToString() + " FPS";
        }

        if (statusText != null)
        {
            statusText.text = status.ToString();
        }

        if (progressText!= null)
        {
            progressText.text = ticks.ToString() + "Years have passed";
        }
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void UpdateStatus(string newStatus)
    {
        status = newStatus;
    }
}
