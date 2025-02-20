using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeView : MonoBehaviour {

    public GameObject tile;
    public void Init(Node node){
        if(tile != null){
            gameObject.name = "Node (" + node.position.x + ", " + node.position.y + ")";
            tile.transform.position = node.position;
            tile.transform.localScale = new Vector3(1f, 1f, 1f);
        } else {
            Debug.LogWarning("NodeView::Init - tile == null!");
        }
    }
    void ColorNode(Color color, GameObject gameObject) {
        if(gameObject != null) {
            Renderer gameObjectRenderer = gameObject.GetComponent<Renderer>();
            gameObjectRenderer.material.color = color;
        }
    }
    public void ColorNode(Color color) {
        ColorNode(color, tile);
    }

}
