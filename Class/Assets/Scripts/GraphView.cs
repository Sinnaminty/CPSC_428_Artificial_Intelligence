using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraphView : MonoBehaviour {
    public GameObject nodeViewPrefab;
    public Color openColor = Color.white;
    public Color blockedColor= Color.black;

    public void Init(Graph graph) { 
        if(graph == null){
            Debug.LogWarning("GraphView::Init. graph == null!");
            return;
        }

        foreach(Node n in graph.nodes) {
            GameObject instance = Instantiate(nodeViewPrefab, Vector3.zero, Quaternion.identity);
            NodeView nodeView = instance.GetComponent<NodeView>();

            if(nodeView != null) {
                nodeView.Init(n);
                //0 is open
                //1 is closed
                if(n.nodeType == 0) {
                    nodeView.ColorNode(openColor);
                } else {
                    nodeView.ColorNode(blockedColor);
                }
            } else {
                Debug.LogWarning("GraphView::Init. nodeView == null!");
            }
        }
    }
}
