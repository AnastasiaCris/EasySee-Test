using System;
using TMPro;
using UnityEngine;
public class UIManager : MonoBehaviour
{
    private int minSize = 3;
    private int maxSize = 20;

    public static UIManager instance;

    private void Awake()
    {
        if (instance == null) instance = this;
    }

    //---------------------------------Grid Values--------------------------------

    /// <summary>
    /// Change Width of the grid in the input field
    /// </summary>
    public void OnWidthValueChanged(TMP_InputField textField)
    {
        int parseW = 0;
        if (int.TryParse(textField.text, out parseW))
        {
            parseW = Int32.Parse(textField.text);
        }
        if (parseW <= minSize && textField.text.Length > 0)
        {
            parseW = minSize;
            textField.text = parseW.ToString();
        }
        if (parseW >= maxSize)
        {
            parseW = maxSize;
            textField.text = parseW.ToString();
        }

        Grid.Width = parseW;
    }
    
    /// <summary>
    /// Change Height of the grid in the input field
    /// </summary>
    public void OnHeightValueChanged(TMP_InputField textField)
    {
        int parseH = 0;
        if (int.TryParse(textField.text, out parseH))
        {
            parseH = Int32.Parse(textField.text);
        }
        if (parseH <= minSize && textField.text.Length > 0)
        {
            parseH = minSize;
            textField.text = parseH.ToString();
        }
        if (parseH >= maxSize)
        {
            parseH = maxSize;
            textField.text = parseH.ToString();
        }

        Grid.Height = parseH;
    }
    
    //---------------------------------Camera--------------------------------

    /// <summary>
    /// Setting up camera position and size to always be in the middle of the maze and always be able to see the whole maze
    /// </summary>
    public void SetUpCamera(float width, float height)
    {
        Camera cam = Camera.main;
        
        float camZoom = height >= width || height == width? 2 + height / 2 : 1 + width / 2;

        if (camZoom <= 4) camZoom = 4; // size can't be smaller then 4

        cam.orthographicSize = camZoom;
    }
}
