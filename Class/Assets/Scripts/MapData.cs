using UnityEngine;
using System.Collections.Generic;

public class MapData : MonoBehaviour
{
    public int height;
    public int width;
    public TextAsset textAsset;

    List<string> GetTextFromFile(TextAsset textAsset) {
        List<string> lines = new List<string>();
        if(textAsset != null) {
            string textData = textAsset.text;
            string[] delimiters = {"\r\n", "\n", ","};
            lines.AddRange(textData.Split(delimiters, System.StringSplitOptions.None));
        }

        return lines;
    }

    public List<string> GetTextFromFile() {
        return GetTextFromFile(textAsset);
    }


    void setDimensions(List<string> textLines) {
        height = textLines.Count;
        foreach (string line in textLines) {
            width = line.Length;
        }
    }

    public int[,] makeMap() {
        List<string> lines = new List<string>();
        lines = GetTextFromFile();
        setDimensions(lines);
        int[,] map = new int[width, height];
        for (int y = 0; y < height; y++) {
            for (int x = 0; x < width; x++) {
                map[x,y] = (int) char.GetNumericValue(lines[y][x]);
            }
        }
        return map;
    }
}