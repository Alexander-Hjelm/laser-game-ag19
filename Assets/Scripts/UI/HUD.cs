using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HUD : MonoBehaviour {
    private TextMeshProUGUI hudText;
    private ObjectManager objMan;
    private ObjectManager.MaxObject[] mo;

    private string[] icons = { "<sprite=\"icons\" index=2>", "<sprite=\"icons\" index=0>", "<sprite=\"icons\" index=1>" };

    // Start is called before the first frame update
    void Start() {
        hudText = GetComponent<TextMeshProUGUI>();
        objMan = GameObject.FindGameObjectWithTag("GameController").GetComponent<ObjectManager>();
        mo = objMan.GetMaxObjectsPerType();

        UpdateHUD();
    }

    // Update is called once per frame
    void Update() {
        UpdateHUD(); // TODO possibly move this to only be called when an object is added or removed
    }

    // Counts current placed phycons and updates the HUD to display remaining objects
    private void UpdateHUD() {
        StringBuilder sb = new StringBuilder("");
        sb.Append("<font=\"sofachrome rg SDF\"> <mark=#41438080>");
        foreach (var cur in mo) {
            sb.Append(icons[(int)cur.type]);
            sb.Append(" : ");
            sb.Append(objMan.CountSpawnedObjectsOfType(cur.type));
            sb.Append("/");
            sb.Append(cur.max);
            sb.Append(" ");
        }
        sb.Append("</font>");
        sb.Append("</mark>");
        hudText.text = sb.ToString();
    }
}
