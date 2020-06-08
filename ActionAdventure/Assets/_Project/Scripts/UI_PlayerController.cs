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

    private int tapCount = 0;

    private float currentSpeed = 0f;
    private float targetedSpeed = 0f;
    private float doubleTapTimer = 0f;

    //config
    private float speedBlendTime = 0.1f;
    private float cameraSensitivityX = 3f;
    private float cameraSensitivityY = 0.1f;
    private float doubleTapThreshold = 0.5f;


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
        counter.text = tapCount.ToString();

        if (EntityTracker_Enemy.Instance.AreEnemiesInRange(transform.position, 10f))
        {
            overShoulder.enabled = false;
            animator.SetBool("Armed", false);
            //animator.SetBool("Draw", false);
        }
        else
        {
            animator.SetBool("Armed", true);
            //animator.SetBool("Draw", true);
        }

        if (currentSpeed != targetedSpeed)
        {
            currentSpeed = Mathf.MoveTowards(currentSpeed, targetedSpeed, speedBlendTime * Time.deltaTime);
            animator.SetFloat("Speed", currentSpeed * 2f);
        }

#if UNITY_EDITOR
        MouseInput();
        DoubleTapTimer();
#else
        DoubleTapTimer();
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
                leftAnalog.gameObject.SetActive(true);
                leftZone.gameObject.SetActive(true);
                leftAnalog.transform.position = _touch.position;

                leftZone.transform.position = _touch.position - Vector2.up * leftAnalog.GetComponent<RectTransform>().rect.height;
            }
            else if (screenPosition > 0.5f)
            {
                StartDoubleTapTracker();
            }
        }
    }

    public void TouchDrag(Touch _touch)
    {
        var screenPosition = Camera.main.ScreenToViewportPoint(_touch.rawPosition).x;

        if (screenPosition < 0.5f)
        {
            leftAnalog.position = _touch.position;

            if ((leftAnalog.position - leftZone.position).sqrMagnitude > leftZone.GetComponent<RectTransform>().rect.height * leftZone.GetComponent<RectTransform>().rect.height)
            {
                leftZone.position = Vector3.Lerp(leftZone.position, leftZone.position + (leftAnalog.position - leftZone.position) * 0.5f, 0.3f);
            }

            transform.localRotation = Quaternion.Slerp(transform.localRotation, Quaternion.Euler(0, Camera.main.transform.rotation.eulerAngles.y, 0), 0.3f);

            targetedSpeed = Direction().magnitude / leftAnalog.GetComponent<RectTransform>().rect.width;

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
                targetedSpeed = 0f;

                WalkAnimation();
            }
            else
            {

            }
        }
    }

    private void DoubleTapTimer()
    {
        if (doubleTapTimer >= 0 && doubleTapTimer < doubleTapThreshold)
        {
            doubleTapTimer += Time.deltaTime;
        }
        else
        {
            tapCount = 0;
            doubleTapTimer = -1f;
        }
    }

    private void StartDoubleTapTracker()
    {
        if (doubleTapTimer > doubleTapThreshold || doubleTapTimer < 0)
        {
            doubleTapTimer = 0f;
            tapCount = 1;
        }
        else
        {
            tapCount++;

            if (tapCount >= 2)
            {
                if (animator.GetBool("Draw"))
                {
                    animator.SetBool("Draw", false);
                }
                else
                {
                    animator.SetBool("Draw", true);
                }
                //if (animator.GetBool("Armed"))
                //{
                //    animator.SetBool("Armed", false);
                //    //animator.SetBool("Draw", false);

                //    //thirdPerson.enabled = true;
                //    //overShoulder.enabled = false;
                //}
                //else
                //{
                //    animator.SetBool("Armed", true);
                //    //animator.SetBool("Draw", true);
                //    //thirdPerson.enabled = true;
                //    //overShoulder.enabled = false;
                //}
            }
        }

    }


    private void WalkAnimation()
    {
        animator.SetFloat("Horizontal", Direction().x);
        animator.SetFloat("Vertical", Direction().y);
        animator.SetFloat("Speed", currentSpeed * 2f);
    }

    private Vector2 Direction()
    {
        return (leftAnalog.position - leftZone.position);
    }

    #region RemoteConfig
    struct userAttributes { };
    struct appAttributes { };

    private void UpdateConfig(ConfigResponse response)
    {
        doubleTapThreshold  = ConfigManager.appConfig.GetFloat(nameof(doubleTapThreshold));
        speedBlendTime      = ConfigManager.appConfig.GetFloat(nameof(speedBlendTime));
        cameraSensitivityX  = ConfigManager.appConfig.GetFloat(nameof(cameraSensitivityX));
        cameraSensitivityY  = ConfigManager.appConfig.GetFloat(nameof(cameraSensitivityY));
    }

    public void FetchConfig()
    {
        ConfigManager.FetchConfigs(new userAttributes(), new appAttributes());
    }
    #endregion

    #region Debug
    private bool toggleDebug = false;
    [SerializeField] private GameObject debugMenu = null;
    float delay = 0;

    [SerializeField] UnityEngine.UI.Text counter = null;

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
        if (Input.GetMouseButtonDown(1))
        {
            ToggleDebug();
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
            leftZone.transform.position = currentPos - Vector3.up * leftAnalog.GetComponent<RectTransform>().rect.height;

            leftAnalog.gameObject.SetActive(true);
            leftZone.gameObject.SetActive(true);

            leftID = 0;
        }
        else if (screenNormal > 0.5f)
        {
            rightID = 0;

            StartDoubleTapTracker();
        }
    }

    private void MouseDrag()
    {
        deltaPos = currentPos - Input.mousePosition;
        if (currentSpeed != 0f)
        {
            transform.localRotation = Quaternion.Slerp(transform.localRotation, Quaternion.Euler(0, Camera.main.transform.rotation.eulerAngles.y, 0), 0.1f);
        }

        if (leftID == 0)
        {
            leftAnalog.position = Input.mousePosition;

            if ((leftAnalog.position - leftZone.position).sqrMagnitude > leftAnalog.GetComponent<RectTransform>().rect.height * leftAnalog.GetComponent<RectTransform>().rect.height)
            {
                leftZone.position = Vector3.Lerp(leftZone.position, leftZone.position + (leftAnalog.position - leftZone.position) * 0.5f, 0.3f);
            }

            targetedSpeed = Direction().magnitude / leftAnalog.GetComponent<RectTransform>().rect.width;

            WalkAnimation();
        }
        if (rightID == 0)
        {
            thirdPerson.m_XAxis.Value -= deltaPos.x * Time.deltaTime * cameraSensitivityX;
            thirdPerson.m_YAxis.Value -= deltaPos.y * Time.deltaTime * cameraSensitivityY;
        }

        currentPos = Input.mousePosition;
    }

    private void MouseUp()
    {
        leftAnalog.gameObject.SetActive(false);
        leftZone.gameObject.SetActive(false);
        targetedSpeed = 0f;

        WalkAnimation();

        leftID = -1;
        rightID = -1;

    }
    #endregion
}
