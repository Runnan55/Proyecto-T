using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivadorNiebla : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    [SerializeField] private MonoBehaviour managerNiebla;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && managerNiebla != null)
        {
            managerNiebla.enabled = true;
        }
    }

    void Update()
    {
        
    }
}
