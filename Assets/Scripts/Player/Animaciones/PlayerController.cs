using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController: MonoBehaviour
{
    public Animator myAnim;

    public static PlayerController instance;
    internal bool isAttacking;

    private void Awake()
    {
        instance = this;
    }


    // Start is called before the first frame update
    void Start()
    {
        myAnim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        Attack();

        
    }

    void Attack()
    {
        if(Input.GetKeyDown(KeyCode.A) && !isAttacking)
        {
            isAttacking = true;           
        }
        
    }
}
