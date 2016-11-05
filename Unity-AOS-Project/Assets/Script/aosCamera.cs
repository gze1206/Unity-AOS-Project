using UnityEngine;
using System.Collections;

public class aosCamera : MonoBehaviour
{
    
    void Update()
    {
        if (Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            transform.Translate(0, 0, -0.1f);
        }
        if (Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            transform.Translate(0, 0, 0.1f);
        }
        if (Input.GetMouseButton(0))
        {
            if (Input.GetAxis("Mouse X") < 0)
            {
                transform.Translate(0.1f, 0, 0);
            }
        }
        if (Input.GetMouseButton(0))
        {
            if (Input.GetAxis("Mouse X") > 0)
            {
                transform.Translate(-0.1f, 0, 0);
            }
        }
        if (Input.GetMouseButton(0))
        {
            if (Input.GetAxis("Mouse Y") < 0)
            {
                transform.Translate(0, 0.1f, 0);
            }
        }
        if (Input.GetMouseButton(0))
        {
            if (Input.GetAxis("Mouse Y") > 0)
            {
                transform.Translate(0, -0.1f, 0);
            }
        }
    }

}
