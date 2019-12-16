using UnityEngine;

public class CamRotation : MonoBehaviour
{
    // lock the vertical rotation (can be const)
    private const float Y_ANGLE_MIN = -85.0f;
    private const float Y_ANGLE_MAX = 85.0f;
    private const float ZOOM_IN_MIN = 20.0f;
    private const float ZOOM_IN_MAX = 2.0f;

    public Transform lookAt;
    public Transform camTransform;

    private Camera cam;

    private bool IsDragging;
    private DropZone d;
    
    public  float distance      = 10.0f;
    private float currentX      = 0.0f;
    private float currentY      = 0.0f;
    public  float sensivityX    = 10f;
    public  float sensivityY    = 8f;
    public  float sensivityZoom = 5f;

    public  GameObject ParentModel;       // drag the Protein Models into it, traverse its children
    public  float TimeBeforeRotate = 60f;  // default 5s, after 5s without interacting, then auto rotate
    public  float timeElapsed;
    private bool  IsRotationReset = true; // make sure the reset block only run once after 5s

    private void Start()
    {
        camTransform = this.transform;
        cam = Camera.main; // get current main cam

        // instantiate dropzone, detect if pointer is on ProteinArea
        d = ParentModel.transform.parent.GetComponent<DropZone>();
    }
    private void Update()
    {
        if (Input.GetMouseButtonDown(2)) // left mouseButton, for right is 1, middle is 2
        {
            IsDragging = true;
        } // Mouse Left Button is being pressed (dragging)
        if (Input.GetMouseButtonUp(2))
        {
            IsDragging = false;
        }   // Mouse Left Button is released (no more dragging)

        // zoom in and out
        if (d.IsPointerOnUI() && d.IsPointerOverUIWithIgnores()) 
        {
            distance += Input.GetAxis("Mouse ScrollWheel") * sensivityZoom;
            distance = Mathf.Clamp(distance, ZOOM_IN_MAX, ZOOM_IN_MIN);
        } 

        // rotation
        if (IsDragging && d.IsPointerOnUI() && d.IsPointerOverUIWithIgnores())
        { 
            currentX += Input.GetAxis("Mouse X") * sensivityX;
            currentY -= Input.GetAxis("Mouse Y") * sensivityY;
            // restrict Y rotation
            currentY = Mathf.Clamp(currentY, Y_ANGLE_MIN, Y_ANGLE_MAX);
            // stop the auto rotation of the current model
            ParentModel.GetComponent<Rotation>().speed = 0; // stop rotation until static, + 5s
            IsRotationReset = false;
            timeElapsed     = 0f;    // reset the timer
        } else // no manual rotate interaction
        {
            if (IsRotationReset)
            {
                return; // eliminate the tiny bug "5s elapsed"
            }
            if ((timeElapsed += Time.deltaTime) > TimeBeforeRotate && !d.IsGoalAchieved)
            {
                ParentModel.GetComponent<Rotation>().speed = 25; // reset the auto rotation
                IsRotationReset = true; // this block will only run once then
                timeElapsed     = 0f;   // reset the timer
            } // 5 sec elapsed and the goal hasn't been achieved
        } 
    }
    private void LateUpdate() // runs after runing the Update block each time
    {
        Vector3 dir = new Vector3(0, 0, -distance);
        Quaternion rotation = Quaternion.Euler(currentY, currentX, 0);
        // rotate cY around X axies, etc
        camTransform.position = lookAt.position + rotation * dir;
        camTransform.LookAt(lookAt.position);
    }
}
