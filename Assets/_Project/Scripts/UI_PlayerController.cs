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
        if (Input.touchCount > 0)
        {
            for (int i = 0; i < Input.touchCount; i++)
            {
                TouchBegin(Input.touches[i]);
                TouchDrag(Input.touches[i]);
                TouchEnd(Input.touches[i]);
            }
        }
#endif
    }

    public void TouchBegin(Touch _touch)
    {
        if (_touch.phase == TouchPhase.Began)
        {
            var screenPosition = Camera.main.ScreenToViewportPoint(_touch.rawPosition).x;

            if (screenPosition < 0.5f)
            {
                if (leftID == -1)
                {
                    leftID = _touch.fingerId;
                    leftAnalog.gameObject.SetActive(true);
                    leftZone.gameObject.SetActive(true);
                    leftAnalog.transform.position = _touch.position;

                    leftZone.transform.position = _touch.position - Vector2.up * leftAnalog.GetComponent<RectTransform>().rect.height * 0.5f;
                }
            }
            else if (screenPosition > 0.5f)
            {
                if (rightID == -1)
                {
                    rightID = _touch.fingerId;
                }
            }
        }
    }

    public void TouchDrag(Touch _touch)
    {
        var screenPosition = Camera.main.ScreenToViewportPoint(_touch.rawPosition).x;

        if (screenPosition < 0.5f)
        {
            leftAnalog.position = _touch.position;

            if ((leftAnalog.position - leftZone.position).sqrMagnitude > 40 * 40)
            {
                leftZone.position += (leftAnalog.position - leftZone.position) * 0.2f;
            }
            transform.localRotation = Quaternion.Euler(0, Camera.main.transform.rotation.eulerAngles.y, 0);

            speed = Direction().magnitude;

            WalkAnimation();
        }

        if (screenPosition > 0.5f)
        {

            thirdPerson.m_XAxis.Value += _touch.deltaPosition.x * Time.deltaTime * cameraSensitivityX;
            thirdPerson.m_YAxis.Value -= _touch.deltaPosition.y * Time.deltaTime * cameraSensitivityY;
        }
    }
    public void TouchEnd(Touch _touch)
    {

        if (_touch.phase == TouchPhase.Ended || _touch.phase == TouchPhase.Canceled)
        {
            if (Camera.main.ScreenToViewportPoint(_touch.rawPosition).x < 0.5f)
            {
                leftAnalog.gameObject.SetActive(false);
                leftZone.gameObject.SetActive(false);
                speed = 0f;

                WalkAnimation();

                leftID = -1;
            }
            else
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

    public void FetchConfig()
    {
        ConfigManager.FetchConfigs(new userAttributes(), new appAttributes());
    }


    #region MobileDebug
    private bool toggleDebug = false;
    [SerializeField] private GameObject debugMenu = null;
    float delay = 0;

    private void ToggleDebug()
    {
        if (toggleDebug)
        {
            debugMenu.SetActive(false);
            toggleDebug = false;
        }
        else
        {
            debugMenu.SetActive(true);
            toggleDebug = true;
        }
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

        var screenNormal = Camera.main.ScreenToViewportPoint(Input.mousePosition).x;

        if (screenNormal < 0.5f)
        {
            leftAnalog.transform.position = currentPos;
            leftZone.transform.position = currentPos - Vector3.up * leftAnalog.GetComponent<RectTransform>().rect.height * 0.5f;

            leftAnalog.gameObject.SetActive(true);
            leftZone.gameObject.SetActive(true);

            leftID = 0;
        }
        else if (screenNormal > 0.5f)
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
        if (rightID == 0)
        {
            thirdPerson.m_XAxis.Value -= deltaPos.x * Time.deltaTime * 10f;
            thirdPerson.m_YAxis.Value -= deltaPos.y * Time.deltaTime * 0.5f;
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
