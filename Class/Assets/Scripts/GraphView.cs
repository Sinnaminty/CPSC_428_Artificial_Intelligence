using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraphView : MonoBehaviour
{
    public GameObject m_nodeViewPrefab;
    Color m_openColor = Color.white;
    Color m_blockedColor = Color.black;
    Color m_startColor = Color.green;
    Color m_goalColor = Color.red;
    NodeView[,] m_nodeViews;

    public void Init(Graph graph)
    {
        if (graph == null)
        {
            Debug.LogWarning("GraphView::Init. graph == null!");
            return;
        }
        m_nodeViews = new NodeView[graph.getWidth(), graph.getHeight()];
        foreach (Node n in graph.nodes)
        {
            GameObject instance = Instantiate(m_nodeViewPrefab, Vector3.zero, Quaternion.identity);
            NodeView nodeView = instance.GetComponent<NodeView>();

            if (nodeView != null)
            {
                nodeView.Init(n);
                m_nodeViews[n.GetXIndex(), n.GetYIndex()] = nodeView;
                switch (n.GetNodeType())
                {
                    case NodeType.Open:
                        nodeView.ColorNode(m_openColor);
                        break;

                    case NodeType.Blocked:
                        nodeView.ColorNode(m_blockedColor);
                        break;

                    case NodeType.Start:
                        nodeView.ColorNode(m_startColor);
                        break;

                    case NodeType.Goal:
                        nodeView.ColorNode(m_goalColor);
                        break;
                }
            }
            else
            {
                Debug.LogWarning("GraphView::Init. nodeView == null!");
            }
        }
    }

    public void ColorNodes(List<Node> nodes, Color color)
    {
        foreach (Node n in nodes)
        {
            if (n != null)
            {
                NodeView nodeView = m_nodeViews[n.GetXIndex(), n.GetYIndex()];
                if (nodeView != null)
                {
                    nodeView.ColorNode(color);
                }
            }
        }
    }

    public NodeView[,] GetNodeViews()
    {
        return m_nodeViews;
    }
}
