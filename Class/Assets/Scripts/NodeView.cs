using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeView : MonoBehaviour
{

    public GameObject tile;
    [Range(0, 0.5f)]
    public float borderSize = 0.15f;
    public void Init(Node node)
    {
        if (tile != null)
        {
            gameObject.name = "Node (" + node.GetXIndex() + ", " + node.GetYIndex() + ")";
            tile.transform.position = node.GetPosition();
            tile.transform.localScale = new Vector3(1f - borderSize, 1f, 1f - borderSize);
        }
        else
        {
            Debug.LogWarning("NodeView::Init - tile == null!");
        }
    }
    void ColorNode(Color color, GameObject gameObject)
    {
        if (gameObject != null)
        {
            Renderer gameObjectRenderer = gameObject.GetComponent<Renderer>();
            gameObjectRenderer.material.color = color;
        }
    }
    public void ColorNode(Color color)
    {
        ColorNode(color, tile);
    }

}
