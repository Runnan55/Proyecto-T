using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piston : MonoBehaviour
{
    public float moveDistance = 10f;
    public float moveDurationForward = 2f;
    public float moveDurationBackward = 2f;
    public float waitDurationForwardToBackward = 3f;
    public float waitDurationBackwardToForward = 3f;
    public DamageZone damageZone;

    private Vector3 initialPosition;
    private bool isMovingForward = true;
    [SerializeField] private Vector3 moveDirection = Vector3.forward;

    void Start()
    {
        initialPosition = transform.position;
        StartCoroutine(MoveRoutine());
    }

    private IEnumerator MoveRoutine()
    {
        while (true)
        {
            if (isMovingForward)
            {
                damageZone.ActivateDamage();
                yield return MoveToPosition(initialPosition + moveDirection.normalized * moveDistance, moveDurationForward);
                yield return new WaitForSeconds(waitDurationForwardToBackward);
                isMovingForward = false;
            }
            else
            {
                yield return MoveToPosition(initialPosition, moveDurationBackward);
                yield return new WaitForSeconds(waitDurationBackwardToForward);
                isMovingForward = true;
            }
        }
    }

    private IEnumerator MoveToPosition(Vector3 targetPosition, float duration)
    {
        Vector3 startPosition = transform.position;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            transform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPosition;
    }
}