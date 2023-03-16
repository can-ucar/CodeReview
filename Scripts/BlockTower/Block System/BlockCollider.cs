using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockCollider : MonoBehaviour
{
    [SerializeField] private Block m_Block;

    private void FixedUpdate()
    {

    }

    public Column collidedColumn;
    
    private void OnTriggerEnter(Collider other)
    {
        //if (!m_Block.m_IsDragging)
        //{
            //return;
        //}
        
        Column column = other.GetComponent<Column>();
        collidedColumn = column;

        if (column && !m_Block.m_IsPlaced && m_Block.m_IsDragging)
        {

            if (!column.m_IsUnlocked)
            {
                return;
            }
            if (column.m_IsColumnBusy)
            {
                //return;
            }
            if (!m_Block.m_InteractedColumns.Contains(column))
            {
                m_Block.m_InteractedColumns.Add(column);
            }

            /*
            if (m_InteractedColumns.Count > 1)
            {
                float distance1 = Mathf.Abs(m_InteractedColumns[0].transform.position.x - transform.position.x);
                float distance2 = Mathf.Abs(m_InteractedColumns[1].transform.position.x - transform.position.x);
                
                if (distance1 < distance2)
                {
                    m_MainColumn = m_InteractedColumns[1];
                }
                else
                {
                    m_MainColumn = m_InteractedColumns[0];
                }
                
            }
            else
            {
                m_MainColumn = column;
            }
            */

            
            //m_Block.m_MyColumn.m_MyBlocksInteractedColumn = m_Block.m_LastInteractedColumn;
            
            //if (m_Block.m_IsDragging && !m_Block.m_IsPlaced && m_Block.m_MyParent.m_ShapeTemplate.m_IsPlaceable)
            {
               // m_Block.m_LastInteractedColumn.AddMyListForPrePlacing(m_Block);
               // m_Block.m_LastInteractedColumn.m_IsPreview = true;
            }
        }

    }

    private void OnTriggerExit(Collider other)
    {
        Column column = other.GetComponent<Column>();
        if (collidedColumn == column) collidedColumn = null;
        if (column && m_Block.m_InteractedColumns.Contains(column))
        {
            m_Block.m_InteractedColumns.Remove(column);
        }
    }
    

    /*
    private void OnTriggerExit(Collider other)
    {
        Column column = other.GetComponent<Column>();
        
        if (column && !m_Block.m_IsPlaced && m_Block.m_IsDragging)
        {
            if (!column.m_IsUnlocked)
            {
                return;
            }
            if (column.m_IsColumnBusy)
            {
                return;
            }
            m_InteractedColumns.Clear();
            m_Block.m_LastInteractedColumn = null;
            m_Block.m_MyColumn.m_MyBlocksInteractedColumn = null;
            m_Block.OnOffMyGhost(false);
            
            if (m_Block.m_IsDragging && !m_Block.m_IsPlaced)
            {
                column.m_LastInteractedBlocks.Remove(m_Block);
                column.m_MyGhostBlocksList.Remove(m_Block);
                // m_NewGhostBlocks.Remove(interactedBlock);
                if (column.m_LastInteractedBlocks.Count==0)
                {
                    column.m_IsPreview = false;
                    column.ClearPreviewLists();
                    column.m_LastInteractedBlocks.Clear();
                    if (!column.m_IsOrganizingBlocks)
                    {
                        column.OrganizeBlocks();
                    }
                }

                if (m_Block.m_MyColumn == column.m_LastInteractedShapeColumn)
                {
                    column.m_LastInteractedShapeColumn = null;
                }
            }
        }
    }
    */
}
