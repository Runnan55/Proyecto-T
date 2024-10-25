using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestBTanim : MonoBehaviour
{
    public Animator animator;
    public string triggerName = "BTTTrigger";
    public float minCooldown = 0.01f;
    public float maxCooldown = 0.20f;

    private bool isAnimating = true;
    private float originalAnimatorSpeed; // Guardar la velocidad original del Animator

    void Start()
    {
        animator = GetComponent<Animator>();
        originalAnimatorSpeed = animator.speed; // Guardar la velocidad original al inicio
        StartCoroutine(LoopAnimationWithCooldown());
    }

    void Update()
    {
        // Asegurarse de que la velocidad del Animator esté correctamente ajustada en tiempo real
        if (PlayerMovement.bulletTimeScale < 1f)
        {
            animator.speed = PlayerMovement.bulletTimeScale; // Ajustar la velocidad a la escala de bullet time
        }
        else
        {
            // Forzar la restauración de la velocidad original cuando bulletTimeScale vuelva a 1
            animator.speed = originalAnimatorSpeed;
        }
    }

    IEnumerator LoopAnimationWithCooldown()
    {
        while (isAnimating)
        {
            // Activa el trigger para la animación
            animator.SetTrigger(triggerName);

            // Forzar el reinicio de la animación al activar el trigger
            animator.Play(animator.GetCurrentAnimatorStateInfo(0).fullPathHash, -1, 0f);

            // Obtén la longitud de la animación, ajustada por la velocidad actual del Animator
            float animationLength = GetAnimationLength() / animator.speed;
            yield return new WaitForSeconds(animationLength);

            // Espera un tiempo aleatorio entre el cooldown mínimo y máximo
            float cooldown = Random.Range(minCooldown, maxCooldown) / animator.speed;
            yield return new WaitForSeconds(cooldown);
        }
    }

    float GetAnimationLength()
    {
        // Obtiene la longitud del estado de animación actual
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        return stateInfo.length;
    }
}