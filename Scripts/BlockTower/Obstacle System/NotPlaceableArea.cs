using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NotPlaceableArea : MonoBehaviour
{
    public ObstacleType m_ObstacleType;

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Block>())
        {
            Block block = other.GetComponent<Block>();
            /*if (block.m_IsPlaced && !block.m_IsOnProcess && !block.m_IsDragging && block.m_CurrentBlockType == BlockType.Star)
            {
                // fail
                Debug.Log("Game Failed!");
            }*/
        }
    }
}

public enum ObstacleType
{
    Simple,
    Spike,
    OutsideCollider
    
}
