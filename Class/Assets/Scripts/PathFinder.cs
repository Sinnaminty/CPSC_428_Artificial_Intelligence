using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PriorityQueue<T>
{
    private List<(T item, float priority)> elements = new List<(T, float)>();

    public int Count => elements.Count;

    public void Enqueue(T item, float priority)
    {
        elements.Add((item, priority));
        elements.Sort((a, b) => a.priority.CompareTo(b.priority));
    }

    public T Dequeue()
    {
        var bestItem = elements[0].item;
        elements.RemoveAt(0);
        return bestItem;
    }

    public bool Contains(T item) => elements.Exists(e => EqualityComparer<T>.Default.Equals(e.item, item));
    public List<T> ToList()
    {
        List<T> list = new List<T>();
        foreach (var element in elements)
        {
            list.Add(element.item);
        }
        return list;
    }
}

public enum SearchAlgorithm
{
    BFS,
    DFS,
    AStarManhattan,
    AStarEuclidean,
    GBFS
}

public class PathFinder : MonoBehaviour
{
    public SearchAlgorithm m_searchAlgorithm;
    Node m_startNode;
    Node m_goalNode;
    Graph m_graph;
    GraphView m_graphView;
    Queue<Node> m_frontierNodes;
    PriorityQueue<Node> m_openSetNodes;
    Stack<Node> m_stackNodes;
    List<Node> m_exploredNodes;
    List<Node> m_pathNodes;
    Color m_startColor = Color.green;
    Color m_goalColor = Color.red;
    Color m_frontierColor = Color.magenta;
    Color m_exploredColor = Color.grey;
    Color m_pathColor = Color.cyan;

    bool m_isComplete = false;

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
        m_stackNodes = new Stack<Node>();
        m_openSetNodes = new PriorityQueue<Node>();

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
        if (m_stackNodes != null)
        {
            graphView.ColorNodes(m_stackNodes.ToList(), m_frontierColor);
        }
        if (m_openSetNodes != null)
        {
            graphView.ColorNodes(m_openSetNodes.ToList(), m_frontierColor);
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

    public IEnumerator BFSSearchRoutine(float timeStep = 0.1f)
    {
        m_frontierNodes.Enqueue(m_startNode);
        yield return null;
        while (!m_isComplete)
        {
            if (m_frontierNodes.Count > 0)
            {
                Node currentNode = m_frontierNodes.Dequeue();
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


    public IEnumerator DFSSearchRoutine(float timeStep = 0.1f)
    {
        m_stackNodes.Push(m_startNode);
        yield return null;
        while (!m_isComplete)
        {
            Debug.Log("PathFinder::DFSSearchRoutine - stackNodesSize = " + m_stackNodes.Count);
            if (m_stackNodes.Count > 0)
            {
                Node currentNode = m_stackNodes.Pop();
                if (!m_exploredNodes.Contains(currentNode))
                {
                    m_exploredNodes.Add(currentNode);
                }

                if (currentNode == m_goalNode)
                {
                    Debug.Log("PathFinder::DFSSearchRoutine - Found goal node!");
                    m_isComplete = true;
                    m_pathNodes = GetPathNodes(m_goalNode);
                    showColors(m_graphView, m_startNode, m_goalNode);
                    break;
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

    private float ManhattanDistance(Node a, Node b)
    {
        return Mathf.Abs(a.GetXIndex() - b.GetXIndex()) + Mathf.Abs(a.GetYIndex() - b.GetYIndex());
    }
    public IEnumerator AStarManhattanSearchRoutine(float timeStep = 0.1f)
    {
        Dictionary<Node, float> gScore = new Dictionary<Node, float>();
        Dictionary<Node, float> fScore = new Dictionary<Node, float>();

        m_openSetNodes.Enqueue(m_startNode, 0);
        gScore[m_startNode] = 0;
        fScore[m_startNode] = ManhattanDistance(m_startNode, m_goalNode);

        yield return null;

        while (m_openSetNodes.Count > 0)
        {
            Node currentNode = m_openSetNodes.Dequeue();

            if (!m_exploredNodes.Contains(currentNode))
            {
                m_exploredNodes.Add(currentNode);
            }

            if (currentNode == m_goalNode)
            {
                Debug.Log("PathFinder::AStarManhattanSearchRoutine - Found goal node!");
                m_isComplete = true;
                m_pathNodes = GetPathNodes(m_goalNode);
                showColors(m_graphView, m_startNode, m_goalNode);
                break;
            }

            foreach (Node neighbor in currentNode.GetNeighbors())
            {
                if (m_exploredNodes.Contains(neighbor)) continue;
                float tentativeGScore = gScore[currentNode] + 1;

                if (!gScore.ContainsKey(neighbor) || tentativeGScore < gScore[neighbor])
                {
                    neighbor.SetPrevious(currentNode); // Set previous node for path reconstruction
                    gScore[neighbor] = tentativeGScore;
                    fScore[neighbor] = gScore[neighbor] + ManhattanDistance(neighbor, m_goalNode);

                    if (!m_openSetNodes.Contains(neighbor))
                    {
                        m_openSetNodes.Enqueue(neighbor, fScore[neighbor]);
                    }
                }
            }

            yield return new WaitForSeconds(timeStep);
            showColors(m_graphView, m_startNode, m_goalNode);
        }
        m_isComplete = true;
    }

    // Manhattan heuristic function

    private float EuclideanDistance(Node a, Node b)
    {
        float dx = a.GetXIndex() - b.GetXIndex();
        float dy = a.GetYIndex() - b.GetYIndex();
        return Mathf.Sqrt(dx * dx + dy * dy);
    }

    public IEnumerator AStarEuclideanSearchRoutine(float timeStep = 0.1f)
    {
        Dictionary<Node, float> gScore = new Dictionary<Node, float>();
        Dictionary<Node, float> fScore = new Dictionary<Node, float>();

        m_openSetNodes.Enqueue(m_startNode, 0);
        gScore[m_startNode] = 0;
        fScore[m_startNode] = EuclideanDistance(m_startNode, m_goalNode);

        yield return null;

        while (m_openSetNodes.Count > 0)
        {
            Node currentNode = m_openSetNodes.Dequeue();

            if (!m_exploredNodes.Contains(currentNode))
            {
                m_exploredNodes.Add(currentNode);
            }

            if (currentNode == m_goalNode)
            {
                Debug.Log("PathFinder::AStarEuclideanSearchRoutine - Found goal node!");
                m_isComplete = true;
                m_pathNodes = GetPathNodes(m_goalNode);
                showColors(m_graphView, m_startNode, m_goalNode);
                break;
            }

            foreach (Node neighbor in currentNode.GetNeighbors())
            {
                if (m_exploredNodes.Contains(neighbor)) continue;
                float tentativeGScore = gScore[currentNode] + 1;

                if (!gScore.ContainsKey(neighbor) || tentativeGScore < gScore[neighbor])
                {
                    neighbor.SetPrevious(currentNode); // Set previous node for path reconstruction
                    gScore[neighbor] = tentativeGScore;
                    fScore[neighbor] = gScore[neighbor] + EuclideanDistance(neighbor, m_goalNode);

                    if (!m_openSetNodes.Contains(neighbor))
                    {
                        m_openSetNodes.Enqueue(neighbor, fScore[neighbor]);
                    }
                }
            }

            yield return new WaitForSeconds(timeStep);
            showColors(m_graphView, m_startNode, m_goalNode);
        }
        m_isComplete = true;
    }

    public IEnumerator GBFSSearchRoutine(float timeStep = 0.1f)
    {
        m_openSetNodes.Enqueue(m_startNode, ManhattanDistance(m_startNode, m_goalNode));

        yield return null;

        while (m_openSetNodes.Count > 0)
        {
            Node currentNode = m_openSetNodes.Dequeue();

            if (!m_exploredNodes.Contains(currentNode))
            {
                m_exploredNodes.Add(currentNode);
            }
            if (currentNode == m_goalNode)
            {
                Debug.Log("PathFinder::GBFSSearchRoutine - Found goal node!");
                m_isComplete = true;
                m_pathNodes = GetPathNodes(m_goalNode);
                showColors(m_graphView, m_startNode, m_goalNode);
                break;
            }

            foreach (Node neighbor in currentNode.GetNeighbors())
            {
                if (m_exploredNodes.Contains(neighbor)) continue;

                neighbor.SetPrevious(currentNode); // Set previous node for path reconstruction
                m_openSetNodes.Enqueue(neighbor, ManhattanDistance(neighbor, m_goalNode));
            }

            yield return new WaitForSeconds(timeStep);
            showColors(m_graphView, m_startNode, m_goalNode);
        }

        m_isComplete = true;
    }

    void ExpandFrontier(Node node)
    {
        for (int i = 0; i < node.GetNeighborsCount(); i++)
        {
            switch (m_searchAlgorithm)
            {
                case SearchAlgorithm.BFS:
                    {
                        if (!m_exploredNodes.Contains(node.GetNeighbors()[i]) && !m_frontierNodes.Contains(node.GetNeighbors()[i]))
                        {
                            List<Node> neighbors = node.GetNeighbors();
                            neighbors[i].SetPrevious(node);
                            m_frontierNodes.Enqueue(node.GetNeighbors()[i]);
                        }
                        break;
                    }

                case SearchAlgorithm.DFS:
                    {
                        if (!m_exploredNodes.Contains(node.GetNeighbors()[i]) && !m_stackNodes.Contains(node.GetNeighbors()[i]))
                        {
                            List<Node> neighbors = node.GetNeighbors();
                            neighbors[i].SetPrevious(node);
                            m_stackNodes.Push(node.GetNeighbors()[i]);
                        }
                        break;
                    }
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
