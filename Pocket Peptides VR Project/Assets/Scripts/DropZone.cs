using UnityEngine;
using UnityEngine.EventSystems;     // for Events
using UnityEngine.SceneManagement;  // for checking the level
using System.Collections;           // for IEnumerator
using System.Collections.Generic;   // for List<RaycastResult>

public class DropZone : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{
    public  GameObject ParentModel; // drag the Protein Models into it, traverse its children
    public  GameObject ActiveModel; // current active Model (can be basic or combination)
    private GameObject Combination; // for displaying the combined protein model
    private GameObject PrevCombina; // for checking if the Resource Load is successful

    public  float NewModelScale = 0.15f; // for scaling Combination's (x, y, z) axis
    public  bool  IsCombination;      // it means the model is not basic protein anymore
    public  bool  IsGoalAchieved;     // name matches or target points are close enough
    public  bool  IsNameMatched;      // Combination's name matches Shadow's name
    public  Material  CommonMat;      // Glow will be dragged here
    private Draggable draggable;      // for updating NumLeft, IsAvailable, and Darken Effect
    private string    ShadowName;     // for storing the active shadow model's name (The Goal)
    private string    AssetPath = "Combination/";  // for finding models in Resources Folder

    // for calculating distance between two target points
    private GameObject interactiveTargetPoint;
    private GameObject shadowModelTargetPoint;
    public  float      targetPointsDistance;
    public  float      clampScale = 0.5f; // how close will target points clamp
    [HideInInspector]
    public  GameObject interactive;
    [HideInInspector]
    public  GameObject shadowModel;

    // make a list for storing bones' rotation
    [HideInInspector]
    public List<Quaternion> b_RotationList;

    bool isSoundPlayed; // for making sure Clamp sound play only once

    public void OnDrop(PointerEventData eventData)
    {
        // Debug.Log(eventData.pointerDrag.name + " was dropped on " + gameObject.name);
        // get current ActiveModel first
        UpdateActiveModel();
        // setup bits and pieces
        draggable = eventData.pointerDrag.GetComponent<Draggable>();
        if (!draggable.IsAvailable) { return; } // if it runs out, we load nothing
        string nameA = ActiveModel.name + eventData.pointerDrag.name;
        string nameB = eventData.pointerDrag.name + ActiveModel.name;
        string pathA = AssetPath + nameA; //  + ".dae", without extension 
        string pathB = AssetPath + nameB; // "00" will be added for chance system
        // for higher than level_1, load nothing but display models have same names as shadows  
        if (SceneManager.GetActiveScene().buildIndex > 1)
        {
            // loop through all shadow models to find the active shadow
            foreach (Transform child in gameObject.transform.Find("Shadow Models"))
            {
                if (child.gameObject.activeSelf) // if active
                {
                    shadowModel = child.gameObject; // assign the current active shadow
                    ShadowName  = child.name;       // assign the shadow's name
                    // loop through all children of shadowModel to find the target point
                    foreach (Transform shadow_child in child.GetComponentsInChildren<Transform>())
                    {
                        if (shadow_child.name == "Target Point")
                        {
                            // assign the shadow model's target point
                            shadowModelTargetPoint = shadow_child.gameObject;
                        }
                    }
                }
            }
            // assign the interactive model and it's target point
            interactive = ParentModel.transform.Find(ShadowName).gameObject;
            //  check if combination's name matches shadow's name 
            if (nameA == ShadowName || nameB == ShadowName)
            {
                // reset the bone's rotation list
                b_RotationList = new List<Quaternion>();
                // loop through all children of interactive to find the target point
                foreach (Transform child in interactive.transform.GetComponentsInChildren<Transform>())
                {
                    if (child.name == "Target Point")
                    {
                        // assign the shadow model's target point
                        interactiveTargetPoint = child.gameObject;
                    }
                    if (child.name.Contains("Bone")) // store each bone's rotation in a list
                    {
                        b_RotationList.Add(child.rotation);
                    }
                }
                // once name matches, stop rotation to clamp the shadow and model
                ParentModel.GetComponent<Rotation>().speed = 0f;
                ParentModel.transform.localRotation = Quaternion.Euler(0, 0, 0);
                GameLogic(); // subtract the numberLeft, hide or destroy previous model, etc
                interactive.SetActive(true); // display it
                IsNameMatched = true;        // active the flag
                FindObjectOfType<AudioManager>().Play("Drop"); // play Drop sound effect
                return; // skip codes below, loads nothing
            }
            else
            {
                IsNameMatched = false;        // disable the flag
                interactive.SetActive(false); // if doesn't match, hide it 
            }
        }
        // try both names to load the file
        Combination = Resources.Load(pathA) as GameObject;
        if (Combination != null && Combination != PrevCombina)
        {
            GameLogic(); // subtract the numberLeft, hide or destroy previous model, etc
            InstantiateNewModel();
            Combination.name = nameA;  // assigns GameObject's name
            PrevCombina = Combination; // storing Combination GameObject for comparing
            FindObjectOfType<AudioManager>().Play("Drop"); // play Drop sound effect
        } // loaded successfully and Combination changed
        else
        {
            Combination = Resources.Load(pathB) as GameObject;
            if (Combination != null && Combination != PrevCombina)
            {
                GameLogic();
                InstantiateNewModel();
                Combination.name = nameB;
                PrevCombina = Combination;
                FindObjectOfType<AudioManager>().Play("Drop"); // play Drop sound effect
            }
            else
            {
                Debug.Log("The Combination " + nameA + " or " + nameB + " Doesn't Exist");
                // show the prompt and hide it after 2 sec
                GameObject.Find("Bottom Prompt Panel").GetComponent<Animator>().SetBool("IsOnDisplay", true);
                StartCoroutine(HideBottomPromptAfter(2));
                StartCoroutine(ShowHighlightEffect());   // highlight the reset button 
                return; // code below won't be executed
            } // loading failed
        }

        // check which shadow model is active (the goal for level_1)
        foreach (Transform child in gameObject.transform.Find("Shadow Models"))
        {
            if (child.gameObject.activeSelf) // if active
            {
                ShadowName = child.name;
            }
        }
        // check if the user achieves the goal
        if (Combination.name == ShadowName)
        {
            // once achieve the goal, stop rotation to clamp the shadow and model
            ParentModel.GetComponent<Rotation>().speed = 0f;
            ParentModel.transform.localRotation = Quaternion.Euler(0, 0, 0);
            IsGoalAchieved = true;
        } // the goal is achieved
        else
        {
            IsGoalAchieved = false;
            ParentModel.GetComponent<Rotation>().speed = 25f;
        } // once the goal isn't achieved, keep the rotation
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        // Debug.Log("Entering the DragZone!");
    } // no function, use OnDrop only
    public void OnPointerExit(PointerEventData eventData)
    {
        // Debug.Log("Leaving the DragZone!");
    }  // no function, use OnDrop only

    IEnumerator HideBottomPromptAfter(float sec)
    {
        yield return new WaitForSeconds(sec);
        GameObject.Find("Bottom Prompt Panel").GetComponent<Animator>().SetBool("IsOnDisplay", false);
    }
    IEnumerator ShowHighlightEffect()
    {
        GameObject Highlight = GameObject.Find("Main Camera").transform.Find("Hint Reset Model").gameObject;
        Highlight.SetActive(true); // show highlight for 3 sec
        yield return new WaitForSeconds(3);
        Highlight.SetActive(false);
    }

    // for all UIs (is pointer on any UI?)
    public bool IsPointerOnUI()
    {
        return EventSystem.current.IsPointerOverGameObject();
    } // to specify some UIs
    public bool IsPointerOverUIWithIgnores()
    {
        PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
        pointerEventData.position = Input.mousePosition;

        List<RaycastResult> raycastResultList = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerEventData, raycastResultList); // cast the ray
        
        for (int i = 0; i < raycastResultList.Count; i++)
        {
            if (raycastResultList[i].gameObject.GetComponent<MouseUIClickThrough>() != null)
            {
                raycastResultList.RemoveAt(i);
                i--;
            }
        }
        return !(raycastResultList.Count > 0);
    } // if it's only on an ignored UI, return true

    public  void GameLogic()
    {
        // subtract the numerLeft each time after a successful combination
        if (draggable != null) { draggable.numberLeft--; }
        // destroy or hide the previous Model (depends on if it's basic)
        if (IsCombination)
        {
            // destroy current Combined Model if it exists
            if (PrevCombina != null) { Destroy(PrevCombina); }
            else { interactive.SetActive(false); } // hide the interactive model
        }
        else
        {
            // get ActiveModel before hiding it
            UpdateActiveModel();
            ActiveModel.SetActive(false);
        }
        // reset flag (distance reset to 0)
        IsNameMatched = false;
    }
    private void InstantiateNewModel()
    {
        // Instantiate, and Setup the Combination GameObject
        Combination = Instantiate(Combination, ParentModel.transform);
        Combination.transform.localScale = new Vector3(NewModelScale, NewModelScale, NewModelScale);
        // Replace shaders with an Array
        Renderer   r = Combination.GetComponent<Renderer>();
        Material[] m = new Material[r.materials.Length]; // names here are simplified
        for (int i = 0; i < m.Length; i++)
        {
            m[i] = CommonMat;
        }
        r.materials = m;
        // The model is not basic anymore
        IsCombination = true;
    }
    private void UpdateActiveModel()
    {
        // try to find the Active Model first, then its name can be accessed
        foreach (Transform child in ParentModel.transform)
        {
            if (child.gameObject.activeInHierarchy) // if active
            {
                ActiveModel = child.gameObject;
            }
        }
    }

    private void Start()        // get an active object in dropzone first
    {
        UpdateActiveModel(); // retrieve the first displayed object
        // if the scene gets to level_3 (complex)
        if (SceneManager.GetActiveScene().buildIndex == 3)
        {
            SetUpComplexModel();
        }
    }   

    private void Update()       // for calculating distance between two target points
    {
        // updates goal distance for displaying on param panel
        if (IsNameMatched && !IsGoalAchieved)      
        {
            targetPointsDistance = Vector3.Distance(interactiveTargetPoint.transform.position, shadowModelTargetPoint.transform.position);
        }
    }

    private void LateUpdate()   // for clamping bones
    {
        if (SceneManager.GetActiveScene().buildIndex > 1)
        {
            // check if target points are close enough
            if (IsNameMatched && targetPointsDistance < clampScale)
            {
                IsGoalAchieved = true; // the goal achieved
                // this is in LateUpdate, make sure only play once
                if (!isSoundPlayed)
                {
                    FindObjectOfType<AudioManager>().Play("Clamp"); // play Clamp sound effect
                    isSoundPlayed = true;
                }
                if (SceneManager.GetActiveScene().buildIndex == 3)
                {
                    shadowModel.SetActive(false); // hide shadow for saving performance
                }
                // loop through all bones,  snap there position with shadow bones'
                foreach (Transform child in interactive.transform.GetComponentsInChildren<Transform>())
                {
                    if (child.name.Contains("Bone"))
                    {
                        foreach (Transform shadow_child in shadowModel.transform.GetComponentsInChildren<Transform>())
                        {
                            if (shadow_child.name == child.name)
                            {
                                child.transform.rotation = shadow_child.rotation;
                            }
                        }
                    }
                }
                targetPointsDistance = 0; // reset the distance
            }
            else
            {
                IsGoalAchieved = false;
                isSoundPlayed  = false;
            }
        }
    }

    private void SetUpComplexModel()
    {
        // loop through all shadow models to find the active shadow
        foreach (Transform child in gameObject.transform.Find("Shadow Models"))
        {
            if (child.gameObject.activeSelf) // if active
            {
                shadowModel = child.gameObject; // assign the current active shadow
                ShadowName = child.name;       // assign the shadow's name
                // loop through all children of shadowModel to find the target point
                foreach (Transform shadow_child in child.GetComponentsInChildren<Transform>())
                {
                    if (shadow_child.name == "Target Point")
                    {
                        // assign the shadow model's target point
                        shadowModelTargetPoint = shadow_child.gameObject;
                    }
                }
            }
        }
        // assign the interactive model and it's target point
        interactive = ParentModel.transform.Find(ShadowName).gameObject;

        // reset the bone's rotation list
        b_RotationList = new List<Quaternion>();
        // loop through all children of interactive to find the target point
        foreach (Transform child in interactive.transform.GetComponentsInChildren<Transform>())
        {
            if (child.name == "Target Point")
            {
                // assign the shadow model's target point
                interactiveTargetPoint = child.gameObject;
            }
            if (child.name.Contains("Bone")) // store each bone's rotation in a list
            {
                b_RotationList.Add(child.rotation);
            }
        }
        // once name matches, stop rotation to clamp the shadow and model
        ParentModel.GetComponent<Rotation>().speed = 0f;
        ParentModel.transform.localRotation = Quaternion.Euler(0, 0, 0);
        IsNameMatched = true;        // active the flag
    }
}
