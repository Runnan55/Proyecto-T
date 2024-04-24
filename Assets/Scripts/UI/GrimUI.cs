using UnityEngine;
using UnityEngine.UI;  // Asegúrate de incluir esto para usar el componente Image

public class GrimUI : MonoBehaviour
{
    public PlayerMovement playerMovement;
    public Image image;
    public Sprite Red;
    public Sprite Blue;

    // Start is called before the first frame update
    void Start()
    {
        image = GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        if (playerMovement.verificarArma)  // Suponiendo que verificarArma es una propiedad booleana
        {
            image.sprite = Red;
        }
        else
        {
            
            image.sprite = Blue;
        }
    }
}