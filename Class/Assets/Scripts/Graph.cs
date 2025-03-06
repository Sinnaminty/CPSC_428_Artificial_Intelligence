using System.Collections.Generic;
using UnityEngine;

public class Graph : MonoBehaviour
{
    // Translates 1's and 0's from MapData.cs to an array of nodes
    public Node[,] nodes; //Array of nodes
    public List<Node> walls = new List<Node>();

    int[,] m_mapData;
    int m_width;
    int m_height;

    Node m_startNode;
    Node m_goalNode;

    public static readonly Vector2[] allDirections =
    {
        new Vector2(0f, 1f),
        //new Vector2(1f, 1f),
        new Vector2(1f, 0f),
        //new Vector2(1f, -1f),
        new Vector2(0f, -1f),
        //new Vector2(-1f, -1f),
        new Vector2(-1f, 0f),
        //new Vector2(-1f, 1f)
    };

    public int getWidth()
    {
        return m_width;
    }
    public int getHeight()
    {
        return m_height;
    }
    public Node getStartNode()
    {
        return m_startNode;
    }
    public Node getGoalNode()
    {
        return m_goalNode;
    }
    public void Init(int[,] mapData)
    {
        m_mapData = mapData;
        m_width = mapData.GetLength(0);
        m_height = mapData.GetLength(1);
        nodes = new Node[m_width, m_height];

        for (int y = 0; y < m_height; y++)
        {
            for (int x = 0; x < m_width; x++)
            {
                // make a node object
                Node newNode = new Node(x, y, (NodeType)mapData[x, y]);

                // our list of nodes is updated.
                nodes[x, y] = newNode;

                //assign position to the node in the world. -y so that it looks like the map 
                newNode.SetPosition(new Vector3(x, 0, -y));

                switch (newNode.GetNodeType())
                {
                    case NodeType.Blocked:
                        walls.Add(newNode);
                        break;

                    case NodeType.Start:
                        m_startNode = newNode;
                        break;

                    case NodeType.Goal:
                        m_goalNode = newNode;
                        break;

                }
            }
        }

        for (int y = 0; y < m_height; y++)
        {
            for (int x = 0; x < m_width; x++)
            {
                if (nodes[x, y].GetNodeType() != NodeType.Blocked)
                {
                    nodes[x, y].SetNeighbors(GetNeighbors(x, y, nodes, allDirections));
                }
            }
        }
    }

    public bool IsWithinBounds(int x, int y)
    {
        return (x >= 0 && x < m_width && y >= 0 && y < m_height);
    }

    List<Node> GetNeighbors(int x, int y, Node[,] NodeArray, Vector2[] directions)
    {
        List<Node> neighborNodes = new List<Node>();
        foreach (Vector2 dir in directions)
        {
            int newX = x + (int)dir.x;
            int newY = y + (int)dir.y;
            if (IsWithinBounds(newX, newY) && NodeArray[newX, newY] != null && NodeArray[newX, newY].GetNodeType() != NodeType.Blocked)
            {
                neighborNodes.Add(NodeArray[newX, newY]);
            }
        }
        return neighborNodes;
    }
}
