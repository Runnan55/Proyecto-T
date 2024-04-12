using UnityEngine;
using TMPro; // Para trabajar con TextMeshPro
using UnityEngine.UI; // Para trabajar con componentes UI Image

public class InventarioPlayer : MonoBehaviour 
{
    public static InventarioPlayer Instance { get; private set; }

    public TMP_Text slot1;
    public TMP_Text slot2;
    public TMP_Text slot3;

    public Image image1; // Referencia a la Image del slot 1
    public Image image2; // Referencia a la Image del slot 2
    public Image image3; // Referencia a la Image del slot 3

    public Sprite emptySprite; // Sprite para slots vacíos
    public Sprite tpCardSprite; // Sprite para TpCard

    public Sprite HoleSprite;
    // Añade aquí más sprites según sea necesario para otras cartas

    public string[] cards = new string[3];
    public int activeSlot = 1; // El slot del medio es el activo por defecto

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // Inicializa todos los slots con "Empty"
        cards[0] = "Empty";
        cards[1] = "Empty";
        cards[2] = "Empty";
        UpdateInventoryDisplay();
    }

    void Update()
    {
if (Input.GetKeyDown(KeyCode.Q) || Input.GetAxis("Mouse ScrollWheel") < 0)
{
    RotateLeft();
}
if (Input.GetKeyDown(KeyCode.E) || Input.GetAxis("Mouse ScrollWheel") > 0)
{
    RotateRight();
}
        else if (Input.GetKeyDown(KeyCode.X)) // Intenta activar carta con X
        {
            UseCard();
        }
    }

    void RotateLeft()
    {
        string temp = cards[0];
        cards[0] = cards[1];
        cards[1] = cards[2];
        cards[2] = temp;
        UpdateInventoryDisplay();
    }

    void RotateRight()
    {
        string temp = cards[2];
        cards[2] = cards[1];
        cards[1] = cards[0];
        cards[0] = temp;
        UpdateInventoryDisplay();
    }

    public void UseCard()
    {
        // Verifica si el slot activo es distinto de "Empty" antes de usar la carta
        if (cards[activeSlot] != "Empty")
        {
            Debug.Log("Carta Activada: " + cards[activeSlot]);
            cards[activeSlot] = "Empty"; // Cambia el nombre de la carta a "Empty"
            UpdateInventoryDisplay();
        }
        else
        {
            Debug.Log("El slot activo está vacío.");
        }
    }

    public string GetCurrentCardName()
    {
        return cards[activeSlot];
    }

    public void UpdateInventoryDisplay()
    {
        slot1.text = cards[0];
        slot2.text = cards[1];
        slot3.text = cards[2];

        // Actualizar los sprites basado en el contenido de cada slot
        UpdateSlotImage(image1, cards[0]);
        UpdateSlotImage(image2, cards[1]);
        UpdateSlotImage(image3, cards[2]);
    }

    void UpdateSlotImage(Image slotImage, string cardName)
    {
        switch (cardName)
        {
            case "TpCard":
                slotImage.sprite = tpCardSprite;
                break;
            // Añade aquí más casos según los nombres de tus cartas y sus sprites correspondientes
            case "Empty":
                slotImage.sprite = emptySprite;
                break;
            case "CartaHole":
                slotImage.sprite = HoleSprite;
                break;
            default:
                slotImage.sprite = emptySprite; // Usa el sprite vacío por defecto para cualquier otro caso
                break;
        }
    }
}
