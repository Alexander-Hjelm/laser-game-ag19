using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    private Text hudText;
    private ObjectManager objMan;
    private ObjectManager.MaxObject[] mo;

    // Start is called before the first frame update
    void Start()
    {
        hudText = GetComponent<Text>();
        objMan = GameObject.FindGameObjectWithTag("GameController").GetComponent<ObjectManager>();
        mo = objMan.GetMaxObjectsPerType ();

        UpdateHUD();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateHUD(); // TODO possibly move this to only be called when an object is added or removed
    }

    // Counts current placed phycons and updates the HUD to display remaining objects
    private void UpdateHUD () {
        StringBuilder sb = new StringBuilder("");
        foreach (var cur in mo) {
            switch (cur.type) {
                case Objects.Mirror:
                    sb.Append("Mirror x ");
                    break;
                case Objects.Prism:
                    sb.Append("Prism x ");
                    break;
            }
            sb.Append(cur.max - objMan.CountSpawnedObjectsOfType(cur.type));
            sb.Append(" ");
        }
        hudText.text = sb.ToString();
    }
}
