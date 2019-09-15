using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    private Text hudText;
    private MaxObject[] mo;

    // Start is called before the first frame update
    void Start()
    {
        hudText = GetComponent<Text>();
        mo = GameObject.FindGameObjectWithTag("GameController").GetComponent<ObjectManager> ().GetMaxObjectsPerType ();

        UpdateHUD();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void UpdateHUD () {
        StringBuilder sb = new StringBuilder("");
        foreach (MaxObject cur in mo) {
            switch (cur.type) {
                case Objects.Mirror:
                    sb.Append("Mirror x ");
                    sb.Append(cur.max);
                    sb.Append(" ");
                    break;
                case Objects.Prism:
                    sb.Append("Prism x ");
                    sb.Append(cur.max);
                    sb.Append(" ");
                    break;
            }
        }
        hudText.text = sb.ToString();
    }
}
