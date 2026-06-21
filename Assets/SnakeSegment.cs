using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnakeSegment
{
    public Vector2Int position;
    public Transform transform;
    public float direction;

    public SnakeSegment(Vector2Int position, Transform transform, float direction)
    {
        this.position = position;
        this.transform = transform;
        this.direction = direction;
    }
}
