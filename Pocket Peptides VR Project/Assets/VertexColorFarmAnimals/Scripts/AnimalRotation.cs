using UnityEngine;

public class AnimalRotation : MonoBehaviour
{
    public float speed = 25f;
    
    // Update is called once per frame
    void Update()
    {
        // transform.Rotate(Vector3.up    * speed * Time.deltaTime);
        transform.Rotate(Vector3.up * speed * Time.deltaTime);
    }
}
