using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserBounceEffect : MonoBehaviour
{
    private Dictionary<Color, string> _materialPathByColor = new Dictionary<Color, string>()
    {
        {Color.red, "Materials/FireBallRed"},
        {Color.blue, "Materials/FireBallBlue"},
        {Color.green, "Materials/FireBallGreen"},
        {Color.magenta, "Materials/FireBallMagenta"},
        {Color.cyan, "Materials/FireBallCyan"},
        {new Color(1,1,0,1), "Materials/FireBallYellow"}
    };

    private Dictionary<Color, string> _sparksMaterialPathByColor = new Dictionary<Color, string>()
    {
        {Color.red, "Materials/SparksRed"},
        {Color.blue, "Materials/SparksBlue"},
        {Color.green, "Materials/SparksGreen"},
        {Color.magenta, "Materials/SparksMagenta"},
        {Color.cyan, "Materials/SparksCyan"},
        {new Color(1,1,0,1), "Materials/SparksYellow"}
    };
    
    public void SetColor(Color color)
    {
        ParticleSystemRenderer particleSystemRenderer = GetComponent<ParticleSystemRenderer>();
        particleSystemRenderer.material = Resources.Load<Material>(_materialPathByColor[color]);

        ParticleSystemRenderer sparksRenderer = transform.Find("ElectricalSparks").GetComponent<ParticleSystemRenderer>();
        sparksRenderer.trailMaterial = Resources.Load<Material>(_sparksMaterialPathByColor[color]);
    }

}
