using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Life : MonoBehaviour
{
    public float maxTime = 600;
    private float currentTime;
    public bool isInvincible = false;

    public TextMeshProUGUI timeText;
    public Image timeImage;
    public DeathScreen deathScreen; 

    public LevelManager levelManager;

    public float invincibilityTime = 1;
    public GameObject shield;

    public CharacterController playerController;  

    PlayerMovement playerMovement;

    public bool DamagePlayer = false;

    void Start()
    {
        currentTime = maxTime;
        timeImage.fillAmount = 1;

        levelManager = FindObjectOfType<LevelManager>();
        playerController = GetComponent<CharacterController>();
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

/*         if (Input.GetKeyDown(KeyCode.O))
        {
            ModifyTime(60);
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            ModifyTime(-60);
        } */

        //shield.SetActive(isInvincible);
    }

    public void ModifyTime(float amount)
    {
       
    DamagePlayer = true;
    
        if (isInvincible && amount < 0) return;
  
        currentTime += amount;
        if (currentTime > maxTime)
        {
            currentTime = maxTime;        
 
        }
        else if (currentTime < 0)
        {            
            levelManager.OnLevelFailed();
            currentTime = 0;
        }
        UpdateTimeImage();
   
        if (amount < 0) StartCoroutine(InvincibilityFrames());   
   
        
        
    }

    IEnumerator InvincibilityFrames()
    {
        isInvincible = true;
        shield.SetActive(true);
        yield return new WaitForSeconds(invincibilityTime);
        isInvincible = false;
        shield.SetActive(false);
    }

public IEnumerator Knockback(Vector3 direction, float duration, float speed)
{
    float elapsed = 0;

    while (elapsed < duration)
    {
        float t = elapsed / duration;
        // Aplicar la función de suavizado
        t = 1 - (1 - t) * (1 - t);

        playerController.Move(direction * speed * Time.deltaTime * t);
        elapsed += Time.deltaTime;
        yield return null;
    }
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