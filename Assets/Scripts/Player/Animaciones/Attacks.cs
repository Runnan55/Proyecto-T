using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.XR;

public class Attacks : StateMachineBehaviour
{

public bool hasAttacked = false;
public string attackNumber;   

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        PlayerMovement playerMovement = PlayerMovement.instance;
         GameObject damageZoneG1 = GameObject.Find("ZonaDañoG1");
        GameObject damageZone2G1 = GameObject.Find("ZonaDaño2G1");
        GameObject damageZoneG2 = GameObject.Find("ZonaDañoG2");
        GameObject damageZone2G2 = GameObject.Find("ZonaDaño2G2");
        GameObject damageZone3G2 = GameObject.Find("ZonaDaño3G2");
        PlayerMovement.cambioarma = false;
           
   switch (attackNumber)
        {
            case "Attack1":

                
                 if (damageZoneG1 != null)
                 {

                    Collider collider = damageZoneG1.GetComponent<Collider>();

                     if (collider != null)
                    {
                      collider.enabled = true;
                    }
                 }                            
                
                break;
            case "Attack2":
               
               
                if (damageZone2G1 != null)
                 {

                    Collider collider = damageZone2G1.GetComponent<Collider>();

                     if (collider != null)
                    {
                      collider.enabled = true;
                    }
                 }          

                break;
            case "Attack3":

                
                playerMovement.GrimorioDistanciaVuelta();
                break;
            case "Attack1P":
                    if (damageZoneG2 != null)
                 {

                    Collider collider = damageZoneG2.GetComponent<Collider>();

                     if (collider != null)
                    {
                      collider.enabled = true;
                    }
             }
                break;
            case "Attack2P":
                     if (damageZone2G2 != null)
                 {

                    Collider collider = damageZone2G2.GetComponent<Collider>();

                     if (collider != null)
                    {
                      collider.enabled = true;
                    }
             }
                break;
            case "Attack3P":
                        if (damageZone3G2 != null)
                 {

                    Collider collider = damageZone3G2.GetComponent<Collider>();

                     if (collider != null)
                    {
                      collider.enabled = true;
                    }
             }
                break;
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
           
        GameObject damageZoneG1 = GameObject.Find("ZonaDañoG1");
        GameObject damageZone2G1 = GameObject.Find("ZonaDaño2G1");
        GameObject damageZoneG2 = GameObject.Find("ZonaDañoG2");
        GameObject damageZone2G2 = GameObject.Find("ZonaDaño2G2");
        GameObject damageZone3G2 = GameObject.Find("ZonaDaño3G2");
   
        switch (attackNumber)
        {
            case "Attack1":

        
                  if (damageZoneG1 != null)
                 {

                    Collider collider = damageZoneG1.GetComponent<Collider>();

                     if (collider != null)
                    {
                      collider.enabled = false;
                    }
                 }          

                
                break;
            case "Attack2":
               

            if (damageZone2G1 != null)
                 {

                    Collider collider = damageZone2G1.GetComponent<Collider>();

                     if (collider != null)
                    {
                      collider.enabled = false;
                    }
                 }          



                break;
            case "Attack3":
               
                break;
            case "Attack1P":
                    if (damageZoneG2 != null)
            {
                Collider collider = damageZoneG2.GetComponent<Collider>();
                
                  if (collider != null)
                  {
                     collider.enabled = false;
                  }
                   
              
            }
                break;
            case "Attack2P":
                        if (damageZone2G2 != null)
                 {

                    Collider collider = damageZone2G2.GetComponent<Collider>();

                     if (collider != null)
                    {
                      collider.enabled = false;
                    }
             }
                break;
            case "Attack3P":
                        if (damageZone3G2 != null)
                 {

                    Collider collider = damageZone3G2.GetComponent<Collider>();

                     if (collider != null)
                    {
                      collider.enabled = false;
                    }
             }
                break;                
       }
       Resetslow();  
         PlayerMovement.cambioarma = true;
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
