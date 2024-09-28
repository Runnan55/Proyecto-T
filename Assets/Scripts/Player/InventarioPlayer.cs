using UnityEngine;
using TMPro; // Para trabajar con TextMeshPro
using UnityEngine.UI; // Para trabajar con componentes UI Image
using System.Collections;


public class InventarioPlayer : MonoBehaviour 
{
    public static InventarioPlayer Instance { get; private set; }

    public TMP_Text slot1;
    public TMP_Text slot2;
    public TMP_Text slot3;

    public Image image1; // Referencia a la Image del slot 1
    public Image image2; // Referencia a la Image del slot 2
    public Image image3; // Referencia a la Image del slot 3
    
    public Color GrayColor = Color.gray;

    public Sprite emptySprite; // Sprite para slots vacíos
    public Sprite tpCardSprite; // Sprite para TpCard

    public Sprite HoleSprite;
    public Sprite FireCard;
    public Sprite FastCard;
    public Sprite SwapCard;
    // Añade aquí más sprites según sea necesario para otras cartas
    public bool mid;
    public string[] cards = new string[3];
    public int activeSlot = 1; // El slot del medio es el activo por defecto

    public string mode;
    private ItemPlacer itemPlacer;

    IEnumerator InitializeReferences()
    {
        yield return new WaitForSeconds(0.1f); // Espera 0.1 segundos para asegurarse de que DefaultHUD(Clone) se ha instanciado
    GameObject hud = GameObject.Find("DefaultHUD(Clone)");

    if (hud != null)
    {
        // Buscar CardsPanel dentro de la jerarquía de DefaultHUD(Clone)
        Transform cardsPanelTransform = hud.transform.Find("CombatUI/CardsPanel");

        if (cardsPanelTransform != null)
        {
            slot1 = cardsPanelTransform.Find("CardTxt_Left").GetComponent<TMP_Text>();
            slot2 = cardsPanelTransform.Find("CardTxt_Main").GetComponent<TMP_Text>();
            slot3 = cardsPanelTransform.Find("CardTxt_Right").GetComponent<TMP_Text>();

            image1 = cardsPanelTransform.Find("izq").GetComponent<Image>();
            image2 = cardsPanelTransform.Find("medio").GetComponent<Image>();
            image3 = cardsPanelTransform.Find("der").GetComponent<Image>();
        }
        else
        {
            // Depuración adicional para mostrar la jerarquía si no se encuentra el CardsPanel
            Debug.LogError("No se pudo encontrar el CardsPanel dentro de DefaultHUD(Clone). Asegúrate de que los nombres de la jerarquía son correctos.");
            PrintChildren(hud.transform);  // Imprime la jerarquía de DefaultHUD(Clone)
        }
    }
    else
    {
        Debug.LogError("No se pudo encontrar DefaultHUD(Clone). Asegúrate de que está en la escena.");
    }
    UpdateInventoryDisplay();
    }
    
void PrintChildren(Transform parent)
{
    foreach (Transform child in parent)
    {
        Debug.Log("Child: " + child.name);
        PrintChildren(child);  // Llamada recursiva para imprimir sub-hijos
    }
}
    
    void Awake()
    {
        itemPlacer = GetComponent<ItemPlacer>();
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
        StartCoroutine(InitializeReferences());
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q) || Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            if (!itemPlacer.CartaColocada)
            {
                RotateLeft();
            }
        }
        if (Input.GetKeyDown(KeyCode.E) || Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            if (!itemPlacer.CartaColocada)
            {
                RotateRight();
            }
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
        itemPlacer.ClearPreview();
    }

    void RotateRight()
    {
        string temp = cards[2];
        cards[2] = cards[1];
        cards[1] = cards[0];
        cards[0] = temp;
        UpdateInventoryDisplay();
        itemPlacer.ClearPreview();
    }

    public void UseCard()
    {
        // Verifica si el slot activo es distinto de "Empty" antes de usar la carta
        if (cards[activeSlot] != "Empty")
        {
            // Debug.Log("Carta Activada: " + cards[activeSlot]);
            cards[activeSlot] = "Empty"; // Cambia el nombre de la carta a "Empty"
            if (mode == "place" && itemPlacer.CartaColocada == true)
            {
                image2.color = GrayColor;
            }
            else
            {
                
                    if (cards[1] == "Empty")
                {
                    // Prioriza la carta en el slot 1 si no está vacía
                    if (cards[0] != "Empty")
                    {
                        cards[1] = cards[0]; // Mueve la carta del slot 1 al slot 2
                        cards[0] = "Empty"; // Limpia el slot 1
                    }
                    // Si el slot 1 está vacío, revisa si el slot 3 tiene una carta
                    else if (cards[2] != "Empty")
                    {
                        cards[1] = cards[2]; // Mueve la carta del slot 3 al slot 2
                        cards[2] = "Empty"; // Limpia el slot 3
                    }
                }
                UpdateInventoryDisplay();
            }
        }
        else
        {
            // Debug.Log("El slot activo está vacío.");
        }
    }

    public string GetCurrentCardName()
    {
        return cards[activeSlot];
    }

    public void UpdateInventoryDisplay()
    {
        if (slot1 && slot2 && slot3)
        {
            slot1.text = cards[0];
            slot2.text = cards[1];
            slot3.text = cards[2];

            // Actualizar los sprites basado en el contenido de cada slot
            UpdateSlotImage(image1, cards[0]);
            mid = true;
            UpdateSlotImage(image2, cards[1]);
            mid = false;
            UpdateSlotImage(image3, cards[2]);
        }
    }

    void UpdateSlotImage(Image slotImage, string cardName)
    {
        switch (cardName)
        {
            case "TpCard":
                slotImage.sprite = tpCardSprite;
                if (mid)
                {
                    mode = "place";
                }
                break;
            // Añade aquí más casos según los nombres de tus cartas y sus sprites correspondientes
            case "Empty":
                slotImage.sprite = emptySprite;
                if (mid)
                {
                    mode = "null";
                }
                break;
            case "CartaHole":
                slotImage.sprite = HoleSprite;
                if (mid)
                {
                    mode = "place";
                }
                break;
            case "FireCard":
                slotImage.sprite = FireCard;
                if (mid)
                {
                    mode = "activable";
                }
                break;
            case "SwapCard":
                slotImage.sprite = SwapCard;
                if (mid)
                {
                    mode = "activable";
                }
                break;
            case "FastCard":
                slotImage.sprite = FastCard;
                if (mid)
                {
                    mode = "self";
                }
                break;
            default:
                slotImage.sprite = emptySprite; // Usa el sprite vacío por defecto para cualquier otro caso
                if (mid)
                {
                    mode = "null";
                }
                break;
        }
    }

    public static string GetCardAtIndex(int index)
    {
        if (Instance != null && index >= 0 && index < Instance.cards.Length)
        {
            return Instance.cards[index];
        }
        else
        {
            return null;
        }
    }
    public void ClearInventory()
    {
        cards[0] = "Empty";
        cards[1] = "Empty";
        cards[2] = "Empty";
        UpdateInventoryDisplay();
    }
}