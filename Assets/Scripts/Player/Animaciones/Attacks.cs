using System.Collections;
using UnityEngine;
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
            playerMovement.StartCoroutine(DashBackward(playerMovement));
                break;                
            case "Attack2":  
            playerMovement.StartCoroutine(DashBackward(playerMovement));           
                break;
            case "Attack3":
            playerMovement.StartCoroutine(DashBackward(playerMovement));
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

                // Start the dash coroutine
                playerMovement.StartCoroutine(DashForward(playerMovement));

                break;
            case "Attack2P":
                if (damageZone2G2 != null)
                {
                    Collider collider = damageZone2G2.GetComponent<Collider>();
                    if (collider != null)
                    {
                        collider.enabled = true;
                    }

                    var vfx2G2 = vfxObject1G2.GetComponent<VisualEffect>();
                    if (vfx2G2 != null)
                    {
                        vfx2G2.enabled = true;
                        vfx2G2.Play();
                    }

                    playerMovement.StartCoroutine(DashForward(playerMovement));
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

                    playerMovement.StartCoroutine(DashForward(playerMovement));
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

    // Coroutine to gradually increase the player's position
private IEnumerator DashForward(PlayerMovement playerMovement)
{
    float dashDistance = playerMovement.MeleeAttack;
    float dashTime = 0.1f; // Time over which the dash occurs
    float elapsedTime = 0f;

    Vector3 startPosition = playerMovement.transform.position;
    Vector3 endPosition = startPosition + playerMovement.transform.forward * dashDistance;

    while (elapsedTime < dashTime)
    {
        float step = (elapsedTime / dashTime) * dashDistance;
        Vector3 nextPosition = Vector3.Lerp(startPosition, endPosition, elapsedTime / dashTime);

        // Perform a raycast to check for obstacles
        if (Physics.Raycast(playerMovement.transform.position, playerMovement.transform.forward, out RaycastHit hit, step))
        {
            // If an obstacle is detected, stop the dash at the hit point
            playerMovement.transform.position = hit.point;
            yield break;
        }

        playerMovement.transform.position = nextPosition;
        elapsedTime += Time.deltaTime;
        yield return null;
    }

    playerMovement.transform.position = endPosition;
}

private IEnumerator DashBackward(PlayerMovement playerMovement)
{
    float dashDistance = playerMovement.DistanciaAttack;
    float dashTime = 0.1f; // Tiempo durante el cual ocurre el dash
    float elapsedTime = 0f;

    Vector3 startPosition = playerMovement.transform.position;
    Vector3 endPosition = startPosition - playerMovement.transform.forward * dashDistance;

    while (elapsedTime < dashTime)
    {
        float step = (elapsedTime / dashTime) * dashDistance;
        Vector3 nextPosition = Vector3.Lerp(startPosition, endPosition, elapsedTime / dashTime);

        // Realizar un raycast para verificar obstáculos
        if (Physics.Raycast(playerMovement.transform.position, -playerMovement.transform.forward, out RaycastHit hit, step))
        {
            // Si se detecta un obstáculo, detener el dash en el punto de impacto
            playerMovement.transform.position = hit.point;
            yield break;
        }

        playerMovement.transform.position = nextPosition;
        elapsedTime += Time.deltaTime;
        yield return null;
    }

    playerMovement.transform.position = endPosition;
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
                break;
            case "Attack2":
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

    public void slow()
    {
        PlayerMovement.instance.speed = PlayerMovement.instance.reduccionVelocidad;
        PlayerMovement.slowless = true;
    }

    public void Resetslow()
    {
        PlayerMovement.instance.speed = PlayerMovement.instance.speed + 15f;
        PlayerMovement.slowless = false;
    }
}
