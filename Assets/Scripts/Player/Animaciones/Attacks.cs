using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.XR;

public class Attacks : StateMachineBehaviour
{

public bool hasAttacked = false;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
         
   GameObject damageZone = GameObject.Find("ZonaDaño");
     if (damageZone != null)
    {
        Collider collider = damageZone.GetComponent<Collider>();
        if (collider != null)
        {
            collider.enabled = true;
        }
    }
    slow();
   
    }
public void  slow()
{
    PlayerMovement.instance.speed = PlayerMovement.instance.reduccionVelocidad;
   
}
public void  Resetslow()
{
    PlayerMovement.instance.speed = PlayerMovement.instance.speed + 15f;
   
}
    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
         
    
   
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        PlayerMovement.hasAttacked = false;

          GameObject damageZone = GameObject.Find("ZonaDaño");
     if (damageZone != null)
    {
        Collider collider = damageZone.GetComponent<Collider>();
        if (collider != null)
        {
            collider.enabled = false;
        }
        Resetslow();
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
