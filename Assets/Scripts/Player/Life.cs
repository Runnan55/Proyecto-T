using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Life : MonoBehaviour
{
    public float maxTime = 600;
    private float currentTime;

    public TextMeshProUGUI timeText;
    public Image timeImage;
    public DeathScreen deathScreen; 

    public LevelManager levelManager;


    void Start()
    {
        currentTime = maxTime;
        timeImage.fillAmount = 1;
                levelManager = FindObjectOfType<LevelManager>();

    }

    void Update()
    {
        if (currentTime > 0)
        {
            currentTime -= Time.deltaTime;
            UpdateTimeText();
            UpdateTimeImage();
        }
        else
            Debug.Log("muerte");
            //levelManager.OnLevelFailed();


        //* DEBUG

        if (Input.GetKeyDown(KeyCode.O))
        {
            ModifyTime(60);
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            ModifyTime(-60);
        }
    }

    public void ModifyTime(float amount)
    {
        currentTime += amount;
        if (currentTime > maxTime)
        {
            currentTime = maxTime;
        }
        else if (currentTime < 0)
        {
            //deathScreen.ShowDeathScreen();
            levelManager.OnLevelFailed();

            currentTime = 0;
        }
        UpdateTimeImage();
    }

    private void UpdateTimeText()
    {
        int minutes = Mathf.FloorToInt(currentTime / 60);
        int seconds = Mathf.FloorToInt(currentTime % 60);
        timeText.text = minutes + ":" + seconds.ToString("00");
    }

    private void UpdateTimeImage()
    {
        timeImage.fillAmount = currentTime / maxTime;
    }
}