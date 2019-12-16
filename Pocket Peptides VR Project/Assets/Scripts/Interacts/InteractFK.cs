using UnityEngine;

/*
 *  this is used for forward kinematic, move one bone will affect other bones nearby
 */

public class InteractFK : MonoBehaviour
{
    public GameObject Armature; // drag the Armature GameObject here

    Transform selection;
    bool IsDragging;
    bool IsDebugged;

    public float speed = 0.2f;

    Vector3 mPrevPos;
    Vector3 mPosDelta;

    CamRotation cr; // for changing the camera rotation state
    GameObject  highlight; // for changing highlight sphere's position

    bool isSoundPlayed; // for making sure Twist sound play only once

    // Update is called once per frame
    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Input.GetMouseButtonDown(0) && Physics.Raycast(ray, out hit))
        {
            IsDragging = true;
            selection = hit.transform;
            if (!isSoundPlayed)
            {
                FindObjectOfType<AudioManager>().Play("Twist"); // play Twist sound effect
                isSoundPlayed = true;
            }
        } // the mouse is pressed and the ray hits something

        if (Input.GetMouseButtonUp(0))
        {
            // Debug.Log("Mouse Released");
            selection  = null;
            IsDragging = false;
            IsDebugged = false;
            FindObjectOfType<AudioManager>().Stop("Twist"); // stop Twist sound effect
            isSoundPlayed = false;
        }

        // tracking mouse on hovering
        if (Physics.Raycast(ray, out hit))
        {
            //Debug.Log("Is dragging " + hit.transform.name);
            highlight.transform.position = hit.transform.position;
        } // mouse is hovering on a bone
        else
        {
            if (!IsDragging) // mouse left the bone and not dragging it, remove highlight
            {
                highlight.transform.position = new Vector3(100, 100, 100); // reset its position to somewhere else
            }
        }

        if (IsDragging)
        {
            if (!IsDebugged)
            {
                // print out the name of the bone that we're dragging, maybe useful someday
                // Debug.Log("Is dragging " + selection.name);
                // while dragging, display a highlight sphere
                highlight.transform.position = selection.position;
                IsDebugged = true;
            }
            // reset cam auto rotation's timer
            cr.timeElapsed = 0f; 
            // Preview Object, learnt from https://www.youtube.com/watch?v=kplusZYqBok (Modified)
            mPosDelta = Input.mousePosition - mPrevPos;
            if (Vector3.Dot(Camera.main.transform.up, Vector3.up) >= 0) // green (y) axis is pointing upwards
            {
                selection.Rotate(Camera.main.transform.up, Vector3.Dot(mPosDelta, Camera.main.transform.right) * speed, Space.World);
            }
            else // green (y) axis is pointing downwards
            {
                selection.Rotate(Camera.main.transform.up, -Vector3.Dot(mPosDelta, Camera.main.transform.right) * speed, Space.World);
            }
            if (Vector3.Dot(Camera.main.transform.right, Vector3.up) >= 0)
            {
                // rotates around the camera's red (x) axis   (changes to -Vector3 suits the protein end)
                selection.Rotate(Camera.main.transform.right, -Vector3.Dot(mPosDelta, Camera.main.transform.up) * speed, Space.World);
            }
            else
            {
                selection.Rotate(Camera.main.transform.right, Vector3.Dot(mPosDelta, Camera.main.transform.up) * speed, Space.World);
            }
            
        }
        mPrevPos = Input.mousePosition;

        // bends the selected bone's adjacent bones
        if (selection != null && selection.childCount > 0)
        {
            selection.GetChild(0).transform.Rotate(Camera.main.transform.up,  Vector3.Dot(mPosDelta, Camera.main.transform.right) * speed * 0.5f, Space.World);
            selection.GetChild(0).transform.Rotate(Camera.main.transform.right, -Vector3.Dot(mPosDelta, Camera.main.transform.up) * speed * 0.5f, Space.World);
            if (selection.GetChild(0).childCount > 0)
            {
                selection.GetChild(0).transform.Rotate(Camera.main.transform.up,  Vector3.Dot(mPosDelta, Camera.main.transform.right) * speed * 0.3f, Space.World);
                selection.GetChild(0).transform.Rotate(Camera.main.transform.right, -Vector3.Dot(mPosDelta, Camera.main.transform.up) * speed * 0.3f, Space.World);
            }
        } // if we select a bone and it has a child
        if (selection != null && selection.parent!= null && !selection.parent.name.Contains("Armature"))
        {
            selection.parent.transform.Rotate(Camera.main.transform.up,  Vector3.Dot(mPosDelta, Camera.main.transform.right) * speed * 0.5f, Space.World);
            selection.parent.transform.Rotate(Camera.main.transform.right, -Vector3.Dot(mPosDelta, Camera.main.transform.up) * speed * 0.5f, Space.World);
            if (selection.parent.parent != null && !selection.parent.parent.name.Contains("Armature"))
            {
                selection.parent.parent.transform.Rotate(Camera.main.transform.up,  Vector3.Dot(mPosDelta, Camera.main.transform.right) * speed * 0.3f, Space.World);
                selection.parent.parent.transform.Rotate(Camera.main.transform.right, -Vector3.Dot(mPosDelta, Camera.main.transform.up) * speed * 0.3f, Space.World);
            }
        } // if we select a bone and its parent isn't the Armature
    }

    void Start()
    {
        cr = GameObject.Find("Main Camera").GetComponent<CamRotation>();
        highlight = GameObject.Find("Highlight Sphere");
    } // init CamRotation
}