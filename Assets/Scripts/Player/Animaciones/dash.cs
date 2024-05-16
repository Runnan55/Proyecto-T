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
       PlayerMovement.hasRotated = false;   
    
       lifeInstance = GameObject.FindGameObjectWithTag("Player").GetComponent<Life>();

         GameObject Lightning = GameObject.Find("Lightning");

        var vfx1G1 = Lightning.GetComponent<VisualEffect>();
        if (vfx1G1 != null)
        {
            vfx1G1.enabled = true;
            vfx1G1.Play();
            
        }   

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
    
    }

    void Invulnerable()
    {
        if (lifeInstance != null)
        {
            lifeInstance.isInvincible = true;
        }
    }

    void Vulnerable()
    {
        if (lifeInstance != null)
        {
            lifeInstance.isInvincible = false;
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
