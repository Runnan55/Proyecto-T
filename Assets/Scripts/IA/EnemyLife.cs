using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class EnemyLife : MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth = 100;
    public AudioSource fxAudioSource; 

    public bool Azul = false;

    void Start()
    {
        currentHealth = maxHealth;
        fxAudioSource = GameManager.Instance.GetComponent<SoundManager>().fxAudioSource;
    }


    public void TakeDamage(int damage)
    {

        currentHealth -= damage;
    }
public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Bullet")
        {
            other.gameObject.SetActive(false);
            TakeDamage(100);
        }
    }
    
public void Update()
{
    
    if (currentHealth <= 0)
    {
        ActualizarPuntos();
        GameManager.Instance.GetComponent<SoundManager>().PlayFx(AudioFx.death, fxAudioSource, false);
        gameObject.SetActive(false);
    }
}
public void ActualizarPuntos()
{

    // Comprueba si el componente SimpleFish se encontró
        if (Azul)
    {
        // Si SimpleFish está presente, imprime "Holaxd"
        Debug.Log("+1 Azul");
         TMP_Text textComponent = GameObject.Find("Canvas/TextPezAzul").GetComponent<TMP_Text>();
            if(textComponent != null)
            {
                // Encuentra el objeto que tiene el script CharacterMovement
                CharacterMovement characterMovement = FindObjectOfType<CharacterMovement>();
                if (characterMovement != null)
                {
                    characterMovement.PuntosAzul += 1;
                    // Cambia el texto para mostrar el valor de PuntosAzul
                    textComponent.text = characterMovement.PuntosAzul.ToString();

                 }
            }
    }
            if (!Azul)
            {
        // Si SimpleFish está presente, imprime "Holaxd"
        Debug.Log("+1 Red");
                // Si SimpleFish está presente, imprime "Holaxd"
        TMP_Text textComponent = GameObject.Find("Canvas/TextPezRojo").GetComponent<TMP_Text>();
            if(textComponent != null)
            {
                // Encuentra el objeto que tiene el script CharacterMovement
                CharacterMovement characterMovement = FindObjectOfType<CharacterMovement>();
                if (characterMovement != null)
                {
                    characterMovement.PuntosRojo += 1;
                    // Cambia el texto para mostrar el valor de PuntosAzul
                    textComponent.text = characterMovement.PuntosRojo.ToString();
                }
            
            }
     }
     }
    }

