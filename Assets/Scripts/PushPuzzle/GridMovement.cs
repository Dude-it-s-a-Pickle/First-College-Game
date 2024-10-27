using System.Collections.Generic;
using UnityEngine;

public class GridMovement : MonoBehaviour
{
    public Transform upCheck;
    public Transform downCheck;
    public Transform leftCheck;
    public Transform rightCheck;
    public LayerMask wall;
    public LayerMask push;
    public LayerMask ice; // New layer for ice blocks
    public Vector2 boxSize = new Vector2(1f, 1f);

    private bool isOnIce = false; // Track if the player is on ice
    private Vector2 iceDirection; // Direction to keep moving while on ice

    void Update()
    {
        if (isOnIce)
        {
            // If the player is on ice, keep moving in the ice direction
            MoveInDirection(iceDirection);
            CheckIfLeftIce();
        }
        else
        {
            // Normal movement logic
            #region WALLCHECK
            if (!Physics2D.OverlapBox(upCheck.position, boxSize, 0f, wall))
            {
                if (Input.GetKeyDown(KeyCode.UpArrow))
                {
                    MoveInDirection(Vector2.up);
                }
            }
            if (!Physics2D.OverlapBox(downCheck.position, boxSize, 0f, wall))
            {
                if (Input.GetKeyDown(KeyCode.DownArrow))
                {
                    MoveInDirection(Vector2.down);
                }
            }
            if (!Physics2D.OverlapBox(leftCheck.position, boxSize, 0f, wall))
            {
                if (Input.GetKeyDown(KeyCode.LeftArrow))
                {
                    MoveInDirection(Vector2.left);
                }
            }
            if (!Physics2D.OverlapBox(rightCheck.position, boxSize, 0f, wall))
            {
                if (Input.GetKeyDown(KeyCode.RightArrow))
                {
                    MoveInDirection(Vector2.right);
                }
            }
            #endregion
        }
    }

    private void MoveInDirection(Vector2 direction)
    {
        Vector2 currentPos = transform.position;
        Vector2 nextPos = currentPos + direction;

        List<Transform> pushableObjects = new List<Transform>();
        bool canMove = true;

        // Check if the next position contains walls or pushable objects
        while (true)
        {
            Collider2D[] colliders = Physics2D.OverlapBoxAll(nextPos, boxSize, 0f);
            bool foundPushable = false;

            foreach (var collider in colliders)
            {
                if (collider == null || collider.transform == transform)
                    continue;

                if ((wall & (1 << collider.gameObject.layer)) != 0)
                {
                    canMove = false;
                    break;
                }

                if ((push & (1 << collider.gameObject.layer)) != 0 && !pushableObjects.Contains(collider.transform))
                {
                    pushableObjects.Add(collider.transform);
                    foundPushable = true;

                    // Update the next position based on the size of the pushable object
                    nextPos += direction;
                }
            }

            if (!canMove || !foundPushable)
            {
                break;
            }
        }

        // Move if there are no obstacles
        if (canMove)
        {
            // Move pushable objects first
            foreach (Transform pushableObject in pushableObjects)
            {
                pushableObject.position += (Vector3)direction;
            }

            // Move player
            transform.position += (Vector3)direction;

            // Check if we are on ice
            if (Physics2D.OverlapBox(transform.position, boxSize, 0f, ice))
            {
                isOnIce = true;
                iceDirection = direction; // Set the ice direction to the current movement direction
            }
        }
        else
        {
            // Stop moving if hit an obstacle
            isOnIce = false;
        }
    }

    private void CheckIfLeftIce()
    {
        // Check if the player has left the ice block
        if (!Physics2D.OverlapBox(transform.position, boxSize, 0f, ice))
        {
            isOnIce = false;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(0f, 0f, 1f);

        if (upCheck != null)
        {
            Gizmos.DrawWireCube(upCheck.position, boxSize);
        }
        if (downCheck != null)
        {
            Gizmos.DrawWireCube(downCheck.position, boxSize);
        }
        if (leftCheck != null)
        {
            Gizmos.DrawWireCube(leftCheck.position, boxSize);
        }
        if (rightCheck != null)
        {
            Gizmos.DrawWireCube(rightCheck.position, boxSize);
        }
    }
}
