using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum NodeType
{
    Open = 0,
    Blocked = 1,
    Start = 2,
    Goal = 3
}

public class Node
{
    public int gScore;
    public int hScore;
    NodeType m_nodeType = NodeType.Open;
    int m_xIndex = -1;
    int m_yIndex = -1;
    Vector3 m_position;

    List<Node> m_neighbors = new List<Node>();
    Node m_previous = null;
    public Node(int xIndex, int yIndex, NodeType nodeType)
    {
        m_xIndex = xIndex;
        m_yIndex = yIndex;
        m_nodeType = nodeType;
    }

    public void Reset()
    {
        m_previous = null;
    }
    public NodeType GetNodeType()
    {
        return m_nodeType;
    }
    public int GetXIndex()
    {
        return m_xIndex;
    }

    public int GetYIndex()
    {
        return m_yIndex;
    }

    public Vector3 GetPosition()
    {
        return m_position;
    }
    public void SetPosition(Vector3 position)
    {
        m_position = position;
    }
    public List<Node> GetNeighbors()
    {
        return m_neighbors;
    }

    public void SetNeighbors(List<Node> neighbors)
    {
        m_neighbors = neighbors;
    }
    public int GetNeighborsCount()
    {
        return m_neighbors.Count;
    }

    public Node GetPrevious()
    {
        return m_previous;
    }

    public void SetPrevious(Node previous)
    {
        m_previous = previous;
    }

}