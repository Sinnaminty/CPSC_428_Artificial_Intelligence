using UnityEngine;
using System.Collections.Generic;

public class MapData : MonoBehaviour
{
    int m_height;
    int m_width;
    public TextAsset m_textAsset;

    List<string> GetTextFromFile(TextAsset textAsset)
    {
        List<string> lines = new List<string>();
        if (textAsset != null)
        {
            string textData = textAsset.text;
            string[] delimiters = { "\r\n", "\n", "," };
            lines.AddRange(textData.Split(delimiters, System.StringSplitOptions.None));
        }

        return lines;
    }

    public List<string> GetTextFromFile()
    {
        return GetTextFromFile(m_textAsset);
    }


    void SetDimensions(List<string> textLines)
    {
        m_height = textLines.Count;
        m_width = textLines[0].Length;
    }

    public int[,] MakeMap()
    {
        List<string> lines = new List<string>();
        lines = GetTextFromFile();
        SetDimensions(lines);
        int[,] map = new int[m_width, m_height];
        for (int y = 0; y < m_height; y++)
        {
            for (int x = 0; x < m_width; x++)
            {
                map[x, y] = (int)char.GetNumericValue(lines[y][x]);
            }
        }
        return map;
    }
    public int GetWidth()
    {
        return m_width;
    }

    public int GetHeight()
    {
        return m_height;
    }
}