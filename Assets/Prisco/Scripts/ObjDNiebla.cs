using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FischlWorks_FogWar; // Importar el espacio de nombres donde se encuentra csFogWar

public class ObjDNiebla : MonoBehaviour
{
    [Header("Referencia a FogWar")]
    public csFogWar fogWar; // Referencia al sistema de niebla

    [Header("Configuración de rango de visión")]
    public int sightRange = 5; // Rango de visión configurable
    public int maxSightRange = 10; // Rango máximo en modo expansivo
    public int incrementoPorGolpe = 1; // Incremento del rango por golpe

    [Header("Modo expansivo")]
    public bool usarModoExpansivo = false; // Indica si el objeto está en modo expansivo
    
    [Header("Rango máximo permanente")]
    public bool usarRangoMaximo = false; // Si está en true, usa el rango máximo permanentemente

    [Header("Reducción de rango")]
    public int rangoInicial = 5; // Rango inicial de visión
    public float tiempoSinGolpe = 2f; // Tiempo editable antes de reducir el rango
    private float tiempoDesdeUltimoGolpe = 0f; // Temporizador para rastrear el tiempo sin golpes

    public int fogRevealerIndex = -1; // Índice del FogRevealer en la lista de fogRevealers
    private bool isActive = false; // Indica si el objeto ya está activado
    private bool rangoIncrementado = false; // Bandera para evitar incrementos duplicados

    private void Start()
    {
        // Buscar automáticamente el objeto FogWar en la escena si no está asignado
        if (fogWar == null)
        {
            fogWar = FindObjectOfType<csFogWar>();
            if (fogWar == null)
            {
                Debug.LogError("No se encontró un objeto con el script csFogWar en la escena.");
            }
        }

        // Si usarRangoMaximo está activado, activar inmediatamente el FogRevealer
        if (usarRangoMaximo && fogWar != null)
        {
            sightRange = maxSightRange;
            ActivarFogRevealer();
        }
    }

    private void Update()
    {
        // Si usarRangoMaximo está activado, usar siempre el rango máximo
        if (usarRangoMaximo)
        {
            if (sightRange != maxSightRange)
            {
                sightRange = maxSightRange;
                ActualizarFogRevealer();
            }
            return; // Salir del Update para evitar la lógica de reducción
        }

        // Si el rango actual es mayor que 0, reducir gradualmente
        if (usarModoExpansivo && sightRange > 0)
        {
            tiempoDesdeUltimoGolpe += Time.deltaTime;

            if (tiempoDesdeUltimoGolpe >= tiempoSinGolpe)
            {
                sightRange = Mathf.Max(sightRange - 1, 0); // Reducir el rango gradualmente
                tiempoDesdeUltimoGolpe = 0f; // Reiniciar el temporizador

                // Actualizar o eliminar el FogRevealer si ya está registrado
                if (sightRange == 0)
                {
                    EliminarFogRevealer();
                }
                else
                {
                    ActualizarFogRevealer();
                }
            }
        }

        // Asegurarse de que los cambios manuales en sightRange se reflejen en el FogRevealer
        if (fogRevealerIndex != -1 && fogWar._FogRevealers[fogRevealerIndex]._SightRange != sightRange)
        {
            ActualizarFogRevealer();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (EsAtaqueDelJugador(other.gameObject) && fogWar != null)
        {
            // Si no está en la lista, volver a agregarlo
            if (fogRevealerIndex == -1)
            {
                ActivarFogRevealer();
            }

            // Si usarRangoMaximo está activado, establecer el rango máximo inmediatamente
            if (usarRangoMaximo)
            {
                sightRange = maxSightRange;
                ActualizarFogRevealer();
            }
            // Incrementar el rango de visión si está en modo expansivo
            else if (usarModoExpansivo && !rangoIncrementado)
            {
                IncrementarRango();
                rangoIncrementado = true; // Marcar como incrementado
            }

            tiempoDesdeUltimoGolpe = 0f; // Reiniciar el temporizador al recibir un golpe
        }
    }

    private void ActivarFogRevealer()
    {
        if (fogWar != null)
        {
            // Registrar este objeto como un FogRevealer si no está registrado
            if (fogRevealerIndex == -1)
            {
                fogRevealerIndex = fogWar.AddFogRevealer(new csFogWar.FogRevealer(transform, sightRange, false));
            }
            isActive = true; // Marcar el objeto como activado
        }
    }

    private void EliminarFogRevealer()
    {
        if (fogRevealerIndex != -1 && fogWar != null)
        {
            fogWar.RemoveFogRevealer(fogRevealerIndex);
            fogRevealerIndex = -1; // Marcar como eliminado
            isActive = false; // Marcar el objeto como desactivado
        }
    }

    private void IncrementarRango()
    {
        // Incrementar el rango de visión hasta el máximo permitido
        sightRange = Mathf.Min(sightRange + incrementoPorGolpe, maxSightRange);

        // Actualizar el rango en el FogRevealer si ya está registrado
        ActualizarFogRevealer();

        // Reiniciar la bandera después de un breve retraso
        StartCoroutine(ResetRangoIncrementado());
    }

    private void ActualizarFogRevealer()
    {
        if (fogRevealerIndex != -1)
        {
            fogWar._FogRevealers[fogRevealerIndex] = new csFogWar.FogRevealer(transform, sightRange, false);
            fogWar.ForceUpdateFog(); // Forzar la actualización de la niebla
        }
    }

    private IEnumerator ResetRangoIncrementado()
    {
        yield return new WaitForSeconds(0.1f); // Ajusta el tiempo según sea necesario
        rangoIncrementado = false;
    }

    private bool EsAtaqueDelJugador(GameObject objeto)
    {
        return objeto.name == "AtaqueL1" ||
               objeto.name == "AtaqueL2" ||
               objeto.name == "AtaqueL3" ||
               objeto.name == "CollidersAtaqueP" ||
               objeto.CompareTag("Boomerang");
    }
}
