using System.Collections;
using UnityEngine;
using UnityEngine.VFX;

public class Attacks : StateMachineBehaviour
{
    public bool hasAttacked = false;
    public string attackNumber;
     private Collider ataqueL1Collider;   
     private Collider ataqueL2Collider; 
     private Collider ataqueL3Collider;
     private GameObject Cubo1;
     private GameObject Cubo2;
     private GameObject Cubo3; 

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
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
                MovimientoJugador.ataqueL = false;
                if (ataqueL1 != null)
                {
                    ataqueL1Collider = ataqueL1.GetComponent<Collider>();
                    if (ataqueL1Collider != null)
                    {
                        ataqueL1Collider.enabled = true; 
                    }
                }    
              if (Cubo1 != null)
    {
        MeshRenderer cubo1Renderer = Cubo1.GetComponent<MeshRenderer>();
        if (cubo1Renderer != null)
        {
            cubo1Renderer.enabled = true; 
        }
    }
                animator.GetComponent<MonoBehaviour>().StartCoroutine(ReduceSpeedTemporarily());
                break;                
            case "Attack2":  
                MovimientoJugador.ataqueL = false;
                MovimientoJugador.speed = 0; 

                    if (ataqueL1 != null)
                {
                    ataqueL2Collider = ataqueL2.GetComponent<Collider>();
                    if (ataqueL2Collider != null)
                    {
                        ataqueL2Collider.enabled = true; 
                    }
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
                    animator.GetComponent<MonoBehaviour>().StartCoroutine(playerMovement.EmpujarJugadorAL2(direccionEmpuje, 0.2f)); // Empuje durante 0.5 segundos
                }
                
                break;
                case "Attack3":
                MovimientoJugador.speed = 0; 
                if (ataqueL1 != null)
                {
                    ataqueL3Collider = ataqueL3.GetComponent<Collider>();
                    if (ataqueL3Collider != null)
                    {
                        ataqueL3Collider.enabled = true; 
                    }
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
                break;
            case "AttackP": 
            MovimientoJugador.speed = 0; 
            if (playerMovement.EstaMoviendose())
                {
                    Vector3 direccionEmpuje = playerMovement.ObtenerDireccionEmpuje();
                    animator.GetComponent<MonoBehaviour>().StartCoroutine(playerMovement.EmpujarJugadorAL2(direccionEmpuje, 0.1f)); // Empuje durante 0.5 segundos
                } 
            MovimientoJugador.ataqueP = false;  
             animator.SetBool("Dash", false);       
                break;
            case "AttackD": 
            MovimientoJugador.ataqueD = false;
            MovimientoJugador.speed = 0;          
                break;
            case "Attack3P":         
                break;
        }
    }

    private IEnumerator ReduceSpeedTemporarily()
    {
        float originalSpeed = MovimientoJugador.speed;
        MovimientoJugador.speed *= 0.8f; // Reduce speed ay 70%
        yield return new WaitForSeconds(1);
        
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
/*
         if (Input.GetKeyDown(KeyCode.Space))
        {
            MovimientoJugador.instance.animator.Play("Dash");
        }
*/
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        PlayerMovement.hasAttacked = false;
        MovimientoJugador.enterAttack = false; // Permitir rotación nuevamente
        GameObject ataqueL1 = GameObject.Find("AtaqueL1");
        GameObject ataqueL2 = GameObject.Find("AtaqueL2");
        GameObject ataqueL3 = GameObject.Find("AtaqueL3");
        GameObject Cubo1 = GameObject.Find("Cubo1");
        GameObject Cubo2 = GameObject.Find("Cubo2");
        GameObject Cubo3 = GameObject.Find("Cubo3");
   
        switch (attackNumber)
        {
            case "Attack1":
             MovimientoJugador.speed = 15; 
             MovimientoJugador.hasRotated = false;

               if (ataqueL1 != null)
                {
                    ataqueL1Collider = ataqueL1.GetComponent<Collider>();
                    if (ataqueL1Collider != null)
                    {
                        ataqueL1Collider.enabled = false; 
                    }
                }   
                if (Cubo1 != null)
    {
        MeshRenderer cubo1Renderer = Cubo1.GetComponent<MeshRenderer>();
        if (cubo1Renderer != null)
        {
            cubo1Renderer.enabled = false; // Activar el MeshRenderer
        }
    }

                break;              
            case "Attack2":
            MovimientoJugador.speed = 15; 
            MovimientoJugador.hasRotated = false;

              if (ataqueL1 != null)
                {
                    ataqueL2Collider = ataqueL2.GetComponent<Collider>();
                    if (ataqueL2Collider != null)
                    {
                        ataqueL2Collider.enabled = false; 
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
            MovimientoJugador.hasRotated = false;

              if (ataqueL1 != null)
                {
                    ataqueL3Collider = ataqueL3.GetComponent<Collider>();
                    if (ataqueL3Collider != null)
                    {
                        ataqueL3Collider.enabled = false; 
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
                MovimientoJugador.speed = 15;
                break;
            case "Attack3P":
             
                break;                
        }
       
    
    }

   
}
