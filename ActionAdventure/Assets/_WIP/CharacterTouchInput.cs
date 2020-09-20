using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.RemoteConfig;

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
    private float cameraSensitivityX = 3f;
    private float cameraSensitivityY = 0.1f;
    private float speedBlendTime = 0.1f;
    private float doubleTapThreshold = 0.5f;
    private float dragThreshold = 0.25f;
    private float distanceMelee = 10f;
    private float distanceRange = 20f;

    private void Awake()
    {
        _characterAction = GetComponent<CharacterAction>();
        _mainCamera = Camera.main;
    }

    private void Start()
    {
        ConfigManager.FetchCompleted += UpdateConfig;
        FetchConfig();
    }

    void Update()
    {
        if (Input.touchCount > 0)
        {
            for (int i = 0; i < Input.touchCount; i++)
            {
                TouchBegin(Input.touches[i]);
                TouchDrag(Input.touches[i]);
                TouchEnd(Input.touches[i]);
            }
        }
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
            transform.localRotation = Quaternion.Slerp(transform.localRotation, Quaternion.Euler(0, _mainCamera.transform.rotation.eulerAngles.y, 0), 0.3f);

            OnScreenUI.Instance.UpdateJoystick(_touch.position);

            _characterAction.Move(OnScreenUI.Instance.Direction());
        }

    }

    public void TouchEnd(Touch _touch)
    {
        if (_touch.phase == TouchPhase.Ended || _touch.phase == TouchPhase.Canceled)
        {
            if (_mainCamera.ScreenToViewportPoint(_touch.rawPosition).x < 0.5f)
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
        else
        {
            _tapCount = 0;
            _doubleTapTimer = -1f;
        }
    }

    private void StartDoubleTapTracker()
    {
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
                //do stuff
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

}
