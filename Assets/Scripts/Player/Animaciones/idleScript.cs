using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class idleScript : StateMachineBehaviour
{
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
       PlayerMovement.hasRotated = false;
       
       PlayerMovement.enterAttack = false;
 
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (MovimientoJugador.isDashing)
        {
            return; // No ejecutar ataques durante el dash
        }

        if (MovimientoJugador.ataqueL)
        {           
             MovimientoJugador.instance.animator.Play("Attack1");
        }
        
        if (MovimientoJugador.ataqueP)
        {                         
             MovimientoJugador.instance.animator.Play("Attack1P");
        }

          if (MovimientoJugador.ataqueD)
        {                         
             MovimientoJugador.instance.animator.Play("Transition3P 0 0");
        }
        
       
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
         
       MovimientoJugador.ataqueL = false;    
       MovimientoJugador.ataqueP = false;
       MovimientoJugador.ataqueD = false;   
        
           
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
