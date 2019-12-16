using UnityEngine;
using System.Collections;

public class HintManager : MonoBehaviour
{
    DropZone d; // for detect goal achieved flag
    bool isHint02Enabled; // flag

    static GameObject Highlight = null; // reference highlight object

    private void OnEnable()
    {
        if (Highlight != null)
        {
            Highlight.SetActive(false); // make sure it's off
        }
        // show highlight 4 sec then hide it
        if (gameObject.name == "Hint00")
        {
            StartCoroutine(ShowHighlightEffect(0)); // guide player to click ? button
        }
        if (gameObject.name == "Hint01")
        {
            StartCoroutine(ShowHighlightEffect(1));
        }
        if (gameObject.name == "Hint02")
        {
            isHint02Enabled = true;
            StartCoroutine(ShowHighlightEffect(2));
        }
        else { isHint02Enabled = false; } // disable the flag
        if (gameObject.name == "Hint03")
        {
            StartCoroutine(ShowHighlightEffect(3));
        }
    }

    IEnumerator ShowHighlightEffect(int index)
    {
        switch (index) // assign highlight base on index
        {
            case 0:
                Highlight = GameObject.Find("Main Camera").transform.Find("Hint00 Highlight").gameObject;
                break;
            case 1:
                Highlight = GameObject.Find("Main Camera").transform.Find("Hint01 Highlight").gameObject;
                break;
            case 2:
                Highlight = GameObject.Find("Main Camera").transform.Find("Hint02 Highlight").gameObject;
                break;
            case 3:
                Highlight = GameObject.Find("Main Camera").transform.Find("Hint03 Highlight").gameObject;
                break;
            default:
                break;
        }
        Highlight.SetActive(true); // show highlight for 4 sec
        yield return new WaitForSeconds(3);
        Highlight.SetActive(false);
    }
    
    private void Start()
    {
        d = GameObject.Find("Protein Drop Panel").GetComponent<DropZone>();
    } 

    private void Update()
    {
        if (isHint02Enabled && d.IsGoalAchieved)
        { // player win, show 03 to guide player submit
            GameObject.Find("Hint02").SetActive(false);
            GameObject.Find("Canvas").transform.Find("Tutorial Panel").transform.Find("Hint03").gameObject.SetActive(true);
        }
    }
}
