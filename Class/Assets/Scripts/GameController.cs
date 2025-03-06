using Unity.VisualScripting;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public MapData m_mapData;
    public Graph m_graph;
    public PathFinder m_pathFinder;

    [Range(0.1f, 1.0f)]
    public float m_timeStep = 0.1f;

    void Start()
    {
        if (m_mapData != null && m_graph != null)
        {
            // Initialize our graph using mapData
            m_graph.Init(m_mapData.MakeMap());

            // What does GetComponent do?
            GraphView graphView = m_graph.GetComponent<GraphView>();

            if (graphView != null)
            {
                graphView.Init(m_graph);
            }
            else
            {
                Debug.LogWarning("GameController::Start - graphView == null!");
            }

            Node startNode = m_graph.getStartNode();
            Node goalNode = m_graph.getGoalNode();

            if (m_pathFinder != null)
            {
                m_pathFinder.Init(m_graph, graphView, startNode, goalNode);
                StartCoroutine(m_pathFinder.SearchRoutine(m_timeStep));
            }
            else
            {
                Debug.LogWarning("GameController::Start - graph out of range!");
            }
        }

    }
}