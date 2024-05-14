using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeguirMouse : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

   void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
    RaycastHit hit;

    if (Physics.Raycast(ray, out hit))
    {
        Vector3 targetPosition = new Vector3(hit.point.x, transform.position.y, hit.point.z);

        // Calcular la dirección hacia la que el jugador debe mirar
        Vector3 directionToLook = (targetPosition - transform.position).normalized;

        // Crear una rotación que mire en la dirección del objetivo
        Quaternion targetRotation = Quaternion.LookRotation(directionToLook);

        // Aplicar una rotación adicional de 180 grados alrededor del eje Y
        targetRotation *= Quaternion.Euler(0f, 180f, 0f);

        // Aplicar la rotación al objeto
        transform.rotation = targetRotation;
    }


 }
}
