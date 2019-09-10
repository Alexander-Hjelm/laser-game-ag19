
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserColorDefinitions
{   
    private struct ColorPair
    {
        public Color Color1;
        public Color Color2;

        public ColorPair(Color color1, Color color2)
        {
            Color1 = color1;
            Color2 = color2;
        }
    }

    private static Dictionary<Color, ColorPair> _splitColors = new Dictionary<Color, ColorPair>()
    {
        {Color.red, new ColorPair(Color.magenta, Color.yellow)},
        {Color.blue, new ColorPair(Color.cyan, Color.magenta)},
        {Color.green, new ColorPair(Color.yellow, Color.cyan)},
        {Color.yellow, new ColorPair(Color.red, Color.green)},
        {Color.magenta, new ColorPair(Color.blue, Color.red)},
        {Color.cyan, new ColorPair(Color.green, Color.green)}

    };

    public static void GetSplitColors(Color color, out Color outColor1, out Color outColor2)
    {
        if(_splitColors.ContainsKey(color))
        {
            outColor1 = _splitColors[color].Color1;
            outColor2 = _splitColors[color].Color2;
        }
        else
        {
            Debug.LogError("Tried to get split colors of color: " + color + ", but that color was not defined in LaserColorDefinitions");
            outColor1 = Color.black;
            outColor2 = Color.black;
        }
    }
}
