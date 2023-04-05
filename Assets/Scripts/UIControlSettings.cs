using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UIControlSettings : MonoBehaviour
{
    public GameObject inputSystemCanvas;

    public GameObject plateCanvas;
    public GameObject sizeCanvas;
    public GameObject settingsCanvas;

    #region Plate UI Components
    public TextMeshProUGUI plateIndexDisplay;
    public TextMeshProUGUI plateNumDisplay;
    public TMP_InputField plateSeedDisplay;
    public TMP_InputField plateOffsetXDisplay;
    public TMP_InputField plateOffsetYDisplay;
    public TMP_InputField plateSpeedXDisplay;
    public TMP_InputField plateSpeedYDisplay;

    #endregion

    #region Size UI Components

    public Slider sizeXInput;
    public Slider sizeYInput;
    public TextMeshProUGUI SizeOutput;
    public TextMeshProUGUI SizePreview;

    #endregion

    public Slider ErosionFactor;
    public Slider VolcanicFactor;
    public Slider ErodeBiasLimit;
    public Slider CamOrbitSpeed;
    public CameraOrbit cam;

    bool plateActive = false;
    bool sizeActive = false;
    bool settingActive = false;

    public Lithosphere lithosphereRef;

    int plateIndex = 0;

    int tempX = 10;
    int tempY = 10;

    private void Awake()
    {
        UpdatePlateInfo();
        UpdateSizeDisplay();
    }


    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }

    //Plate Edit 
    #region Plates controls

    public void UpdatePlateInfo()
    {
        plateNumDisplay.text = lithosphereRef.plates.Count.ToString();

        if (lithosphereRef.plates.Count == 0)
        {
            plateIndex = -1;
        }

        //Plate Display index

        plateIndexDisplay.text = "Plate Index: " + plateIndex;
        //reset colour, in case something changed it earlier 
        plateIndexDisplay.color = Color.white;

        //At this point, fail display at every point if plateIndex is -1
        if (plateIndex == -1)
        {
            plateSeedDisplay.text = "No Plate Data";
            plateOffsetXDisplay.text = "0";
            plateOffsetYDisplay.text = "0";
            plateSpeedXDisplay.text = "0";
            plateSpeedYDisplay.text = "0";
            return;
        }
        //if no failure to find plate, display current plate data

        //Plate Seed Display

        plateSeedDisplay.text = lithosphereRef.plates[plateIndex].seed.ToString();

        //plate offset display

        plateOffsetXDisplay.text = lithosphereRef.plates[plateIndex].offset.x.ToString();
        plateOffsetYDisplay.text = lithosphereRef.plates[plateIndex].offset.y.ToString();

        //plate speed display

        plateSpeedXDisplay.text = lithosphereRef.plates[plateIndex].speed.x.ToString();
        plateSpeedYDisplay.text = lithosphereRef.plates[plateIndex].speed.y.ToString();
    }
    public void AddPlateIndex()
    {
        int targetIndex = plateIndex + 1;

        //Wraparound check
        if (lithosphereRef.plates.Count - 1 < targetIndex)
        {
            targetIndex = 0;
        }

        if (lithosphereRef.plates.Count == 0)
        {
            targetIndex = -1;
        }

        plateIndex = targetIndex;

        //Update display
        UpdatePlateInfo();
    }
    public void TakePlateIndex()
    {
        int targetIndex = plateIndex - 1;

        //Wraparound check
        if (0 > targetIndex)
        {
            targetIndex = lithosphereRef.plates.Count - 1;
        }

        if (lithosphereRef.plates.Count == 0)
        {
            targetIndex = -1;
        }

        plateIndex = targetIndex;

        //Update display
        UpdatePlateInfo();

    }
    public void AddNewPlate()
    {
        Plate temp = new Plate();
        lithosphereRef.plates.Add(temp);
        UpdatePlateInfo();
    }
    public void RemoveCurrentPlate()
    {
        //if we dont have 2 plates to operate on, abandon that idea immediately. 
        if (lithosphereRef.plates.Count < 2)
        {
            plateIndexDisplay.color = Color.red;
            return;
        }

        //If we have enough plates, nuke that one, and by default jump to the previous plate

        lithosphereRef.plates.RemoveAt(plateIndex);
        TakePlateIndex();

    }
    public void SetPlateSeed()
    {
        int n;
        bool isNumeric = int.TryParse(plateSeedDisplay.text, out n);

        if (!isNumeric)
        {
            plateSeedDisplay.text = n.ToString();
        }

        lithosphereRef.plates[plateIndex].seed = n;
    }
    public void SetPlateOffsetX()
    {
        float n;
        bool isNumeric = float.TryParse(plateOffsetXDisplay.text, out n);

        if (!isNumeric)
        {
            plateOffsetXDisplay.text = n.ToString();
        }

        lithosphereRef.plates[plateIndex].offset.x = n;
    }
    public void SetPlateOffsetY()
    {
        float n;
        bool isNumeric = float.TryParse(plateOffsetYDisplay.text, out n);

        if (!isNumeric)
        {
            plateOffsetYDisplay.text = n.ToString();
        }

        lithosphereRef.plates[plateIndex].offset.y = n;
    }
    public void SetPlateSpeedX()
    {
        float n;
        bool isNumeric = float.TryParse(plateSpeedXDisplay.text, out n);

        if (!isNumeric)
        {
            plateSpeedXDisplay.text = n.ToString();
        }

        lithosphereRef.plates[plateIndex].speed.x = n;
    }
    public void SetPlateSpeedY()
    {
        float n;
        bool isNumeric = float.TryParse(plateSpeedYDisplay.text, out n);

        if (!isNumeric)
        {
            plateSpeedYDisplay.text = n.ToString();
        }

        lithosphereRef.plates[plateIndex].speed.y = n;
    }

    #endregion

    //Dimensions Edit
    #region Dimensions controls

    private int ValidateMapSizeComponent(int old)
    {
        int temp = old;

        if (temp > 1000)
        {
            temp = 1000;
        }

        if (temp < 10)
        {
            temp = 10;
        }

        return temp;
    }
    public void ApplySizeChange()
    {
        lithosphereRef.mapDepth = tempY;
        lithosphereRef.mapWidth = tempX;

        lithosphereRef.ApplySizeChange(tempX, tempY);
        UpdateSizeDisplay();
    }
    public void UpdateSizeDisplay()
    {
        SizeOutput.text = lithosphereRef.mapWidth.ToString() + " x " + lithosphereRef.mapDepth.ToString();
        SizePreview.text = tempX + " x " + tempY;
        //Display warnings for values above 200
        if (lithosphereRef.mapDepth > 200 || lithosphereRef.mapWidth > 200)
        {
            SizeOutput.color = Color.yellow;
        } else
        {
            SizeOutput.color = Color.white;
        }
    }
    public void SetTerrainDepth()
    {
        int n = sizeXInput.value.ConvertTo<int>();

        tempX = n;

        UpdateSizeDisplay();
    }
    public void SetTerrainWidth()
    {
        int n = sizeYInput.value.ConvertTo<int>();

        tempY = n;

        UpdateSizeDisplay();
    }
    #endregion

    //Simulation Edit
    #region Sim Settings

    public void ApplyVolcanismFactor()
    {
        float n = VolcanicFactor.value;

        lithosphereRef.volcanismFactor = n;
    }
    public void ApplyErosionFactor()
    {
        float n = ErosionFactor.value;

        lithosphereRef.erosionFactor = n;
    }
    public void ApplyErosionBias()
    {
        float n = ErodeBiasLimit.value;

        lithosphereRef.targetTectonicErosionHeight = n;
    }
    public void ApplyCamOrbitSpeed()
    {
        float n = CamOrbitSpeed.value;
        cam.camOrbitSpeed = n;
    }

    #endregion

    public void ShowHidePlateSettings()
    {
        plateActive = !plateActive;

        //Check self for active
        if (plateActive)
        {
            plateCanvas.SetActive(true);
            
            //Set all other to inactive
            if (sizeActive)
            {
                ShowHideSizeSettings();
            }
            if (settingActive)
            {
                ShowHideSimSettings();
            }
        } else
        {
            plateCanvas.SetActive(false);
        }

        //Check if any plates are active. If *any* are, show the input system canvas.
        CheckShowInputcanvas();
        
    }

    public void ShowHideSizeSettings()
    {
        sizeActive = !sizeActive;

        //Check self for active
        if (sizeActive)
        {
            sizeCanvas.SetActive(true);

            //Set all other to inactive
            if (settingActive)
            {
                ShowHideSimSettings();
            }
            if (plateActive)
            {
                ShowHidePlateSettings();
            }
        }
        else
        {
            sizeCanvas.SetActive(false);
        }

        //Check if any plates are active. If *any* are, show the input system canvas.
        CheckShowInputcanvas();

    }

    public void ShowHideSimSettings()
    {
        settingActive = !settingActive;

        //Check self for active
        if (settingActive)
        {
            settingsCanvas.SetActive(true);

            //Set all other to inactive
            if (plateActive)
            {
                ShowHidePlateSettings();
            }
            if (sizeActive)
            {
                ShowHideSizeSettings();
            }
        }
        else
        {
            settingsCanvas.SetActive(false);
        }

        //Check if any plates are active. If *any* are, show the input system canvas.
        CheckShowInputcanvas();

    }

    private void CheckShowInputcanvas()
    {
        if (plateActive || sizeActive || settingActive)
        {
            inputSystemCanvas.SetActive(true);
        } else
        {
            inputSystemCanvas.SetActive(false);
        }
    }
}
