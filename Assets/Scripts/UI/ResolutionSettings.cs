/* using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Resolution : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown resolutionDropdown;

    private UnityEngine.Resolution[] resolutions;
    private List<UnityEngine.Resolution> filteredResolutions;

    private float currentRefreshRate;
    private int currentResolutionIndex = 0;

    void Start()
    {
        resolutions = Screen.resolutions;
        filteredResolutions = new List<UnityEngine.Resolution>();

        resolutionDropdown.ClearOptions();
        currentRefreshRate = Screen.currentResolution.refreshRateRatio.value;

        UnityEngine.Resolution customResolution = new UnityEngine.Resolution();
        customResolution.width = 640;
        customResolution.height = 360;
        customResolution.refreshRate = (int)Screen.currentResolution.refreshRate;
        filteredResolutions.Add(customResolution);

        for (int i = 0; i < resolutions.Length; i++)
        {
            if (resolutions[i].refreshRate == currentRefreshRate)
            {
                filteredResolutions.Add(resolutions[i]);
            }
        }

        List<string> options = new List<string>();
        for (int i = 0; i < filteredResolutions.Count; i++)
        {
            string resolutionOption = filteredResolutions[i].width + "x" + filteredResolutions[i].height + " " + filteredResolutions[i].refreshRate + " Hz";
            options.Add(resolutionOption);

            if (filteredResolutions[i].width == Screen.width && filteredResolutions[i].height == Screen.height)
            {
                currentResolutionIndex = i;
            }
        }

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();
    }

    public void SetResolution(int resolutionIndex)
    {
        UnityEngine.Resolution resolution = filteredResolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, true);
    }
} */

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Resolution : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown resolutionDropdown;

    private UnityEngine.Resolution[] resolutions;
    private List<UnityEngine.Resolution> filteredResolutions;

    private float currentRefreshRate;
    private int currentResolutionIndex = 0;

    void Start()
    {
        resolutions = Screen.resolutions;
        filteredResolutions = new List<UnityEngine.Resolution>();

        resolutionDropdown.ClearOptions();
        currentRefreshRate = Screen.currentResolution.refreshRate;

        UnityEngine.Resolution customResolution = new UnityEngine.Resolution();
        customResolution.width = 640;
        customResolution.height = 360;
        customResolution.refreshRate = (int)Screen.currentResolution.refreshRate;
        filteredResolutions.Add(customResolution);

        for (int i = 0; i < resolutions.Length; i++)
        {
            if (resolutions[i].refreshRate == currentRefreshRate)
            {
                filteredResolutions.Add(resolutions[i]);
            }
        }

        List<string> options = new List<string>();
        for (int i = 0; i < filteredResolutions.Count; i++)
        {
            string resolutionOption = filteredResolutions[i].width + "x" + filteredResolutions[i].height + " " + filteredResolutions[i].refreshRate + " Hz";
            options.Add(resolutionOption);

            if (filteredResolutions[i].width == Screen.width && filteredResolutions[i].height == Screen.height)
            {
                currentResolutionIndex = i;
            }
        }

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();
    }

    public void SetResolution(int resolutionIndex)
    {
        UnityEngine.Resolution resolution = filteredResolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, true);
    }
}
