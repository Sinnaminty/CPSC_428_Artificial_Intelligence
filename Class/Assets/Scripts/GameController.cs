using Unity.VisualScripting;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public MapData mapData;
    public Graph graph;

    void Start()
    {
        if (mapData != null && graph != null)
        {
            int [,] mapInstance = mapData.makeMap(); // 2D array of 1's and 0's
            graph.Init(mapInstance); // Convert the above to array of nodes
            GraphView graphView = graph.gameObject.GetComponent<GraphView>();
            if(graphView != null) {
                graphView.Init(graph);
            } else {
                Debug.LogWarning("GameController::Start - graphView == null!");
            }
        }

    }
}