using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Unity.RemoteConfig;

public class CharacterMouseSimulationInput : MonoBehaviour
{
    private CharacterAction _characterAction = null;
    private Camera _mainCamera = null;

    //[SerializeField] Transform leftZone = null;
    //[SerializeField] Transform leftAnalog = null;

    [SerializeField] private CinemachineFreeLook _thirdPerson = null;
    [SerializeField] private CinemachineFreeLook _overShoulder = null;

    //private bool isOverShoulder = false;
    private Vector2 _rotateAxis = Vector2.zero;

    private Animator _animator = null;

    private int _leftID = -1;
    private int _rightID = -1;

    private int _tapCount = 0;

    private float _doubleTapTimer = 0f;
    private float _dragTimer = 0f;

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
        _animator = GetComponentInChildren<Animator>();
        _characterAction = GetComponent<CharacterAction>();
        _mainCamera = Camera.main;
    }

    private void Start()
    {
        _animator.SetBool("Armed", true);

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
        counter.text = _tapCount.ToString();


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
            var screenPosition = _mainCamera.ScreenToViewportPoint(_touch.rawPosition).x;

            if (screenPosition < 0.5f)
            {
                OnScreenUI.Instance.EnableJoystick(_touch.position);
            }
            else if (screenPosition > 0.5f)
            {
                StartDoubleTapTracker();
            }
        }
    }

    public void TouchDrag(Touch _touch)
    {
        var screenPosition = _mainCamera.ScreenToViewportPoint(_touch.rawPosition).x;
        _dragTimer += Time.deltaTime;

        if (screenPosition < 0.5f)
        {
            OnScreenUI.Instance.UpdateJoystick(_touch.position);

            transform.localRotation = Quaternion.Slerp(transform.localRotation, Quaternion.Euler(0, _mainCamera.transform.rotation.eulerAngles.y, 0), 0.3f);

            WalkAnimation();
        }

        if (screenPosition > 0.5f)
        {
            if (_animator.GetBool("Draw"))
            {
                if (_dragTimer > dragThreshold)
                {
                    _doubleTapTimer = -1;
                    _tapCount = 0;
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
                _thirdPerson.m_XAxis.Value += _touch.deltaPosition.x * Time.deltaTime * cameraSensitivityX;
                _thirdPerson.m_YAxis.Value -= _touch.deltaPosition.y * Time.deltaTime * cameraSensitivityY;
            }
        }
    }

    public void TouchEnd(Touch _touch)
    {

        if (_touch.phase == TouchPhase.Ended || _touch.phase == TouchPhase.Canceled)
        {
            if (_mainCamera.ScreenToViewportPoint(_touch.rawPosition).x < 0.5f)
            {

                OnScreenUI.Instance.DisableJoystick();

                WalkAnimation();
            }
            else
            {

            }
        }
    }

    private void DoubleTapTimer()
    {
        if (_doubleTapTimer >= 0 && _doubleTapTimer < doubleTapThreshold)
        {
            _doubleTapTimer += Time.deltaTime;
        }
        else if (_doubleTapTimer > doubleTapThreshold)
        {
            if (_animator.GetBool("Draw"))
            {
                _animator.SetTrigger("Attack");
            }
            _tapCount = 0;
            _doubleTapTimer = -1f;
        }
    }

    private void StartDoubleTapTracker()
    {
        _dragTimer = 0;

        if (_doubleTapTimer > doubleTapThreshold || _doubleTapTimer < 0)
        {
            _doubleTapTimer = 0f;
            _tapCount = 1;
        }
        else
        {
            _tapCount++;

            if (_tapCount >= 2)
            {
                //if (animator.GetBool("Range"))
                //{
                if (_animator.GetBool("Draw"))
                {
                    _animator.SetBool("Draw", false);

                    transform.localRotation = Quaternion.Euler(0, transform.localEulerAngles.y, 0);

                    //thirdPerson.enabled = true;
                    _overShoulder.enabled = false;

                }
                else
                {
                    _doubleTapTimer = -1;
                    _animator.SetBool("Draw", true);
                    transform.localRotation = Quaternion.Euler(0, _mainCamera.transform.rotation.eulerAngles.y, 0);
                    //thirdPerson.enabled = false;
                    _overShoulder.enabled = true;

                }
                //}
            }
        }

    }


    private void WalkAnimation()
    {
        _characterAction.Move(OnScreenUI.Instance.Direction());

        //animator.SetFloat("Horizontal", Direction().x);
        //animator.SetFloat("Vertical", Direction().y);
        //animator.SetFloat("Speed", currentSpeed * 2f);
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

        var screenNormal = _mainCamera.ScreenToViewportPoint(Input.mousePosition).x;

        if (screenNormal < 0.5f)
        {
            OnScreenUI.Instance.EnableJoystick(currentPos);
            //leftAnalog.transform.position = currentPos;
            //leftZone.transform.position = currentPos - Vector3.up * leftAnalog.GetComponent<RectTransform>().rect.height;

            //leftAnalog.gameObject.SetActive(true);
            //leftZone.gameObject.SetActive(true);

            _leftID = 0;
        }
        else if (screenNormal > 0.5f)
        {
            _rightID = 0;

            StartDoubleTapTracker();
        }
    }

    private void MouseDrag()
    {
        deltaPos = currentPos - Input.mousePosition;
        _dragTimer += Time.deltaTime;


        if (_leftID == 0)
        {
            OnScreenUI.Instance.UpdateJoystick(Input.mousePosition);
            transform.localRotation = Quaternion.Slerp(transform.localRotation, Quaternion.Euler(0, _mainCamera.transform.rotation.eulerAngles.y, 0), 0.1f);

            WalkAnimation();
        }
        if (_rightID == 0)
        {
            if (_animator.GetBool("Draw"))
            {
                if (_dragTimer > dragThreshold)
                {
                    _doubleTapTimer = -1;
                    _tapCount = 0;
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
                _thirdPerson.m_XAxis.Value -= deltaPos.x * Time.deltaTime * cameraSensitivityX;
                _thirdPerson.m_YAxis.Value -= deltaPos.y * Time.deltaTime * cameraSensitivityY;
            }
        }

        currentPos = Input.mousePosition;
    }

    private void MouseUp()
    {
        OnScreenUI.Instance.DisableJoystick();

        WalkAnimation();

        _leftID = -1;
        _rightID = -1;

    }
    #endregion
}
