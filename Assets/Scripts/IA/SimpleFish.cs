using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleFish : MonoBehaviour
{

    public int rutina;
    public float contador;
    //public Animator animator;
    public Quaternion angulo;
    public float grado;

    public GameObject target;
    public bool huyendo = false;
    public float speed = 1;
    public float maxSpeed = 2;
    public float distancia = 2;

    // Start is called before the first frame update
    void Start()
    {
        //  animator = GetComponent<Animator>();
        target = GameObject.Find("Player");
    }

    // Update is called once per frame
    void Update()
    {
        comportamiento();
    }

    public void comportamiento()
    {
        if (Vector3.Distance(transform.position, target.transform.position) > distancia)
        {
            //animator.SetBool("run", false);
            

            contador += 1 * Time.deltaTime;
            if (contador >= 2)
            {
                rutina = Random.Range(0, 2);
                contador = 0;
                huyendo = false;
            }

            switch (rutina)
            {
                case 0:
                    //animator.SetBool("walk", false);
                    break;
                case 1: //determina direccion de desplazamiento
                    grado = Random.Range(0, 360);
                    angulo = Quaternion.Euler(0, grado, 0);
                    rutina++;
                    break;
                case 2:
                    transform.rotation = Quaternion.RotateTowards(transform.rotation, angulo, 0.5f);
                    transform.Translate(Vector3.forward * speed * Time.deltaTime);
                    //animator.SetBool("walk", true);
                    break;

            }
        }
        else
        {
            if (Vector3.Distance(transform.position, target.transform.position) > 1 && !huyendo)
            {
                var lookPos = transform.position - target.transform.position;
                var rotation = Quaternion.LookRotation(lookPos);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, rotation, 3);
                transform.Translate(Vector3.forward * maxSpeed * Time.deltaTime);
                if (transform.position.y < 5)
                {
                    transform.position = new Vector3(transform.position.x, 5, transform.position.z);
                }
            }
            else
            {
                //animator.SetBool("walk", false);
                //animator.SetBool("run", false);

                //animator.SetBool("attack", true);
                huyendo = true;
            }
        }
    }

   


}
