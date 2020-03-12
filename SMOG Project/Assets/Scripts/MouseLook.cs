using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLook : MonoBehaviour
{
    enum MOUSELOOK
    {
        LOOK_X,
        LOOK_Y,
        LOOK_BOTH
    }

    [SerializeField] MOUSELOOK mouseLookType;
    [SerializeField] float sensitivity;
    [SerializeField] float minRotation;
    [SerializeField] float maxRotation;
    float midValue;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        float minRotationOverflowed = 360 + minRotation;
        midValue = (minRotationOverflowed + maxRotation) / 2;
    }

    void Update()
    {

        if(mouseLookType == MOUSELOOK.LOOK_Y)
        {
            float rawRotation = sensitivity * Input.GetAxis("Mouse Y");
            float newRotation = transform.localEulerAngles.x + (-rawRotation);

            if(newRotation < minRotation || newRotation >= midValue && newRotation < 360 + minRotation)
            {
                newRotation = minRotation;
            }
            else if(newRotation > maxRotation && newRotation < midValue)
            {
                newRotation = maxRotation;
            }

            transform.localEulerAngles = new Vector3(newRotation,0,0);
        }
        if (mouseLookType == MOUSELOOK.LOOK_X)
        {
            float rawRotation = sensitivity * Input.GetAxis("Mouse X");
            transform.eulerAngles += new Vector3(0, rawRotation, 0);
        }
    }
}
