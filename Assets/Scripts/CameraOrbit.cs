using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraOrbit : MonoBehaviour
{
    public GameObject terrainFocus;
    public bool shouldOrbit = false;
    public bool hideGenUI = false;

    public float camOrbitSpeed = 1f;

    //Set up references to UI controlers
    public UIControlStatus statusUI;
    public UIControlSettings settingsUI;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.O))
        {
            shouldOrbit = !shouldOrbit;
        }

        if (Input.GetKeyDown(KeyCode.H))
        {
            hideGenUI = !hideGenUI;
            if (hideGenUI)
            {
                statusUI.Hide();
                settingsUI.Hide();
            } else
            {
                statusUI.Show();
                settingsUI.Show();
            }
        }

        if (shouldOrbit)
        {
            transform.LookAt(terrainFocus.transform);
            transform.Translate(Vector3.right * camOrbitSpeed * Time.deltaTime);
        }
    }
}
