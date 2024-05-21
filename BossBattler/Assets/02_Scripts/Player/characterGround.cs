using System.Collections.Generic;
using UnityEngine;

//This script is used by both movement and jump to detect when the character is touching the ground

public class CharacterGround : MonoBehaviour
{
    private bool onGround;
    public float groundYpos;
    private List<RaycastHit2D> raycastHits = new();

    [Header("Collider Settings")]
    [SerializeField][Tooltip("Length of the ground-checking collider")] private float groundLength = 0.95f;
    [SerializeField][Tooltip("Distance between the ground-checking colliders")] private Vector3 colliderOffset;

    [Header("Layer Masks")]
    [SerializeField][Tooltip("Which layers are read as the ground")] private LayerMask groundLayer;

    private void Update()
    {
        //Determine if the player is stood on objects on the ground layer, using a pair of raycasts
        raycastHits.Clear();
        raycastHits.Add(Physics2D.Raycast(transform.position + colliderOffset, Vector2.down, groundLength, groundLayer));
        raycastHits.Add(Physics2D.Raycast(transform.position - colliderOffset, Vector2.down, groundLength, groundLayer));

        onGround = false;
        foreach (var hit in raycastHits)
        {
            if (hit.collider != null)
            {
                onGround = true;
                groundYpos = hit.point.y;
            }
        }

    }

    private void OnDrawGizmos()
    {
        //Draw the ground colliders on screen for debug purposes
        if (onGround) { Gizmos.color = Color.green; } else { Gizmos.color = Color.red; }
        Gizmos.DrawLine(transform.position + colliderOffset, transform.position + colliderOffset + Vector3.down * groundLength);
        Gizmos.DrawLine(transform.position - colliderOffset, transform.position - colliderOffset + Vector3.down * groundLength);
    }

    //Send ground detection to other scripts
    public bool GetOnGround() { return onGround; }

}