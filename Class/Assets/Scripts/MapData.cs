using UnityEngine;

public class MapData : MonoBehaviour
{
    public int height = 5;
    public int width = 5;

    public int[,] makeMap()
    {
        int[,] map = new int[width, height];
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                map[x,y] = 0;
            }
        }
        //map[0, 3] = 1;
        //map[1, 4] = 1;
        return map;
    }
}