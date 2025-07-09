using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class HubTeleport : MonoBehaviour
{
    [Header("Visuales de la puerta")]
    public GameObject visualPuertaAbierta;
    public GameObject visualPuertaCerrada;

    [Header("Configuración de escena")]
    public string nombreEscenaDestino;

    [Header("Nivel requerido para abrir")]
    public int nivelRequerido = 1;

    private bool puedeEntrar = false;

    void Start()
    {
        StartCoroutine(ComprobarNivelConDelay());
    }

    IEnumerator ComprobarNivelConDelay()
    {
        yield return new WaitForSeconds(0.5f);

        GameObject jugadorObj = GameObject.FindWithTag("Player");
        if (jugadorObj != null)
        {
            MovimientoJugador jugador = jugadorObj.GetComponent<MovimientoJugador>();
            if (jugador != null)
            {
                bool abierta = jugador.nivelActual >= nivelRequerido;
                puedeEntrar = abierta;
                MostrarEstadoPuerta(abierta);
            }
            else
            {
                Debug.LogWarning("El jugador no tiene el componente MovimientoJugador.");
            }
        }
        else
        {
            Debug.LogWarning("No se ha encontrado ningún objeto con tag 'Player'.");
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        MovimientoJugador jugador = other.GetComponent<MovimientoJugador>();
        if (jugador == null) return;

        if (jugador.nivelActual >= nivelRequerido)
        {
            SceneManager.LoadScene(nombreEscenaDestino);
        }
        else
        {
            Debug.Log("Nivel insuficiente para entrar en " + gameObject.name);
        }
    }

    void MostrarEstadoPuerta(bool abierta)
    {
        if (visualPuertaAbierta != null) visualPuertaAbierta.SetActive(abierta);
        if (visualPuertaCerrada != null) visualPuertaCerrada.SetActive(!abierta);
    }
}
