using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.RemoteConfig;
using System;

public class CharacterTouchInput : MonoBehaviour
{
    private CharacterAction _characterAction = null;
    private Camera _mainCamera = null;

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

#if UNITY_EDITOR
        if (!GetComponent<CharacterMouseSimulationInput>())
        {
            gameObject.AddComponent<CharacterMouseSimulationInput>();
        }
#endif
    }

    private bool TryGetComponent(Type type, out object comp)
    {
        throw new NotImplementedException();
    }

    private void Start()
    {
        //temp
        _characterAction.ToggleWeapon();
        //temp

        ConfigManager.FetchCompleted += UpdateConfig;
        FetchConfig();
    }

    void Update()
    {
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

        //debug
        OnScreenUI.Instance.DebugUpdateTapCount(_tapCount.ToString());
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
            else
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
            transform.localRotation = Quaternion.Slerp(transform.localRotation, Quaternion.Euler(0, _mainCamera.transform.rotation.eulerAngles.y, 0), 0.3f);

            OnScreenUI.Instance.UpdateJoystick(touch.position);

            _characterAction.Move(OnScreenUI.Instance.Direction());
        }
        else
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


    #region RemoteConfig
    struct userAttributes { };
    struct appAttributes { };

    private void UpdateConfig(ConfigResponse response)
    {
        aimSensitivityX = ConfigManager.appConfig.GetFloat(nameof(aimSensitivityX));
        aimSensitivityY = ConfigManager.appConfig.GetFloat(nameof(aimSensitivityY));

        doubleTapThreshold = ConfigManager.appConfig.GetFloat(nameof(doubleTapThreshold));
        dragThreshold = ConfigManager.appConfig.GetFloat(nameof(dragThreshold));
    }

    public void FetchConfig()
    {
        ConfigManager.FetchConfigs(new userAttributes(), new appAttributes());
    }
    #endregion

}
