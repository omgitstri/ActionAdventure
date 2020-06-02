using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class UIControl : MonoBehaviour
{
    [SerializeField] Transform leftZone = null;
    [SerializeField] Transform leftAnalog = null;
    [SerializeField] private List<int> leftScreenTouchID = new List<int>();
    [SerializeField] private List<int> rightScreenTouchID = new List<int>();
    [SerializeField] private CinemachineFreeLook vcam = null;

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

        if (rightID == 0)
        {
            vcam.m_XAxis.Value -= deltaPos.x * Time.deltaTime * 10f;
            vcam.m_YAxis.Value -= deltaPos.y * Time.deltaTime * 0.5f;
        }
        if (leftID == 0)
        {
            leftAnalog.position = Input.mousePosition;
            if ((leftAnalog.position - leftZone.position).sqrMagnitude > 20 * 20)
            {
                leftZone.position += (leftAnalog.position - leftZone.position) * 0.5f;
            }

            transform.position += Direction() * 3 * Time.deltaTime;
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


        //DebugTouch();

        leftScreenTouchID.Clear();
        rightScreenTouchID.Clear();

        for (int i = 0; i < Input.touches.Length; i++)
        {
            if (Camera.main.ScreenToViewportPoint(Input.GetTouch(i).position).x < 0.5f || Camera.main.ScreenToViewportPoint(Input.mousePosition).x < 0.5f)
            {
                leftScreenTouchID.Add(i);
            }
            else if (Camera.main.ScreenToViewportPoint(Input.GetTouch(i).position).x > 0.5f || Camera.main.ScreenToViewportPoint(Input.mousePosition).x > 0.5f)
            {
                rightScreenTouchID.Add(i);
            }
        }
    }

    public void DebugTouch()
    {
        if (leftScreenTouchID.Count > 0)
        {
            leftID = leftScreenTouchID[0];
        }
        else
        {
            leftID = -1;
        }

        if (rightScreenTouchID.Count > 0)
        {
            rightID = rightScreenTouchID[0];

#if UNITY_EDITOR
            vcam.m_XAxis.Value += deltaPos.x * Time.deltaTime * 10f;
            vcam.m_YAxis.Value += deltaPos.y * Time.deltaTime * 0.5f;

#else

#endif
            //Camera.main.transform.rotation = Quaternion.Euler(
            //    Camera.main.transform.rotation.eulerAngles.x + Input.GetTouch(rightID).deltaPosition.y * Time.deltaTime, 
            //    Camera.main.transform.rotation.eulerAngles.y + Input.GetTouch(rightID).deltaPosition.x * Time.deltaTime, 
            //    Camera.main.transform.rotation.eulerAngles.z);
        }
        else
        {
            rightID = -1;
        }
    }
}
