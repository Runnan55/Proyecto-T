using UnityEngine;
using UnityEngine.SceneManagement;

public class HubTeleport : MonoBehaviour
{
    [Header("Visuales de la puerta")]
    public GameObject visualPuertaAbierta;
    public GameObject visualPuertaCerrada;

    [Header("Configuración de escena")]
    public string nombreEscenaDestino;

    [Header("Nivel requerido para abrir")]
    public int nivelRequerido = 1;

    private bool puertaActiva = false;

    void Start()
    {
        ActivarPuerta(false);
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        MovimientoJugador jugador = other.GetComponent<MovimientoJugador>();
        if (jugador == null)
        {
            Debug.LogWarning("El objeto con tag 'Player' no tiene MovimientoJugador.");
            return;
        }

        if (jugador.nivelActual >= nivelRequerido)
        {
            ActivarPuerta(true);

            if (!string.IsNullOrEmpty(nombreEscenaDestino))
            {
                SceneManager.LoadScene(nombreEscenaDestino);
            }
            else
            {
                Debug.LogWarning("No se ha asignado nombreEscenaDestino en " + gameObject.name);
            }
        }
        else
        {
            Debug.Log("El nivel del jugador es insuficiente para esta puerta.");
        }
    }

    void ActivarPuerta(bool abierta)
    {
        puertaActiva = abierta;
        if (visualPuertaAbierta != null) visualPuertaAbierta.SetActive(abierta);
        if (visualPuertaCerrada != null) visualPuertaCerrada.SetActive(!abierta);
    }
}
