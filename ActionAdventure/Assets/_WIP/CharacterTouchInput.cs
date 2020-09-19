using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.RemoteConfig;

public class CharacterTouchInput : MonoBehaviour
{
    private int tapCount = 0;
    private float doubleTapTimer = 0f;

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
            StartDoubleTapTracker();
        }
    }

    public void TouchDrag(Touch _touch)
    {

    }

    public void TouchEnd(Touch _touch)
    {
        if (_touch.phase == TouchPhase.Ended || _touch.phase == TouchPhase.Canceled)
        {

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
