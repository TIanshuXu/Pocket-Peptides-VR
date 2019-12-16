using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;   // for LayoutElement
using TMPro;            // for TextMeshProUGUI

public class Draggable : MonoBehaviour
{
    // IDragHandler, IBeginDragHandler, IEndDragHandler
    public bool IsAvailable;   // there is some component left
    public  int  numberLeft;    // default protein component number
    private int  presetNum;     // used for recording preset component number
    private bool isCombination; // it means the model is not basic protein anymore

    public  Transform parentToReturnTo;
    public  GameObject placeholder;
    private GameObject darkenEffect; // represent the component is unavailable

    TextMeshProUGUI numLeft; // display the number of components left
    GameObject      blocker;
    GameObject      blockerGlow;
    DropZone        dropZone;

    public GameObject ProteinModels;

    private void Start()
    {
        // record how many components are set
        presetNum = numberLeft;
        // reference the numLeft
        numLeft = this.transform.Find("NumLeft").gameObject.GetComponent<TextMeshProUGUI>();
        updateNumLabel(); // update its text
        // display the name text on the Icon
        this.transform.Find("Name").gameObject.GetComponent<TextMeshProUGUI>().text = this.transform.name;
        // reference to the Darken gameObject
        darkenEffect = this.transform.Find("Darken").gameObject;
        // reference a blocker for refining the Orbit display
        // blocker     = GameObject.Find("Protein Drop Panel").transform.Find("Blocker").gameObject;
        // blockerGlow = GameObject.Find("Main Camera").transform.Find("Blocker Glow").gameObject;
        // reference the DropZone script
        dropZone = GameObject.Find("Protein Drop Panel").GetComponent<DropZone>();

    }
    private void Update()
    {
        // test if the component is available
        if (numberLeft > 0) 
        {
            IsAvailable = true; // make it draggable
            darkenEffect.SetActive(false);
            gameObject.GetComponent<Button>().interactable = true;
        } 
        else
        {
            IsAvailable = false;
            darkenEffect.SetActive(true);
            gameObject.GetComponent<Button>().interactable = false;
        }
    }
    
    public void displayMe() // display Model with the same name as the component
    {
        foreach (Transform child in ProteinModels.transform)
        {
            if (child.gameObject.activeInHierarchy)
            {
                child.gameObject.SetActive(false);
            }
        }
        //ProteinModels.transform.Find("Sheet").gameObject.SetActive(false);
        ProteinModels.transform.Find(gameObject.name).gameObject.SetActive(true);
        GameObject.Find("Name Text").GetComponent<TextMeshProUGUI>().text = gameObject.name;
        /*
        foreach (Transform child in dropZone.ParentModel.transform)
        {
            if (child.gameObject.activeInHierarchy) // if active
            {
                child.gameObject.SetActive(false);
                // reset previous component's number left
                GameObject.Find(child.name).GetComponent<Draggable>().resetNumLeft();
            }
        }
        dropZone.ParentModel.transform.Find(gameObject.name).gameObject.SetActive(true);
        */

        /*
        // test if the Model is still basic
        if (!dropZone.IsCombination)
        {
            // inactive the currently Active Model
            foreach (Transform child in dropZone.ParentModel.transform)
            {
                if (child.gameObject.activeInHierarchy) // if active
                {
                    child.gameObject.SetActive(false);
                    // reset previous component's number left
                    GameObject.Find(child.name).GetComponent<Draggable>().resetNumLeft();
                }
            }
            // active Model with the same name as the Component
            dropZone.ParentModel.transform.Find(gameObject.name).gameObject.SetActive(true);
            // subtract the number left of this component and update the label
            numberLeft -= 1;
            updateNumLabel();
        } // it's still basic
        */
    }
    public void resetNumLeft() // reset the number of component back to what has been set
    {
        numberLeft = presetNum;
        updateNumLabel();
    }
    public void updateNumLabel() // update the number text on the component
    {
        if (numberLeft > 0) 
        {
            numLeft.text = "x" + numberLeft.ToString();
        } // there is still some component left
        else 
        {
            numLeft.text = "";
        } // no component left, display nothing as the number text
    }
    

    /*
    public void OnBeginDrag(PointerEventData eventData)
    {
        // create a placeholder and set its size and index
        placeholder = new GameObject();
        placeholder.transform.SetParent(this.transform.parent);
        LayoutElement le   = placeholder.AddComponent<LayoutElement>();
        le.preferredWidth  = this.GetComponent<LayoutElement>().preferredWidth;
        le.preferredHeight = this.GetComponent<LayoutElement>().preferredHeight;
        le.flexibleWidth   = 0;
        le.flexibleHeight  = 0; // these are not working..
        placeholder.transform.localScale = new Vector2 (0.2f, 0.2f);
        placeholder.transform.SetSiblingIndex(this.transform.GetSiblingIndex()); 
        
        parentToReturnTo = this.transform.parent;
        this.transform.SetParent(this.transform.parent.parent);

        GetComponent<CanvasGroup>().blocksRaycasts = false;

        // enable the ProteinArea Blocker to block the camera orbit rotation
        blocker.SetActive(true);
        blockerGlow.SetActive(true);
        FindObjectOfType<AudioManager>().Play("Drag");  // play Drag sound effect
        GameObject.Find("Tooltip").SetActive(false);    // hide tooltips
    }
    public void OnDrag(PointerEventData eventData)
    {
        if (IsAvailable) // test if there is any component left
        {
            // move the component
            this.transform.position = eventData.position;
        }
        
        int newSiblingIndex = parentToReturnTo.childCount;
        
        for (int i = 0; i < parentToReturnTo.childCount; i++)
        {
            if (this.transform.position.y > parentToReturnTo.GetChild(i).position.y)
            {
                newSiblingIndex = i;
                if (placeholder.transform.GetSiblingIndex() < newSiblingIndex)
                {
                    newSiblingIndex--;
                }
                break;
            }
        }
        placeholder.transform.SetSiblingIndex(newSiblingIndex);
    }
    public void OnEndDrag(PointerEventData eventData)
    {
        this.transform.SetParent(parentToReturnTo);
        this.transform.SetSiblingIndex(placeholder.transform.GetSiblingIndex());
        GetComponent<CanvasGroup>().blocksRaycasts = true;
        Destroy(placeholder);
        // display the number left each time the mouse released
        updateNumLabel(); 
        // disable the ProteinArea Blocker to enable the camera orbit rotation
        blocker.SetActive(false);
        blockerGlow.SetActive(false);
        FindObjectOfType<AudioManager>().Stop("Drag"); // stop Drag sound effect
        GameObject.Find("Button Panel").transform.Find("Tooltip").gameObject.SetActive(true); // show tooltips
    }

    */
}
