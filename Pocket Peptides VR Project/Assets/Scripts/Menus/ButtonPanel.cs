using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonPanel : MonoBehaviour
{
    DropZone  dropZone;
    Draggable draggable;

    GameObject init_Obj; // for storing the first displayed object

    LevelSystem ls;

    AudioManager am;     // for referencing the audio manager script

    public GameObject components;    // for dragging in the parent of basic components
    public GameObject proteinModels; // for dragging in the parent of protein models
    public GameObject winPanel;      // for dragging in the win panel
    public GameObject losePanel;     // for dragging in the lose panel



    void Start()
    {
        // reference the DropZone and Draggabe scripts
        dropZone  = proteinModels.transform.parent.GetComponent<DropZone>();
        init_Obj  = dropZone.ActiveModel; // store the first displayed object
        if (init_Obj.name != "Complex")   // refer the init object's draggable
        {
            draggable = components.transform.Find(init_Obj.name).GetComponent<Draggable>();
            // subtract the displaying model's number left
            draggable.numberLeft -= 1;
            draggable.updateNumLabel();
        }
        // init level system
        ls = GameObject.Find("LevelSystem").GetComponent<LevelSystem>();
        // reference the Audio Manager script 
        am = FindObjectOfType<AudioManager>(); // find script directly
    }

    public void UndoComplex() // for level_3 complex models
    {
        // reset flags
        dropZone.IsGoalAchieved = false;
        // hide shadow models (not good)
        // proteinModels.transform.parent.Find("Shadow Models").gameObject.SetActive(false);
        GameObject.Find("Highlight Sphere").transform.position = new Vector3(100, 100, 100); // move light away
        // reset bones' rotation
        if (dropZone.b_RotationList.Count != 0) // if the list is not empty
        {
            int i = 0; // bone rotation list's index
            foreach (Transform child in dropZone.interactive.transform.GetComponentsInChildren<Transform>())
            {
                if (child.name.Contains("Bone"))
                {
                    child.rotation = dropZone.b_RotationList[i];
                    i++;
                }
            }
        }
    }

    public void UndoAll() // will be assined to RESET button, and be renamed as "Reset"
    {
        am.Stop("Twist"); // make sure no twist sound playing
        // subtract NumLeft, hide or destroy previous model
        dropZone.GameLogic();
        // reset flags
        dropZone.IsCombination  = false;
        dropZone.IsGoalAchieved = false;
        // hide shadow models (not good)
        // proteinModels.transform.parent.Find("Shadow Models").gameObject.SetActive(false);
        proteinModels.GetComponent<Rotation>().speed = 25f; // reset the rotation speed
        GameObject.Find("Highlight Sphere").transform.position = new Vector3(100, 100, 100); // move light away
        // reset bones' rotation
        if (dropZone.b_RotationList.Count != 0) // if the list is not empty
        {
            int i = 0; // bone rotation list's index
            foreach (Transform child in dropZone.interactive.transform.GetComponentsInChildren<Transform>())
            {
                if (child.name.Contains("Bone"))
                {
                    child.rotation = dropZone.b_RotationList[i];
                    i++;
                }
            }
        }
        // reset All NumLeft
        foreach (Transform child in components.transform)
        {
            if (child.gameObject.activeInHierarchy)
            {
                child.GetComponent<Draggable>().resetNumLeft();
            } // if the child is active, then reset the number
        }
        if (init_Obj.name == "Complex") { return; }
        // show the first displayed object
        init_Obj.SetActive(true);
        draggable.numberLeft -= 1;
        draggable.updateNumLabel();
    }
    public void Submit()
    {
        if (dropZone.IsGoalAchieved)
        {
            dropZone.b_RotationList.Clear(); // reset the list when switching levels
            winPanel.SetActive(true);
            am.Play("Victory");
        }
        else
        {
            losePanel.SetActive(true);
            am.Play("Lost");
        }
    }
    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("QUIT!");
    }

    public void GoToNextLevel()
    {
        // SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        FindObjectOfType<Transition>().FadeToNextLevel(); // use transition code
    }
}
