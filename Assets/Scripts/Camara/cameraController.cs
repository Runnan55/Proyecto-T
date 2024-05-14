using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraController : MonoBehaviour
{
    public Transform[] views;
    public float transitionSpeed;
    Transform currentView;

    void Start()
    {
        currentView = transform;
    }

    
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.E))
        {
            currentView = views[0];
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            currentView = views[1];
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            currentView = views[2];
        }
    }

    private void LateUpdate()
    {
        transform.position = Vector3.Lerp(transform.position, currentView.position, Time.deltaTime * transitionSpeed);

        Vector3 currentAngle = new Vector3(
            Mathf.Lerp(transform.rotation.eulerAngles.x, currentView.transform.rotation.eulerAngles.x, Time.deltaTime * transitionSpeed),
            Mathf.Lerp(transform.rotation.eulerAngles.y, currentView.transform.rotation.eulerAngles.y, Time.deltaTime * transitionSpeed),
            Mathf.Lerp(transform.rotation.eulerAngles.z, currentView.transform.rotation.eulerAngles.z, Time.deltaTime * transitionSpeed)
            ); 
        transform.eulerAngles = currentAngle;
    }
}
