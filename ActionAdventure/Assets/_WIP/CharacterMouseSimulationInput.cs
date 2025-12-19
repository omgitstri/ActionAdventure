using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.RemoteConfig;

public class CharacterMouseSimulationInput : MonoBehaviour
{
    private CharacterAction _characterAction = null;
    private Camera _mainCamera = null;

    private int _leftID = -1;
    private int _rightID = -1;

    private int _tapCount = 0;

    private float _doubleTapTimer = 0f;
    private float _dragTimer = 0f;

    //config
    private float aimSensitivityX = 3f;
    private float aimSensitivityY = 0.1f;

    private float doubleTapThreshold = 0.5f;
    private float dragThreshold = 0.25f;

    private void Awake()
    {
        _characterAction = GetComponent<CharacterAction>();
        _mainCamera = Camera.main;
    }

    private void Start()
    {
        //temp
        _characterAction.ToggleWeapon();
        //temp

        // ConfigManager.FetchCompleted += UpdateConfig;
        // FetchConfig();
    }

    void Update()
    {
        OnScreenUI.Instance.DebugUpdateTapCount(_tapCount.ToString());

        DoubleTapTimer();

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

    public void TouchBegin(Touch touch)
    {
        if (touch.phase == TouchPhase.Began)
        {
            var screenPosition = _mainCamera.ScreenToViewportPoint(touch.rawPosition).x;

            if (screenPosition < 0.5f)
            {
                OnScreenUI.Instance.EnableJoystick(touch.position);
            }
            else if (screenPosition > 0.5f)
            {
                StartDoubleTapTracker();
            }
        }
    }

    public void TouchDrag(Touch touch)
    {
        var screenPosition = _mainCamera.ScreenToViewportPoint(touch.rawPosition).x;
        _dragTimer += Time.deltaTime;

        if (screenPosition < 0.5f)
        {
            OnScreenUI.Instance.UpdateJoystick(touch.position);

            transform.localRotation = Quaternion.Slerp(transform.localRotation, Quaternion.Euler(0, _mainCamera.transform.rotation.eulerAngles.y, 0), 0.3f);

            _characterAction.Move(OnScreenUI.Instance.Direction());
        }

        if (screenPosition > 0.5f)
        {
            if (_dragTimer > dragThreshold)
            {
                _doubleTapTimer = -1;
                _tapCount = 0;
            }

            _characterAction.Aim(touch.deltaPosition, aimSensitivityX, aimSensitivityY);
        }
    }

    public void TouchEnd(Touch touch)
    {

        if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
        {
            if (_mainCamera.ScreenToViewportPoint(touch.rawPosition).x < 0.5f)
            {

                OnScreenUI.Instance.DisableJoystick();

                _characterAction.Move(OnScreenUI.Instance.Direction());
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

            _characterAction.Attack();

            _tapCount = 0;
            _doubleTapTimer = -1f;
        }
    }

    private void StartDoubleTapTracker()
    {
        _dragTimer = 0.0f;

        if (_doubleTapTimer > doubleTapThreshold || _doubleTapTimer < 0.0f)
        {
            _doubleTapTimer = 0.0f;
            _tapCount = 1;
        }
        else
        {
            _tapCount++;

            if (_tapCount >= 2)
            {
                _tapCount = 0;
                //switch camera
                if (!_characterAction.GetToggleThirdCamera(_mainCamera))
                {
                    _doubleTapTimer = -1.0f;
                }
            }
        }
    }

    //
    // #region RemoteConfig
    // struct userAttributes { };
    // struct appAttributes { };

    // private void UpdateConfig(ConfigResponse response)
    // {
    //     aimSensitivityX = ConfigManager.appConfig.GetFloat(nameof(aimSensitivityX));
    //     aimSensitivityY = ConfigManager.appConfig.GetFloat(nameof(aimSensitivityY));
    //
    //     doubleTapThreshold = ConfigManager.appConfig.GetFloat(nameof(doubleTapThreshold));
    //     dragThreshold = ConfigManager.appConfig.GetFloat(nameof(dragThreshold));
    // }
    //
    // public void FetchConfig()
    // {
    //     ConfigManager.FetchConfigs(new userAttributes(), new appAttributes());
    // }
    // #endregion



    #region PC Debug
    //debug
    Vector3 deltaPos = Vector3.zero;
    Vector3 currentPos = Vector3.zero;

    private void MouseInput()
    {
        if (Input.GetMouseButtonDown(1))
        {
            OnScreenUI.Instance.ToggleDebug();
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

            _characterAction.Move(OnScreenUI.Instance.Direction());
        }
        if (_rightID == 0)
        {
            if (_dragTimer > dragThreshold)
            {
                _doubleTapTimer = -1;
                _tapCount = 0;
            }

            _characterAction.Aim(deltaPos, aimSensitivityX, aimSensitivityY);
        }

        currentPos = Input.mousePosition;
    }

    private void MouseUp()
    {
        OnScreenUI.Instance.DisableJoystick();

        _characterAction.Move(OnScreenUI.Instance.Direction());

        _leftID = -1;
        _rightID = -1;

    }
    #endregion
}
