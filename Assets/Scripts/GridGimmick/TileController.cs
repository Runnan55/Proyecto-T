using UnityEngine;

public class TileController : MonoBehaviour
{
    [Header("Animation Settings")]
    [SerializeField] private float raisedHeight = 1.5f;
    [SerializeField] private float animationDuration = 0.5f;
    [SerializeField] private LeanTweenType raiseEase = LeanTweenType.easeOutBack;
    [SerializeField] private LeanTweenType lowerEase = LeanTweenType.easeInBack;
    
    private Vector3 originalPosition;
    private bool isRaised = false;
    private LTDescr currentTween;

    private void Awake() => originalPosition = transform.position;

    public void ToggleTile(bool raise)
    {
        if (raise == isRaised) return;
        
        isRaised = raise;
        float targetY = raise ? originalPosition.y + raisedHeight : originalPosition.y;
        
        if(currentTween != null) LeanTween.cancel(currentTween.id);
        
        var obstacle = GetComponent<UnityEngine.AI.NavMeshObstacle>();
        if (obstacle == null)
            obstacle = gameObject.AddComponent<UnityEngine.AI.NavMeshObstacle>();

        currentTween = LeanTween.moveY(gameObject, targetY, animationDuration)
            .setEase(raise ? raiseEase : lowerEase)
            .setOnStart(() => {
            if (raise)
            {
                gameObject.layer = LayerMask.NameToLayer("obstacleLayers");
                obstacle.carving = true;
            }
            })
            .setOnComplete(() => {
            if (!raise)
            {
                gameObject.layer = LayerMask.NameToLayer("VisibleThroughFog");
                obstacle.carving = false;
            }
            });
    }
}