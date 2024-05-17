using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallDetector : MonoBehaviour
{
    [Header("Detection")]
    [SerializeField] private Transform[] _detectionPointsRight;
    [SerializeField] private Transform[] _detectionPointsLeft;

    [SerializeField] private float _detectionLength = 0.1f;
    [SerializeField] private LayerMask _wallLayerMask;

    public bool DetectRightWallNearby()
    {
        foreach (Transform detectionPoint in _detectionPointsRight)
        {
            RaycastHit2D hitResult = Physics2D.Raycast(detectionPoint.position, Vector2.right, _detectionLength, _wallLayerMask);
            if (hitResult.collider != null && hitResult.collider.CompareTag("Player"))
            {

                return true;
            }
        }
        return false;
    }
    public bool DetectLeftWallNearby()
    {
        foreach (Transform detectionPoint in _detectionPointsLeft)
        {
            RaycastHit2D hitResult = Physics2D.Raycast(detectionPoint.position, -Vector2.right, _detectionLength, _wallLayerMask);
            if (hitResult.collider != null)
            {
                return true;
            }
        }
        return false;
    }
}
