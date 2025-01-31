using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PruebaMovimiento : MonoBehaviour
{
    private Rigidbody rb;
    private bool isDashing = false;
    private bool canDash = true;
    private float dashTime;

    public float speed = 10.0f;
    public float rotationSpeed = 720.0f; // Velocidad de rotación en grados por segundo
    public float dashSpeed = 20.0f; // Velocidad del dash
    public float dashDuration = 0.2f; // Duración del dash en segundos
    public float dashCooldown = 1.0f; // Tiempo de recarga del dash en segundos

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        movimientoJugador();
    }

    void movimientoJugador()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical);

        if (movement.magnitude > 1)
        {
            movement.Normalize();
        }

        if (movement != Vector3.zero)
        {
            Quaternion toRotation = Quaternion.LookRotation(movement, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, rotationSpeed * Time.deltaTime);
        }

        if (Input.GetKeyDown(KeyCode.LeftShift) && canDash && !isDashing)
        {
            StartCoroutine(Dash());
        }

        if (!isDashing)
        {
            Vector3 newPosition = rb.position + movement * speed * Time.deltaTime;
            rb.MovePosition(newPosition);
        }
    }

    IEnumerator Dash()
    {
        isDashing = true;
        canDash = false;
        dashTime = dashDuration;

        Vector3 dashDirection = transform.forward;

        while (dashTime > 0)
        {
            rb.MovePosition(rb.position + dashDirection * dashSpeed * Time.deltaTime);
            dashTime -= Time.deltaTime;
            yield return null;
        }

        isDashing = false;
        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }
}