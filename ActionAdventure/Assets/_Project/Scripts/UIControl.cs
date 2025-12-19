using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// using Cinemachine;

public class UIControl : MonoBehaviour
{
    [SerializeField] Transform leftZone = null;
    [SerializeField] Transform leftAnalog = null;

    // [SerializeField] private CinemachineFreeLook vcam = null;

    int leftID = -1;
    int rightID = -1;
    Vector3 deltaPos = Vector3.zero;
    Vector3 currentPos = Vector3.zero;

    private void MouseDown()
    {
        currentPos = Input.mousePosition;

        if (Camera.main.ScreenToViewportPoint(Input.mousePosition).x < 0.5f)
        {
            leftAnalog.transform.position = currentPos;
            leftZone.transform.position = currentPos - Vector3.up * 20;
            leftAnalog.gameObject.SetActive(true);
            leftZone.gameObject.SetActive(true);

            leftID = 0;
        }
        else if (Camera.main.ScreenToViewportPoint(Input.mousePosition).x > 0.5f)
        {
            rightID = 0;
        }
    }

    private Vector3 Direction()
    {
        var dir = (leftAnalog.position - leftZone.position).normalized;

        return new Vector3(dir.x, 0, dir.y);
    }

    private void MouseDrag()
    {
        deltaPos = currentPos - Input.mousePosition;
        transform.localRotation = Quaternion.Euler(0, Camera.main.transform.rotation.eulerAngles.y, 0);

        if (rightID == 0)
        {
            // vcam.m_XAxis.Value -= deltaPos.x * Time.deltaTime * 10f;
            // vcam.m_YAxis.Value -= deltaPos.y * Time.deltaTime * 0.5f;
        }
        if (leftID == 0)
        {
            leftAnalog.position = Input.mousePosition;
            if ((leftAnalog.position - leftZone.position).sqrMagnitude > 20 * 20)
            {
                leftZone.position += (leftAnalog.position - leftZone.position) * 0.5f;
            }

            transform.position += transform.forward * Direction().z * 3 * Time.deltaTime;
            transform.position += transform.right * Direction().x * 3 * Time.deltaTime;
        }


        currentPos = Input.mousePosition;
    }

    private void MouseUp()
    {
        leftAnalog.gameObject.SetActive(false);
        leftZone.gameObject.SetActive(false);

        leftID = -1;
        rightID = -1;
    }

    void Update()
    {
#if UNITY_EDITOR

        if (Input.GetKey(KeyCode.A))
        {
            // vcam.m_XAxis.Value -= 1 * Time.deltaTime * 20;
        }

        if (Input.GetKey(KeyCode.S))
        {
            // vcam.m_XAxis.Value += 1 * Time.deltaTime * 20;
        }


        if (Input.GetMouseButtonDown(0))
        {
            MouseDown();
        }

        if (Input.GetMouseButton(0))
        {
            MouseDrag();
        }

        if (Input.GetMouseButtonUp(0))
        {
            MouseUp();
        }
#else
        for (int i = 0; i < Input.touches.Length; i++)
        {
            TouchBegin(i);
            TouchEnd(i);
            TouchActions();
        }
#endif
    }

    public void TouchBegin(int _i)
    {
        if (Input.GetTouch(_i).phase == TouchPhase.Began)
        {
            if (Camera.main.ScreenToViewportPoint(Input.GetTouch(_i).position).x < 0.5f)
            {
                if (leftID == -1)
                {
                    leftID = Input.GetTouch(_i).fingerId;
                    leftAnalog.gameObject.SetActive(true);
                    leftZone.gameObject.SetActive(true);
                    leftAnalog.transform.position = Input.GetTouch(_i).position;
                    leftZone.transform.position = Input.GetTouch(_i).position - Vector2.up * 40;
                }
            }
            else if (Camera.main.ScreenToViewportPoint(Input.GetTouch(_i).position).x > 0.5f)
            {
                if (rightID == -1)
                {
                    rightID = Input.GetTouch(_i).fingerId;
                }
            }
        }
    }

    public void TouchEnd(int _i)
    {

        if (Input.GetTouch(_i).phase == TouchPhase.Ended)
        {

            if (Input.GetTouch(_i).fingerId == leftID)
            {
                leftAnalog.gameObject.SetActive(false);
                leftZone.gameObject.SetActive(false);

                leftID = -1;
            }

            if (Input.GetTouch(_i).fingerId == rightID)
            {
                rightID = -1;
            }
        }
    }

    public void TouchActions()
    {
        if (leftID != -1)
        {
            leftAnalog.position = Input.GetTouch(leftID).position;

            if ((leftAnalog.position - leftZone.position).sqrMagnitude > 20 * 20)
            {
                leftZone.position += (leftAnalog.position - leftZone.position) * 0.5f;
            }

            transform.position += transform.forward * Direction().z * 3 * Time.deltaTime;
            transform.position += transform.right * Direction().x * 3 * Time.deltaTime;
        }

        if (rightID != -1)
        {
            transform.localRotation = Quaternion.Euler(0, Camera.main.transform.rotation.eulerAngles.y, 0);

            // vcam.m_XAxis.Value += Input.GetTouch(rightID).deltaPosition.x * Time.deltaTime * 20f;
            // vcam.m_YAxis.Value -= Input.GetTouch(rightID).deltaPosition.y * Time.deltaTime * 0.5f;
        }
    }
}
