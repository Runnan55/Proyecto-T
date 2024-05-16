using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.XR;
using UnityEngine.VFX;
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
        
        GameObject vfxObject1G1 = GameObject.Find("VFXSlash1G1");

        GameObject vfxObject2G1 = GameObject.Find("VFXSlash2G1");
         GameObject vfxObject2G11 = GameObject.Find("VFXSlash2G11");

        GameObject vfxObject1G2 = GameObject.Find("VFXSlash1G2");
        
         GameObject vfxObject31 = GameObject.Find("VFXSlash31");

        GameObject vfxObject2G2 = GameObject.Find("VFXSlash2G2");

        GameObject vfxObject32 = GameObject.Find("VFXSlash32");
           
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


                 var vfx1G1 = vfxObject1G1.GetComponent<VisualEffect>();
        if (vfx1G1 != null)
        {
            vfx1G1.enabled = true;
            vfx1G1.Play();
            
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

                  
                 var vfx2G1 = vfxObject2G1.GetComponent<VisualEffect>();
        if (vfx2G1 != null)
        {
            vfx2G1.enabled = true;
            vfx2G1.Play();
        }      

                      
                 var vfx2G11 = vfxObject2G11.GetComponent<VisualEffect>();
        if (vfx2G11 != null)
        {
            vfx2G11.enabled = true;
            vfx2G11.Play();
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
                      collider.enabled = true;
                    }         


             }


var vfx1G2 = vfxObject2G2.GetComponent<VisualEffect>();
        if (vfx1G2 != null)
        {
            vfx1G2.enabled = true;
            vfx1G2.Play();
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


           // Asegúrate de que vfxObject es el objeto que tiene el efecto visual que quieres activar
        var vfx2G2 = vfxObject1G2.GetComponent<VisualEffect>();
        if (vfx2G2 != null)
        {
            vfx2G2.enabled = true;
            vfx2G2.Play();
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


                   var vfx11G2 = vfxObject31.GetComponent<VisualEffect>();
        if (vfx11G2 != null)
        {
            vfx11G2.enabled = true;
            vfx11G2.Play();
        }

 var vfx22G2 = vfxObject32.GetComponent<VisualEffect>();
        if (vfx11G2 != null)
        {
            vfx22G2.enabled = true;
            vfx22G2.Play();
        }
   


                break;
       }
            slow();


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

    public void  slow()
{
    PlayerMovement.instance.speed = PlayerMovement.instance.reduccionVelocidad;
    PlayerMovement.slowless = true;
   
}
public void  Resetslow()
{
    PlayerMovement.instance.speed = PlayerMovement.instance.speed + 15f;
    PlayerMovement.slowless = false;
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
