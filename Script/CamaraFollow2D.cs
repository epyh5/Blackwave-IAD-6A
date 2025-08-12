using UnityEngine;

public class CameraFollow2D : MonoBehaviour
{
    [Header("Target")]
    public Transform player;
    
    [Header("Follow Settings")]
    public bool followX = true;
    public bool followY = false;
    public float smoothTime = 0.3f;
    
    [Header("Offset")]
    public Vector2 offset = Vector2.zero;
    
    [Header("Boundaries")]
    public bool useMinMaxX = false;
    public float minX = 0f;
    public float maxX = 10f;
    
    private Vector2 velocity;

    void LateUpdate()
    {
        if (player == null) return;

        Vector3 targetPos = transform.position;
        
    
        if (followX)
        {
            targetPos.x = player.position.x + offset.x;
            
            
            if (useMinMaxX)
            {
                targetPos.x = Mathf.Clamp(targetPos.x, minX, maxX);
            }
        }
        
     
        if (followY)
        {
            targetPos.y = player.position.y + offset.y;
        }
        
        
        Vector2 smoothedPosition = Vector2.SmoothDamp(
            new Vector2(transform.position.x, transform.position.y),
            new Vector2(targetPos.x, targetPos.y),
            ref velocity,
            smoothTime
        );
        
        transform.position = new Vector3(smoothedPosition.x, smoothedPosition.y, transform.position.z);
    }
}