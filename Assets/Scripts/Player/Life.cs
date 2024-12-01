using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Life : MonoBehaviour
{
    public float maxTime = 600;
    private float currentTime;
    public bool isInvincible = false;

    public float timeBank = 0;
    public TextMeshProUGUI timeBankText;

    public bool isAlive = true;
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

    [Header("Materials")]
    public Material invincibleMaterial;
    public Material damageMaterial;
    public Material originalMaterial;
    public SkinnedMeshRenderer skinnedMeshRenderer;

    [Header("Sounds")]
    [SerializeField] private FMODUnity.EventReference damage;

    public Effect effect;

    IEnumerator FindHealthPanelReferences()
    {
        yield return new WaitForSeconds(0.5f); // Espera breve para permitir la inicialización de la UI

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
                    //Debug.Log("timeText y timeImage encontrados correctamente.");
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

            // Buscar DeathPanel dentro de CombatUI en DefaultHUD(Clone)
            Transform deathPanelTransform = hud.transform.Find("DeathPanel");
            if (deathPanelTransform != null)
            {
                deathScreen = deathPanelTransform.gameObject;
            }
            else
            {
                Debug.LogError("No se pudo encontrar DeathPanel dentro de CombatUI.");
            }

            // Buscar TimeBankText dentro de CombatUI en DefaultHUD(Clone)
            Transform timeBankTextTransform = healthPanelTransform.Find("TimeBankText");

            if (timeBankTextTransform != null)
            {
                timeBankText = timeBankTextTransform.GetComponent<TextMeshProUGUI>();
            }
            else
            {
                Debug.LogError("No se pudo encontrar TimeBankText dentro de CombatUI.");
            }
        }
        else
        {
            Debug.LogError("No se pudo encontrar DefaultHUD(Clone). Asegúrate de que está en la escena.");
        }
    }

    void Start()
    {
        isInvincible = false;
        currentTime = maxTime;

        if (timeImage != null)
        {
            timeImage.fillAmount = 1;
        }
        else
        {
            //Debug.Log("timeImage es null.");
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
    }

    else if (timeBank > 0)
    {
        timeBank -= Time.deltaTime;

/*         if (timeBank < 0)
        {
            currentTime += timeBank; // Añadir el tiempo restante del banco
            timeBank = 0;
        } */
    }
    else
    {
        if (isAlive)
        {
            Death();
        }
    }

    // Asegurarse de que currentTime no sea negativo
    if (currentTime < 0)
    {
        currentTime = 0;
    }

    UpdateTimeText();
    UpdateTimeImage();
    UpdateTimeBankText(); // Actualizar el texto del banco del tiempo

    if (!isAlive && Input.GetKeyDown(KeyCode.F))
    {
        deathScreen.gameObject.SetActive(false);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // Depuración: Sumar 10 segundos al banco del tiempo al pulsar B
    if (Input.GetKeyDown(KeyCode.B))
    {
        timeBank += 6;
        UpdateTimeBankText();
    }
}

    public void Death()
    {
        currentTime = 0;
        UpdateTimeText();
        UpdateTimeImage();

        Debug.Log("muerte");
        isAlive = false;
        deathScreen.gameObject.SetActive(true);
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

        if (amount < 0)
        {
            ChangeMaterial(damageMaterial);
            StartCoroutine(InvincibilityFrames());
            effect.ScreenDamageEffect(Random.Range(1f, 1));
            FMODUnity.RuntimeManager.PlayOneShot(damage);
        }
    }

    IEnumerator InvincibilityFrames()
    {
        isInvincible = true;
        float elapsedTime = 0f;
        bool isDamageMaterial = true;

        while (elapsedTime < invincibilityTime)
        {
            ChangeMaterial(isDamageMaterial ? damageMaterial : originalMaterial);
            isDamageMaterial = !isDamageMaterial;
            elapsedTime += 0.1f; // velocidad parpadeo
            yield return new WaitForSeconds(0.1f);
        }

        isInvincible = false;
        ChangeMaterial(originalMaterial);
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

    private void UpdateTimeBankText()
    {
        if (timeBankText != null)
        {
            if (timeBank > 0)
            {
                timeBankText.gameObject.SetActive(true);
                int seconds = Mathf.FloorToInt(timeBank);
                if (timeBank < 5)
                {
                    int hundredths = Mathf.FloorToInt((timeBank - seconds) * 100);
                    timeBankText.text = "+" + seconds.ToString("00") + ":" + hundredths.ToString("00");
                }
                else
                {
                    timeBankText.text = "+" + seconds.ToString("00");
                }
            }
            else
            {
                timeBankText.gameObject.SetActive(false);
            }
        }
    }

    public void enableInvencibility()
    {
        isInvincible = true;
    }

    public void disableInvencibility()
    {
        isInvincible = false;
    }

    public void setInvincibleMat()
    {
        ChangeMaterial(invincibleMaterial);
    }
    
    public void setDamagedMat()
    {
        ChangeMaterial(damageMaterial);
    }

    public void setOriginalMat()
    {
        ChangeMaterial(originalMaterial);
    }

    public void fullHealth()
    {
        currentTime = maxTime;
        UpdateTimeText();
        UpdateTimeImage();
    }

    public void halfHealth()
    {
        currentTime = maxTime / 2;
        UpdateTimeText();
        UpdateTimeImage();
    }

    public void plus30Secs()
    {
        maxTime = maxTime + 30;
        UpdateTimeText();
        UpdateTimeImage();
    }

    public void minus30Secs()
    {
        if (maxTime > 30)
        {
            maxTime -= 30;
            if (currentTime > maxTime)
            {
                currentTime = maxTime;
            }
            UpdateTimeText();
            UpdateTimeImage();
        }
    }

    private void ChangeMaterial(Material newMaterial)
    {
        if (skinnedMeshRenderer != null && newMaterial != null)
        {
            skinnedMeshRenderer.material = new Material(newMaterial);
        }
    }

    // Método para ir a la siguiente sala
    public void GoToNextRoom(float newMaxTime)
    {
        // Ajustar la vida máxima y la vida actual
        maxTime = newMaxTime;
        currentTime = maxTime;

        // Guardar tiempo sobrante en el banco del tiempo
        if (currentTime > 0 && currentTime <= 10)
        {
            timeBank += currentTime;
        }

        // Limitar el banco del tiempo a un máximo de 10 segundos
        if (timeBank > 10)
        {
            timeBank = 10;
        }

        // Actualizar la UI
        UpdateTimeText();
        UpdateTimeImage();
        UpdateTimeBankText();
    }

    // Método para limpiar el banco del tiempo
    public void ClearTimeBank()
    {
        timeBank = 0;
        UpdateTimeBankText();
    }
}