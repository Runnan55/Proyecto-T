using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cinemachine;
using TMPro;

public class CameraTest : MonoBehaviour
{
    public CinemachineVirtualCamera virtualCamera;
    public Slider fovSlider;
    public Slider rotationXSlider;
    public Slider distanceSlider;
    public TextMeshProUGUI fovText;
    public TextMeshProUGUI rotationXText;
    public TextMeshProUGUI distanceText;
    public Button isometricButton;
    public Button defaultButton;
    public Canvas uiCanvas;

    private CinemachineFramingTransposer framingTransposer;

    void Start()
    {
        if (virtualCamera != null)
        {
            framingTransposer = virtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
        }

        fovSlider.minValue = 5;
        fovSlider.maxValue = 100;

        rotationXSlider.minValue = 0;
        rotationXSlider.maxValue = 90;

        distanceSlider.minValue = 5;
        distanceSlider.maxValue = 90;

        fovSlider.onValueChanged.AddListener(UpdateFOV);
        rotationXSlider.onValueChanged.AddListener(UpdateRotationX);
        distanceSlider.onValueChanged.AddListener(UpdateDistance);

        isometricButton.onClick.AddListener(SetIsometricView);
        defaultButton.onClick.AddListener(SetDefaultView);

        SetDefaultView();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F2))
        {
            ToggleCanvas();
        }
    }

    void UpdateFOV(float value)
    {
        if (virtualCamera != null)
        {
            virtualCamera.m_Lens.FieldOfView = value;
        }
        if (fovText != null)
        {
            fovText.text = "FOV: " + value.ToString("F2");
        }
    }

    void UpdateRotationX(float value)
    {
        if (virtualCamera != null)
        {
            Vector3 rotation = virtualCamera.transform.eulerAngles;
            rotation.x = value;
            virtualCamera.transform.eulerAngles = rotation;
        }
        if (rotationXText != null)
        {
            rotationXText.text = "X Rotation: " + value.ToString("F2");
        }
    }

    void UpdateDistance(float value)
    {
        if (framingTransposer != null)
        {
            framingTransposer.m_CameraDistance = value;
        }
        if (distanceText != null)
        {
            distanceText.text = "Distance: " + value.ToString("F2");
        }
    }

    void SetIsometricView()
    {
        if (virtualCamera != null)
        {
            virtualCamera.m_Lens.FieldOfView = 60;
            virtualCamera.transform.eulerAngles = new Vector3(30, 45, 0);

            if (framingTransposer != null)
            {
                framingTransposer.m_CameraDistance = 34;
            }

            fovSlider.value = virtualCamera.m_Lens.FieldOfView;
            rotationXSlider.value = virtualCamera.transform.eulerAngles.x;
            distanceSlider.value = framingTransposer.m_CameraDistance;
        }
    }

    void SetDefaultView()
    {
        if (virtualCamera != null)
        {
            virtualCamera.m_Lens.FieldOfView = 60;
            virtualCamera.transform.eulerAngles = new Vector3(50, 0, 0);

            if (framingTransposer != null)
            {
                framingTransposer.m_CameraDistance = 34;
            }

            fovSlider.value = virtualCamera.m_Lens.FieldOfView;
            rotationXSlider.value = virtualCamera.transform.eulerAngles.x;
            distanceSlider.value = framingTransposer.m_CameraDistance;
        }
    }

    void ToggleCanvas()
    {
        if (uiCanvas != null)
        {
            uiCanvas.enabled = !uiCanvas.enabled;
        }
    }
}