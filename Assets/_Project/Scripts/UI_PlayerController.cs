using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Unity.RemoteConfig;

public class UI_PlayerController : MonoBehaviour
{
    [SerializeField] Transform leftZone = null;
    [SerializeField] Transform leftAnalog = null;

    [SerializeField] private CinemachineFreeLook thirdPerson = null;
    [SerializeField] private CinemachineFreeLook overShoulder = null;
    [SerializeField] private Transform spine = null;

    private bool isOverShoulder = false;
    private Vector2 rotateAxis = Vector2.zero;

    private Animator animator = null;

    private int leftID = -1;
    private int rightID = -1;

    private float speed = 0f;
    private float cameraSensitivityX = 3f;
    private float cameraSensitivityY = 0.1f;

    struct userAttributes { };
    struct appAttributes { };


    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        ConfigManager.FetchCompleted += UpdateConfig;
        FetchConfig();
    }

    float delay = 0;

    void Update()
    {
        if (Input.touchCount >= 5 && delay < 0)
        {
            ToggleDebug();
            delay = 3f;
        }

        delay -= Time.deltaTime;

#if UNITY_EDITOR

        MouseInput();
#else
        ToggleDebug();

        if (Input.touchCount > 0)
        {
            for (int i = 0; i < Input.touchCount; i++)
            {
                TouchBegin(Input.touches[i], i);
                TouchActions(Input.touches[i]);
                TouchEnd(Input.touches[i], i);
            }
        }
#endif
    }

    public void TouchBegin(Touch _touch, int _i)
    {
        if (Input.GetTouch(_i).phase == TouchPhase.Began)
        {
            if (Camera.main.ScreenToViewportPoint(_touch.position).x < 0.5f)
            {
                if (leftID == -1)
                {
                    leftID = _touch.fingerId;
                    leftAnalog.gameObject.SetActive(true);
                    leftZone.gameObject.SetActive(true);
                    leftAnalog.transform.position = _touch.position;

                    leftZone.transform.position = _touch.position - Vector2.up * leftAnalog.GetComponent<RectTransform>().sizeDelta.y * 0.25f;
                }
            }
            else if (Camera.main.ScreenToViewportPoint(_touch.position).x > 0.5f)
            {
                if (rightID == -1)
                {
                    rightID = _touch.fingerId;
                }
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
                leftZone.position += (leftAnalog.position - leftZone.position) * 0.2f;
            }
            transform.localRotation = Quaternion.Euler(0, Camera.main.transform.rotation.eulerAngles.y, 0);

            speed = Direction().magnitude;

            WalkAnimation();
        }

        if (rightID != -1)
        {

            thirdPerson.m_XAxis.Value += Input.GetTouch(rightID).deltaPosition.x * Time.deltaTime * cameraSensitivityX;
            thirdPerson.m_YAxis.Value -= Input.GetTouch(rightID).deltaPosition.y * Time.deltaTime * cameraSensitivityY;
        }
    }
    public void TouchEnd(int _i)
    {

        if (Input.GetTouch(_i).phase == TouchPhase.Ended || Input.GetTouch(_i).phase == TouchPhase.Canceled)
        {

            if (Input.GetTouch(_i).fingerId == leftID)
            {
                leftAnalog.gameObject.SetActive(false);
                leftZone.gameObject.SetActive(false);
                speed = 0f;

                WalkAnimation();

                leftID = -1;
            }

            if (Input.GetTouch(_i).fingerId == rightID)
            {
                rightID = -1;
            }
        }
    }

    private void WalkAnimation()
    {
        animator.SetFloat("Horizontal", Direction().x);
        animator.SetFloat("Vertical", Direction().z);
        animator.SetFloat("Speed", speed * 2f);
    }

    private Vector3 Direction()
    {
        var dir = (leftAnalog.position - leftZone.position).normalized;

        return new Vector3(dir.x, 0, dir.y);
    }

    private void UpdateConfig(ConfigResponse response)
    {
        cameraSensitivityX = ConfigManager.appConfig.GetFloat(nameof(cameraSensitivityX));
        cameraSensitivityY = ConfigManager.appConfig.GetFloat(nameof(cameraSensitivityY));
    }


    #region MobileDebug
    private bool toggleDebug = false;
    [SerializeField] private GameObject debugMenu = null;

    private void ToggleDebug()
    {
        if (toggleDebug)
        {
            debugMenu.SetActive(false);
        }
        else
        {
            debugMenu.SetActive(true);
        }
    }

    public void FetchConfig()
    {
        ConfigManager.FetchConfigs(new userAttributes(), new appAttributes());
    }

    #endregion

    #region PC Debug
    //debug
    Vector3 deltaPos = Vector3.zero;
    Vector3 currentPos = Vector3.zero;


    private void MouseInput()
    {
        if (Input.GetKey(KeyCode.A))
        {
            thirdPerson.m_XAxis.Value -= 1 * Time.deltaTime * 20;
        }

        if (Input.GetKey(KeyCode.S))
        {
            thirdPerson.m_XAxis.Value += 1 * Time.deltaTime * 20;
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
    }

    private void MouseDown()
    {
        currentPos = Input.mousePosition;

        if (Camera.main.ScreenToViewportPoint(Input.mousePosition).x < 0.5f)
        {
            leftAnalog.transform.position = currentPos;
            leftZone.transform.position = currentPos - Vector3.up * leftAnalog.GetComponent<RectTransform>().sizeDelta.y * 0.5f;

            leftAnalog.gameObject.SetActive(true);
            leftZone.gameObject.SetActive(true);

            leftID = 0;
        }
        else if (Camera.main.ScreenToViewportPoint(Input.mousePosition).x > 0.5f)
        {
            rightID = 0;
        }
    }

    private void MouseDrag()
    {
        deltaPos = currentPos - Input.mousePosition;
        if (speed != 0f)
        {
            transform.localRotation = Quaternion.Euler(0, Camera.main.transform.rotation.eulerAngles.y, 0);
        }

        if (rightID == 0)
        {
            thirdPerson.m_XAxis.Value -= deltaPos.x * Time.deltaTime * 10f;
            thirdPerson.m_YAxis.Value -= deltaPos.y * Time.deltaTime * 0.5f;
        }
        if (leftID == 0)
        {
            leftAnalog.position = Input.mousePosition;
            if ((leftAnalog.position - leftZone.position).sqrMagnitude > 40 * 40)
            {
                leftZone.position += (leftAnalog.position - leftZone.position) * 0.2f;
            }
            speed = Direction().magnitude;

            WalkAnimation();
        }

        currentPos = Input.mousePosition;
    }

    private void MouseUp()
    {
        leftAnalog.gameObject.SetActive(false);
        leftZone.gameObject.SetActive(false);
        speed = 0f;

        WalkAnimation();

        leftID = -1;
        rightID = -1;
    }
    #endregion
}
