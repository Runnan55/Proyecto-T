using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI; 

public class TarotManager : MonoBehaviour
{
    public LevelManager levelManager;
    public GameObject tarotCardUI;
    public TextMeshProUGUI arcanaNameText;
    public Image cardImage; // La imagen de la carta

    public GameObject centralObject; // El objeto alrededor del cual los otros girarán
    public GameObject[] rotatingObjects; // Los objetos que girarán
    public float moveDuration = 5f; // La duración del movimiento en segundos
    public float moveUpDistance = 5f; // La distancia que la primera carta se moverá hacia arriba
    public float moveUpDuration = 2f; // La duración del movimiento hacia arriba en segundos

    public ArcanaImage[] arcanaImages; // Las imágenes de las arcanas

    private Dictionary<string, Sprite> arcanaImageDict; // El diccionario que asocia los nombres de las arcanas con las imágenes

    public GameObject player;
    public PlayerMovement pm;

    [System.Serializable]
    public class ArcanaImage
    {
        public string name; // El nombre de la arcana
        public Sprite image; // La imagen de la arcana
    }

    void Awake()
    {
        pm = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();
    }

    void Start()
    {
        // Inicializa el diccionario
        arcanaImageDict = new Dictionary<string, Sprite>();
        foreach (ArcanaImage arcanaImage in arcanaImages)
        {
            arcanaImageDict.Add(arcanaImage.name, arcanaImage.image);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        StartTarot();
        //player = GameObject.FindGameObjectWithTag("Player");
        pm.speed = 0;
        pm.animator.Play("Idle");
        pm.enabled = false;
    }

    void StartTarot()
    {
        StartCoroutine(PlayAnimationAndLoadLevel());
    }

    public IEnumerator PlayAnimationAndLoadLevel()
    {
        levelManager.SelectRandomCard();

        // Activa las cartas justo antes de iniciar la animación
        foreach (GameObject obj in rotatingObjects)
        {
            obj.SetActive(true);
        }
        
        // Inicia el movimiento
        float elapsedTime = 0f;
        while (elapsedTime < moveDuration)
        {
            foreach (GameObject obj in rotatingObjects)
            {
                // Calcula la nueva posición del objeto
                Vector3 newPosition = Vector3.Lerp(obj.transform.position, centralObject.transform.position, elapsedTime / moveDuration);

                // Actualiza la posición del objeto
                obj.transform.position = newPosition;
            }

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Desactiva todas las cartas excepto la primera
        for (int i = 1; i < rotatingObjects.Length; i++)
        {
            rotatingObjects[i].SetActive(false);
        }

        // Cambia la imagen de la carta
        if (arcanaImageDict.ContainsKey(levelManager.selectedCard))
        {
            cardImage.sprite = arcanaImageDict[levelManager.selectedCard];
        }

        // Mueve la primera carta hacia arriba
        Vector3 startPosition = rotatingObjects[0].transform.position;
        Vector3 endPosition = startPosition + new Vector3(0, moveUpDistance, 0);
        elapsedTime = 0f; // Reinicia el tiempo
        while (elapsedTime < moveUpDuration)
        {
            rotatingObjects[0].transform.position = Vector3.Lerp(startPosition, endPosition, elapsedTime / moveUpDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Muestra el nombre de la arcana seleccionada
        arcanaNameText.gameObject.SetActive(true);
        arcanaNameText.text = levelManager.selectedCard;
        tarotCardUI.SetActive(true);

        // Espera 3 segundos
        yield return new WaitForSeconds(3f);

        levelManager.LoadLevel();
    }
}