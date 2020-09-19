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

    //private bool isOverShoulder = false;
    private Vector2 rotateAxis = Vector2.zero;

    private Animator animator = null;

    private int leftID = -1;
    private int rightID = -1;

    private int tapCount = 0;

    private float currentSpeed = 0f;
    private float targetedSpeed = 0f;
    private float doubleTapTimer = 0f;
    private float dragTimer = 0f;

    //config
    private float aimSensitivityX = 3f;
    private float aimSensitivityY = 0.1f;
    private float cameraSensitivityX = 3f;
    private float cameraSensitivityY = 0.1f;
    private float speedBlendTime = 0.1f;
    private float doubleTapThreshold = 0.5f;
    private float dragThreshold = 0.25f;
    private float distanceMelee = 10f;
    private float distanceRange = 20f;

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
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

        if (EntityTracker_Enemy.Instance.AreEnemiesInRange(transform.position, distanceMelee))
        {
            //thirdPerson.enabled = true;

            overShoulder.enabled = false;

            animator.SetBool("Range", false);
            animator.SetBool("Draw", false);
        }
        else if (EntityTracker_Enemy.Instance.AreEnemiesInRange(transform.position, distanceRange))
        {
            animator.SetBool("Range", true);
            //animator.SetBool("Draw", true);
        }
        else
        {
            //thirdPerson.enabled = true;

            animator.SetBool("Draw", false);
            overShoulder.enabled = false;
            //animator.SetBool("Range", false);
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
        dragTimer += Time.deltaTime;

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
            if (animator.GetBool("Draw"))
            {
                if (dragTimer > dragThreshold)
                {
                    doubleTapTimer = -1;
                    tapCount = 0;
                }

                //transform.rotation *= Quaternion.Euler(new Vector3(transform.rotation.eulerAngles.x, _touch.deltaPosition.x * Time.deltaTime * aimSensitivityX, transform.rotation.eulerAngles.z));
                transform.Rotate(Vector3.up * _touch.deltaPosition.x * Time.deltaTime * aimSensitivityX);
                transform.Rotate(Vector3.left * _touch.deltaPosition.y * Time.deltaTime * aimSensitivityY);

                //clamp
                if (transform.localEulerAngles.x > 30 && transform.localEulerAngles.x < 50)
                {
                    transform.localEulerAngles = new Vector3(29.9f, transform.localEulerAngles.y, transform.localEulerAngles.z);
                }
                else if (transform.localEulerAngles.x < 330 && transform.localEulerAngles.x > 320)
                {
                    transform.localEulerAngles = new Vector3(331.1f, transform.localEulerAngles.y, transform.localEulerAngles.z);
                }

                transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y, 0);
            }
            else
            {
                thirdPerson.m_XAxis.Value += _touch.deltaPosition.x * Time.deltaTime * cameraSensitivityX;
                thirdPerson.m_YAxis.Value -= _touch.deltaPosition.y * Time.deltaTime * cameraSensitivityY;
            }
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
        else if (doubleTapTimer > doubleTapThreshold)
        {
            if (animator.GetBool("Draw"))
            {
                animator.SetTrigger("Attack");
            }
            tapCount = 0;
            doubleTapTimer = -1f;
        }
    }

    private void StartDoubleTapTracker()
    {
        dragTimer = 0;

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
                if (animator.GetBool("Range"))
                {
                    if (animator.GetBool("Draw"))
                    {
                        animator.SetBool("Draw", false);

                        transform.localRotation = Quaternion.Euler(0, transform.localEulerAngles.y, 0);

                        //thirdPerson.enabled = true;
                        overShoulder.enabled = false;

                    }
                    else
                    {
                        doubleTapTimer = -1;
                        animator.SetBool("Draw", true);
                        transform.localRotation = Quaternion.Euler(0, Camera.main.transform.rotation.eulerAngles.y, 0);
                        //thirdPerson.enabled = false;
                        overShoulder.enabled = true;

                    }
                }
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
        aimSensitivityX = ConfigManager.appConfig.GetFloat(nameof(aimSensitivityX));
        aimSensitivityY = ConfigManager.appConfig.GetFloat(nameof(aimSensitivityY));
        cameraSensitivityX = ConfigManager.appConfig.GetFloat(nameof(cameraSensitivityX));
        cameraSensitivityY = ConfigManager.appConfig.GetFloat(nameof(cameraSensitivityY));

        speedBlendTime = ConfigManager.appConfig.GetFloat(nameof(speedBlendTime));

        doubleTapThreshold = ConfigManager.appConfig.GetFloat(nameof(doubleTapThreshold));
        dragThreshold = ConfigManager.appConfig.GetFloat(nameof(dragThreshold));

        distanceMelee = ConfigManager.appConfig.GetFloat(nameof(distanceMelee));
        distanceRange = ConfigManager.appConfig.GetFloat(nameof(distanceRange));
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
        dragTimer += Time.deltaTime;

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
            if (animator.GetBool("Draw"))
            {
                if (dragTimer > dragThreshold)
                {
                    doubleTapTimer = -1;
                    tapCount = 0;
                }

                transform.Rotate(Vector3.up * -deltaPos.x * Time.deltaTime * aimSensitivityX);
                transform.Rotate(Vector3.left * deltaPos.y * Time.deltaTime * aimSensitivityY);

                //clamp
                if (transform.localEulerAngles.x > 30 && transform.localEulerAngles.x < 50)
                {
                    transform.localEulerAngles = new Vector3(29.9f, transform.localEulerAngles.y, transform.localEulerAngles.z);
                }
                else if (transform.localEulerAngles.x < 330 && transform.localEulerAngles.x > 320)
                {
                    transform.localEulerAngles = new Vector3(331.1f, transform.localEulerAngles.y, transform.localEulerAngles.z);
                }

                transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y, 0);
            }
            else
            {
                thirdPerson.m_XAxis.Value -= deltaPos.x * Time.deltaTime * cameraSensitivityX;
                thirdPerson.m_YAxis.Value -= deltaPos.y * Time.deltaTime * cameraSensitivityY;
            }
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
