using UnityEngine;

public class Rotation : MonoBehaviour
{
    public float speed = 25f;
    Quaternion InitRotation; // for storing initial rotation

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(Vector3.up    * speed * Time.deltaTime);
        transform.Rotate(Vector3.right * speed * Time.deltaTime);
    }

    // for OVR setting scale and rotation with slider
    float x, y, z; // for scale
    float eX, eY, eZ; // for rotation

    private void Start()
    {
        InitRotation = gameObject.transform.localRotation;

        // for OVR setting scale and rotation with slider
        x = transform.localScale.x;
        y = transform.localScale.y;
        z = transform.localScale.z;
        eX = transform.eulerAngles.x;
        eY = transform.eulerAngles.y;
        eZ = transform.eulerAngles.z;
    }

    private void OnEnable()
    {
        // reset the rotation
        if (InitRotation != null)
        {
            gameObject.transform.localRotation = InitRotation; 
        }
        else
        {
            gameObject.transform.localRotation = Quaternion.Euler(0, 0, 0);
        }
    }

    // for OVR setting scale and rotation with slider
    public void SetScale(float scale)
    {
        transform.localScale = new Vector3(x * scale, y * scale, z * scale);
    }
    public void SetRotation(float rot)
    {
        transform.eulerAngles = new Vector3(eX, eY + rot, eZ);
    }

}
