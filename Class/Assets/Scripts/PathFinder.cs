using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class PathFinder : MonoBehaviour
{
    Node m_startNode;
    Node m_goalNode;
    Graph m_graph;
    GraphView m_graphView;
    Queue<Node> m_frontierNodes;
    List<Node> m_exploredNodes;
    List<Node> m_pathNodes;
    Color m_startColor = Color.green;
    Color m_goalColor = Color.red;
    Color m_frontierColor = Color.magenta;
    Color m_exploredColor = Color.grey;
    Color m_pathColor = Color.cyan;

    bool m_isComplete = false;
    int m_iterations = 0;

    public void Init(Graph graph, GraphView graphView, Node start, Node goal)
    {
        if (start == null || goal == null || graphView == null || graph == null)
        {
            Debug.LogWarning("PathFinder::Init - Missing components.");
            return;
        }
        if (start.GetNodeType() == NodeType.Blocked || goal.GetNodeType() == NodeType.Blocked)
        {
            Debug.LogWarning("PathFinder::Init - Make sure start and goal nodes are open");
            return;
        }

        m_graph = graph;
        m_graphView = graphView;
        m_startNode = start;
        m_goalNode = goal;
        m_frontierNodes = new Queue<Node>();
        m_frontierNodes.Enqueue(start);
        m_pathNodes = new List<Node>();
        m_exploredNodes = new List<Node>();

        for (int r = 0; r < graph.getWidth(); r++)
        {
            for (int c = 0; c < graph.getHeight(); c++)
            {
                m_graph.nodes[r, c].Reset();
            }
        }

        showColors(graphView, start, goal);

        m_isComplete = false;
        m_iterations = 0;
    }

    void showColors(GraphView graphView, Node start, Node goal)
    {
        if (graphView == null || start == null || goal == null)
        {
            return;
        }

        NodeView startNodeView = graphView.GetNodeViews()[start.GetXIndex(), start.GetYIndex()];
        NodeView goalNodeView = graphView.GetNodeViews()[goal.GetXIndex(), goal.GetYIndex()];

        if (m_frontierNodes != null)
        {
            graphView.ColorNodes(m_frontierNodes.ToList(), m_frontierColor);
        }
        if (m_exploredNodes != null)
        {
            graphView.ColorNodes(m_exploredNodes, m_exploredColor);
        }
        if (m_pathNodes != null)
        {
            graphView.ColorNodes(m_pathNodes, m_pathColor);
        }

        if (startNodeView != null)
        {
            startNodeView.ColorNode(m_startColor);
        }
        else
        {
            Debug.LogWarning("StartNodeView does not exist");
        }
        if (goalNodeView != null)
        {
            goalNodeView.ColorNode(m_goalColor);
        }
        else
        {
            Debug.LogWarning("GoalNodeView does not exist");
        }

    }

    public IEnumerator SearchRoutine(float timeStep = 0.1f)
    {
        yield return null;
        while (!m_isComplete)
        {
            if (m_frontierNodes.Count > 0)
            {
                Node currentNode = m_frontierNodes.Dequeue();
                m_iterations++;
                if (!m_exploredNodes.Contains(currentNode))
                {
                    m_exploredNodes.Add(currentNode);
                }

                if (m_frontierNodes.Contains(m_goalNode))
                {
                    m_isComplete = true;
                    m_pathNodes = GetPathNodes(m_goalNode);
                }
                ExpandFrontier(currentNode);


                yield return new WaitForSeconds(timeStep);
            }
            else
            {
                m_isComplete = true;
            }
            showColors(m_graphView, m_startNode, m_goalNode);
        }
    }

    void ExpandFrontier(Node node)
    {
        for (int i = 0; i < node.GetNeighborsCount(); i++)
        {
            if (!m_exploredNodes.Contains(node.GetNeighbors()[i]) && !m_frontierNodes.Contains(node.GetNeighbors()[i]))
            {
                List<Node> neighbors = node.GetNeighbors();
                neighbors[i].SetPrevious(node);
                m_frontierNodes.Enqueue(node.GetNeighbors()[i]);
            }
        }
    }

    List<Node> GetPathNodes(Node endNode)
    {
        List<Node> path = new List<Node>();
        if (endNode == null)
        {
            return path;
        }
        path.Add(endNode);
        Node currentNode = endNode.GetPrevious();
        while (currentNode != null)
        {
            path.Insert(0, currentNode);
            currentNode = currentNode.GetPrevious();
        }
        return path;
    }
}
