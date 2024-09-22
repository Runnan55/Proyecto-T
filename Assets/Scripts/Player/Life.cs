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
    public Canvas canvas;

    public TextMeshProUGUI timeText;
    public Image timeImage;
    public GameObject deathScreen; 
    public LoseManager LoseManager;

    public LevelManager levelManager;

    public float invincibilityTime = 1;
    public GameObject shield;


    public CharacterController playerController;  

    PlayerMovement playerMovement;

    public bool DamagePlayer = false;

    public SHAKECAMERA m_shakeCamera;

        IEnumerator FindHealthPanelReferences()
    {
        yield return new WaitForSeconds(0.1f); // Espera breve para permitir la inicialización de la UI

        // Encontrar el DefaultHUD(Clone) globalmente
        GameObject hud = GameObject.Find("DefaultHUD(Clone)");

        if (hud != null)
        {
            // Buscar HealthPanel dentro de CombatUI en DefaultHUD(Clone)
            Transform healthPanelTransform = hud.transform.Find("CombatUI/HealthPanel");

            if (healthPanelTransform != null)
            {
                // Asignar la referencia al TextMeshProUGUI timeText (TMP_Health)
                timeText = healthPanelTransform.Find("TMP_Health").GetComponent<TextMeshProUGUI>();

                // Asignar la referencia al componente Image timeImage (HealthFill)
                timeImage = healthPanelTransform.Find("HealthFill").GetComponent<Image>();

                // Verificación de las referencias
                if (timeText != null && timeImage != null)
                {
                    Debug.Log("timeText y timeImage encontrados correctamente.");
                }
                else
                {
                    Debug.LogError("No se encontraron timeText o timeImage.");
                }
            }
            else
            {
                Debug.LogError("No se pudo encontrar HealthPanel dentro de CombatUI.");
            }
        }
        else
        {
            Debug.LogError("No se pudo encontrar DefaultHUD(Clone). Asegúrate de que está en la escena.");
        }
    }

    void Start()
    {
        currentTime = maxTime;

        if (timeImage != null)
        {
            timeImage.fillAmount = 1;
        }
        else
        {
            Debug.Log("timeImage es null.");
        }

        levelManager = FindObjectOfType<LevelManager>();
        playerController = GetComponent<CharacterController>();
        LoseManager = FindObjectOfType<LoseManager>();

        if (playerController == null)
        {
            Debug.Log("No se encontró el componente CharacterController.");
        }

        StartCoroutine(FindHealthPanelReferences());
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

        //StartCoroutine(m_shakeCamera.shake());
        
     

        if (currentTime > maxTime)
        {
            currentTime = maxTime;        
 
        }
        else if (currentTime < 0)
        {            
            //levelManager.OnLevelFailed();
            deathScreen.gameObject.SetActive(true);
            LoseManager.Lose();
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
        if (timeText != null)
        {
            int minutes = Mathf.FloorToInt(currentTime / 60);
            int seconds = Mathf.FloorToInt(currentTime % 60);
            timeText.text = minutes + ":" + seconds.ToString("00");
        }
    }

    private void UpdateTimeImage()
    {
        if (timeImage != null)
        {
            timeImage.fillAmount = currentTime / maxTime;
        }
    }
}