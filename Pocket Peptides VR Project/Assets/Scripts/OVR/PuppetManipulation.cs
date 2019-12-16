using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuppetManipulation : MonoBehaviour
{
    OVRGrabbable ovrGrabbable;
    Vector3 CurtEuler; // for storing euler angles of the control cube
    Vector3 DeltEuler;

    bool isStartGrab = true; // start grab the cube

    private void Start()
    {
        ovrGrabbable = GetComponent<OVRGrabbable>();
    }

    private void Update()
    {
        int i = 0;
        float x = 0, y = 0, z = 0;
        if (ovrGrabbable.isGrabbed)
        {
            

            // transform.parent.eulerAngles = CurtEuler;

            // Debug.Log((this.transform.eulerAngles - CurtEuler) + " Delta " + i);
            // Debug.Log((this.transform.eulerAngles) + " Cube " + i);
            // Debug.Log((CurtEuler) + " Current " + i);
            //i++;

            //DeltEuler = Mathf.Abs(this.transform.eulerAngles - CurtEuler);
            /////
            //transform.parent.eulerAngles = transform.parent.eulerAngles + DeltEuler * 0.5f;   




            /*   Alternative method
            if (isStartGrab)
            {
                // record the initial cube euler angle
                CurtEuler = transform.parent.eulerAngles;
                isStartGrab = false;
            }

            DeltEuler = this.transform.eulerAngles - CurtEuler;

            //x = DeltEuler.x;
            //y = DeltEuler.y;
            //z = DeltEuler.z;
            if (DeltEuler.x > 0) x = 0.1f;
            else x = -0.1f;
            if (DeltEuler.y > 0) y = 0.1f;
            else y = -0.1f;
            if (DeltEuler.z > 0) z = 0.1f;
            else z = -0.1f;
            //DeltEuler.Set(x, y, z);

            transform.parent.Rotate(x, y, z);

            //transform.parent.eulerAngles += DeltEuler * 0.5f;

            */

            transform.parent.eulerAngles = this.transform.eulerAngles;
        }
        else
        {
            //CurtEuler = transform.parent.eulerAngles;
            // CurtEuler = this.transform.eulerAngles; // current cube's euler
            isStartGrab = true;
        }

    }
}
