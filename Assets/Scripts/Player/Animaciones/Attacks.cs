using System.Collections;
using UnityEngine;
using UnityEngine.VFX;

public class Attacks : StateMachineBehaviour
{
   
    public string attackNumber;
     private Collider ataqueL1Collider;   
     private Collider ataqueL2Collider; 
     private Collider ataqueL3Collider;
     private GameObject Cubo1;
     private GameObject Cubo2;
     private GameObject Cubo3; 

    [SerializeField] private FMODUnity.EventReference attack1;
    [SerializeField] private FMODUnity.EventReference attack2;
    [SerializeField] private FMODUnity.EventReference attack3;


    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Cancelar el dash si se inicia una nueva animación
        if (MovimientoJugador.isDashing)
        {
            MovimientoJugador.isDashing = false;
            return; // No ejecutar ataques durante el dash
        }

        MovimientoJugador playerMovement = MovimientoJugador.instance;
        GameObject ataqueL1 = GameObject.Find("AtaqueL1");
        GameObject ataqueL2 = GameObject.Find("AtaqueL2");
        GameObject ataqueL3 = GameObject.Find("AtaqueL3");
        GameObject Cubo1 = GameObject.Find("Cubo1");
        GameObject Cubo2 = GameObject.Find("Cubo2");
        GameObject Cubo3 = GameObject.Find("Cubo3");
           
        switch (attackNumber)
        {
            case "Attack1":     
                // Prevenir ejecución durante ejecución Y recuperación del ataque pesado
                if (MovimientoJugador.ataqueP)
                {
                    return;
                }                   
                MovimientoJugador.ataqueL = false;
                if (ataqueL1 != null)
                {
                    ataqueL1Collider = ataqueL1.GetComponent<Collider>();
                    if (ataqueL1Collider != null)
                    {
                        ataqueL1Collider.enabled = true; 
                    }
                    // Activar el DamageDealer del ataque 1
                    DamageDealer damageDealer1 = ataqueL1.GetComponent<DamageDealer>();
                    if (damageDealer1 != null)
                    {
                        damageDealer1.gameObject.SetActive(true);
                    }
                    FMODUnity.RuntimeManager.PlayOneShot(attack1);
                }    
                if (Cubo1 != null)
                {
                    MeshRenderer cubo1Renderer = Cubo1.GetComponent<MeshRenderer>();
                    if (cubo1Renderer != null)
                    {
                        cubo1Renderer.enabled = true; 
                    }
                }
                animator.GetComponent<MonoBehaviour>().StartCoroutine(ReducirVelocidadConLerp(animator, stateInfo.length));
                break;                
            case "Attack2":  
                // Prevenir ejecución durante ejecución Y recuperación del ataque pesado
                if (MovimientoJugador.ataqueP)
                {
                    return;
                }
                MovimientoJugador.enterAttack = true; 
                MovimientoJugador.ataqueL = false;
                MovimientoJugador.speed = 0;
                

                if (ataqueL2 != null)
                {
                    ataqueL2Collider = ataqueL2.GetComponent<Collider>();
                    if (ataqueL2Collider != null)
                    {
                        ataqueL2Collider.enabled = true; 
                    }
                    // Activar el DamageDealer del ataque 2
                    DamageDealer damageDealer2 = ataqueL2.GetComponent<DamageDealer>();
                    if (damageDealer2 != null)
                    {
                        damageDealer2.gameObject.SetActive(true);
                    }
                    FMODUnity.RuntimeManager.PlayOneShot(attack2);
                }  

                if (Cubo2 != null)
                {
                    MeshRenderer cubo2Renderer = Cubo2.GetComponent<MeshRenderer>();
                    if (cubo2Renderer != null)
                    {
                        cubo2Renderer.enabled = true; 
                    }
                }
                
                if (playerMovement.EstaMoviendose())
                {
                    Vector3 direccionEmpuje = playerMovement.ObtenerDireccionEmpuje();
                    animator.GetComponent<MonoBehaviour>().StartCoroutine(playerMovement.EmpujarJugadorAL2(direccionEmpuje, 0.1f)); // Empuje durante 0.1 segundos
                }

                // Desactivar el movimiento del jugador
                playerMovement.canMove = false;
                break;
            case "Attack3":
                // Prevenir ejecución durante ejecución Y recuperación del ataque pesado
                if (MovimientoJugador.ataqueP)
                {
                    return;
                }
                MovimientoJugador.enterAttack = true; 
                MovimientoJugador.ataqueL = false;
                MovimientoJugador.speed = 0; 
                if (ataqueL3 != null)
                {
                    ataqueL3Collider = ataqueL3.GetComponent<Collider>();
                    if (ataqueL3Collider != null)
                    {
                        ataqueL3Collider.enabled = true; 
                    }
                    // Activar el DamageDealer del ataque 3
                    DamageDealer damageDealer3 = ataqueL3.GetComponent<DamageDealer>();
                    if (damageDealer3 != null)
                    {
                        damageDealer3.gameObject.SetActive(true);
                    }
                    FMODUnity.RuntimeManager.PlayOneShot(attack3);
                }  

                if (Cubo3 != null)
                {
                    MeshRenderer cubo3Renderer = Cubo3.GetComponent<MeshRenderer>();
                    if (cubo3Renderer != null)
                    {
                        cubo3Renderer.enabled = true; 
                    }
                } 
                    
                animator.GetComponent<MonoBehaviour>().StartCoroutine(playerMovement.EmpujarJugadorAL3(0.1f)); // Empuje durante 0.1 segundos
                animator.GetComponent<MonoBehaviour>().StartCoroutine(DelayAfterThirdAttack());
                break;
            case "AttackP": 
                MovimientoJugador.speed = 0; 
                if (playerMovement.EstaMoviendose())
                {
                    Vector3 direccionEmpuje = playerMovement.ObtenerDireccionEmpuje();
                    animator.GetComponent<MonoBehaviour>().StartCoroutine(playerMovement.EmpujarJugadorAL2(direccionEmpuje, 0.05f)); // Empuje durante 0.5 segundos
                } 
                MovimientoJugador.ataqueP = false;  
                animator.SetBool("Dash", false);       
                break;
            case "AttackD": 
                MovimientoJugador.ataqueD = true;
                MovimientoJugador.speed = 0;          
                break;
            case "Attack3P":         
                break;
        }
    } 

    private IEnumerator ReducirVelocidadConLerp(Animator animator, float duracion)
    {
        float velocidadInicial = MovimientoJugador.speed;
        float tiempoTranscurrido = 0f;
        Vector3 ultimaDireccionMovimiento = MovimientoJugador.instance.ObtenerDireccionMovimiento(); // Obtener la última dirección de movimiento basada en los inputs

        while (tiempoTranscurrido < duracion)
        {
            if (MovimientoJugador.isDashing) // Cancelar el movimiento si se realiza un dash
            {
                yield break;
            }

            MovimientoJugador.speed = Mathf.Lerp(velocidadInicial, 0, tiempoTranscurrido / duracion);
            MovimientoJugador.instance.transform.Translate(ultimaDireccionMovimiento * MovimientoJugador.speed * Time.deltaTime, Space.World); // Mover al jugador en la última dirección de movimiento
            tiempoTranscurrido += Time.deltaTime;
            yield return null;
        }

        MovimientoJugador.speed = 0;
        yield return new WaitForSeconds(0.1f); // Asegurarse de que la velocidad se mantenga en 0 por un breve momento
        MovimientoJugador.speed = velocidadInicial; // Restablecer la velocidad al valor inicial
    }

    private IEnumerator DelayAfterThirdAttack()
    {
        MovimientoJugador.canAttack = false;
        yield return new WaitForSeconds(0.8f); // Reducido de 1.3f a 0.8f
        MovimientoJugador.canAttack = true;
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (MovimientoJugador.isDashing)
        {
            animator.Play("Dash");
        }        
        else if (MovimientoJugador.isDashing) 
        {
            animator.Play("Dash"); 
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        PlayerMovement.hasAttacked = false;
        MovimientoJugador.enterAttack = false; // Permitir rotación nuevamente
        // Solo resetear hasRotated si realmente terminamos todos los ataques (no en transiciones de combo)
        GameObject ataqueL1 = GameObject.Find("AtaqueL1");
        GameObject ataqueL2 = GameObject.Find("AtaqueL2");
        GameObject ataqueL3 = GameObject.Find("AtaqueL3");
        GameObject Cubo1 = GameObject.Find("Cubo1");
        GameObject Cubo2 = GameObject.Find("Cubo2");
        GameObject Cubo3 = GameObject.Find("Cubo3");

        MovimientoJugador.speed = 15; // Restablecer la velocidad al valor original
        MovimientoJugador.instance.canMove = true; // Reactivar el movimiento del jugador

        switch (attackNumber)
        {
            case "Attack1":
                // NO resetear hasRotated aquí para permitir combos
                if (ataqueL1 != null)
                {
                    ataqueL1Collider = ataqueL1.GetComponent<Collider>();
                    if (ataqueL1Collider != null)
                    {
                        ataqueL1Collider.enabled = false; 
                    }
                    // Resetear el DamageDealer del ataque 1
                    DamageDealer damageDealer1 = ataqueL1.GetComponent<DamageDealer>();
                    if (damageDealer1 != null)
                    {
                        damageDealer1.ResetDamage();
                    }
                }   
                if (Cubo1 != null)
                {
                    MeshRenderer cubo1Renderer = Cubo1.GetComponent<MeshRenderer>();
                    if (cubo1Renderer != null)
                    {
                        cubo1Renderer.enabled = false; // Desactivar el MeshRenderer
                    }
                }
                break;              
            case "Attack2":
                MovimientoJugador.speed = 15; 
                // NO resetear hasRotated aquí para permitir combos
                if (ataqueL2 != null)
                {
                    ataqueL2Collider = ataqueL2.GetComponent<Collider>();
                    if (ataqueL2Collider != null)
                    {
                        ataqueL2Collider.enabled = false; 
                    }
                    // Resetear el DamageDealer del ataque 2
                    DamageDealer damageDealer2 = ataqueL2.GetComponent<DamageDealer>();
                    if (damageDealer2 != null)
                    {
                        damageDealer2.ResetDamage();
                    }
                }   
                if (Cubo2 != null)
                {
                    MeshRenderer cubo2Renderer = Cubo2.GetComponent<MeshRenderer>();
                    if (cubo2Renderer != null)
                    {
                        cubo2Renderer.enabled = false; // Activar el MeshRenderer
                    }
                }
                break;
            case "Attack3":
                MovimientoJugador.speed = 15; 
                MovimientoJugador.hasRotated = false; // Resetear para permitir nueva rotación
                if (ataqueL3 != null)
                {
                    ataqueL3Collider = ataqueL3.GetComponent<Collider>();
                    if (ataqueL3Collider != null)
                    {
                        ataqueL3Collider.enabled = false; 
                    }
                    // Resetear el DamageDealer del ataque 3
                    DamageDealer damageDealer3 = ataqueL3.GetComponent<DamageDealer>();
                    if (damageDealer3 != null)
                    {
                        damageDealer3.ResetDamage();
                    }
                }   
                if (Cubo3 != null)
                {
                    MeshRenderer cubo3Renderer = Cubo3.GetComponent<MeshRenderer>();
                    if (cubo3Renderer != null)
                    {
                        cubo3Renderer.enabled = false; // Activar el MeshRenderer
                    }
                }  
                break;
            case "AttackP":
                MovimientoJugador.speed = 15; 
                MovimientoJugador.hasRotated = false;
                MovimientoJugador.ataqueP = false;    
                break;
            case "AttackD":
                MovimientoJugador.enterAttack = false;
                MovimientoJugador.ataqueD = false;
                MovimientoJugador.ataqueD2 = false;
                MovimientoJugador.hasRotated = false; // Resetear para permitir nueva rotación
                MovimientoJugador.speed = 15;
                break;
            case "Attack3P":
                break;                
        }
    }
}
