using UnityEngine;

public class BoneCollision : MonoBehaviour
{
    InteractFK iFK;
    GameObject colBoneA;
    GameObject colBoneB;

    bool isIFKAssigned;

    float boneDistance;
    DropZone    d;

    private void Start()
    {
        // reference the dropzone script
        d = GameObject.Find("Protein Drop Panel").GetComponent<DropZone>();
    }

    private void Update()
    {
        if (d.IsNameMatched && !isIFKAssigned)
        {
            iFK = d.interactive.GetComponent<InteractFK>();
            isIFKAssigned = true; // flag
        }
        if (!d.IsNameMatched)
        {
            isIFKAssigned = false; // reset the flag
        }
    }

    private void OnCollisionEnter(Collision colInfo)
    {
        if (colInfo.collider.name.Contains("Bone"))
        {
            //Debug.Log(colInfo.collider.name);
        }
        // reverse movement (not good, will be added later)
        /*
        if (iFK != null && iFK.speed > 0)
        {
            iFK.speed *= -1;
        }
        */
    }

    private void OnCollisionExit(Collision colInfo)
    {
        // reverse back (not good, will be added later)
        /*
        if (iFK != null && iFK.speed < 0)
        {
            iFK.speed *= -1;
        */
    }
}
