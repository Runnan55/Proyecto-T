using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class dash : StateMachineBehaviour
{

  private Life lifeInstance;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
      override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

       MovimientoJugador.hasRotated = false;  
     
    
       lifeInstance = GameObject.FindGameObjectWithTag("Player").GetComponent<Life>();
     

        Invulnerable();
        
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
      Vulnerable();
    
  
    if (MovimientoJugador.ataqueP)
        {                         
             MovimientoJugador.instance.animator.Play("Attack1P");
        }

         MovimientoJugador.isInDodgeArea = false;
    }


    void Invulnerable()
    {
        if (lifeInstance != null)
        {
            lifeInstance.enableInvencibility();
            lifeInstance.setInvincibleMat();
        }
    }

    void Vulnerable()
    {
        if (lifeInstance != null)
        {
            lifeInstance.disableInvencibility();
            lifeInstance.setOriginalMat();
        }
    }



    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
