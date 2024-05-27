using UnityEngine;

public class TargetManager : MonoBehaviour
{
    public static TargetManager Instance { get; private set; }

    private int targetCount = 0;
    public int targetThreshold = 3;
    public GameObject objectToActivate;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void IncrementTargetCount()
    {
        targetCount++;
        if (targetCount >= targetThreshold)
        {
            ActivateObject();
        }
    }

    private void ActivateObject()
    {
        if (objectToActivate != null)
        {
            objectToActivate.SetActive(true);
        }
    }
}
