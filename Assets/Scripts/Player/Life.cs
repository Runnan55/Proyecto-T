using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Life : MonoBehaviour
{
    public float maxTime = 600;
    public float currentTime;
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

    public Rigidbody playerController;  

    PlayerMovement playerMovement;

    public bool DamagePlayer = false;

    public SHAKECAMERA m_shakeCamera;

    [Header("Materials")]
    public Material invincibleMaterial;
    public Material damageMaterial;
    public Material originalMaterial;
    public MeshRenderer meshRenderer; // Cambiado de SkinnedMeshRenderer a MeshRenderer

    [Header("Sounds")]
    [SerializeField] private FMODUnity.EventReference damage;

    public Effect effect;

    private Coroutine timerCoroutine; // Añadido

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

        levelManager = FindObjectOfType<LevelManager>();
        playerController = GetComponent<Rigidbody>();
        LoseManager = FindObjectOfType<LoseManager>();

        if (playerController == null)
        {
            Debug.Log("No se encontró el componente CharacterController.");
        }

        StartCoroutine(FindHealthPanelReferences());
    }

    public void StartTimer(float initialTime)
    {
        currentTime = initialTime;
        UpdateTimeText();
        UpdateTimeImage();
        UpdateTimeBankText();

        if (timerCoroutine != null)
        {
            StopCoroutine(timerCoroutine); // Detener la corrutina existente
        }
        timerCoroutine = StartCoroutine(TimerCoroutine()); // Iniciar una nueva corrutina
    }

    void Update()
    {
        if (!isAlive && Input.GetKeyDown(KeyCode.F))
        {
            
        if (deathScreen != null)
        {
            Debug.Log("Desactivando DeathScreen");
            deathScreen.gameObject.SetActive(false);
        }
        
        else
        {
            Debug.LogError("deathScreen no está asignado.");
        }
            Time.timeScale = 1f; // Restablecer el time scale antes de recargar la escena
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            Debug.Log("f");
        }

        if (Input.GetKeyDown(KeyCode.B))
        {
            AddToTimeBank(6);
        }

        if (currentTime <= 0 && timeBank <= 0 && isAlive)
        {
            Death();
        }
            
        UpdateTimeText();
        UpdateTimeImage();
        UpdateTimeBankText();
    }

    public void StopTimer()
    {
        if (timerCoroutine != null)
        {
            StopCoroutine(timerCoroutine);
            timerCoroutine = null;
        }
    }

    IEnumerator TimerCoroutine()
    {
        while (currentTime > 0 || timeBank > 0)
        {
            if (currentTime > 0)
            {
                currentTime -= Time.deltaTime;
            }
            else if (timeBank > 0)
            {
                timeBank -= Time.deltaTime;
            }
            else
            {
                if (isAlive)
                {
                    Death();
                }
            }

            if (currentTime < 0)
            {
                currentTime = 0;
            }

            UpdateTimeText();
            UpdateTimeImage();
            UpdateTimeBankText();

            yield return null;
        }
    }

    public void Death()
    {
        currentTime = 0;
        UpdateTimeText();
        UpdateTimeImage();

        isAlive = false;
        Time.timeScale = 0f;
        Debug.Log("MORISIOn, is alive? " + isAlive);
        deathScreen.gameObject.SetActive(true);
    }

    public void ModifyTime(float amount)
    {
        DamagePlayer = true;

        if (isInvincible && amount < 0) return;

        if (amount < 0)
        {
            float damage = -amount;

            // Reducir primero currentTime
            if (currentTime > 0)
            {
                if (currentTime >= damage)
                {
                    currentTime -= damage;
                    damage = 0;
                }
                else
                {
                    damage -= currentTime;
                    currentTime = 0;
                }
                UpdateTimeText();
            }

            // Luego reducir timeBank
            if (damage > 0)
            {
                if (timeBank >= damage)
                {
                    timeBank -= damage;
                    damage = 0;
                }
                else
                {
                    damage -= timeBank;
                    timeBank = 0;
                }
                UpdateTimeBankText();
            }
        }
        else
        {
            currentTime += amount;
            if (currentTime > maxTime)
            {
                currentTime = maxTime;
            }
        }

        if (currentTime <= 0 && timeBank <= 0)
        {
            Death();
        }

        UpdateTimeImage();

        if (amount < 0 && (-amount) > 0)
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

            playerController.MovePosition(direction * speed * Time.deltaTime * t);
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

    public void UpdateTimeBankText()
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
        maxTime += 30;
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
        if (meshRenderer != null && newMaterial != null)
        {
            meshRenderer.material = new Material(newMaterial);
        }
    }

    public void GoToNextRoom(float newMaxTime)
    {
        maxTime = newMaxTime;
        currentTime = maxTime;

        if (currentTime > 0 && currentTime <= 10)
        {
            AddToTimeBank(currentTime);
        }

        UpdateTimeText();
        UpdateTimeImage();
        UpdateTimeBankText();
    }

    public void ClearTimeBank()
    {
        timeBank = 0;
        UpdateTimeBankText();
    }

    public void AddToTimeBank(float time)
    {
        // Limitar la cantidad agregada por conjunto de oleadas a un máximo de 10 segundos
        time = Mathf.Min(time, 10);
        timeBank += time;
        UpdateTimeBankText();
    }

    public void ClearTime()
    {
        currentTime = 0;
        UpdateTimeText();
        UpdateTimeImage();
    }
}