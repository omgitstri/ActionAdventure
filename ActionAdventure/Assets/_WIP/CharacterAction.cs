using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.RemoteConfig;
using Cinemachine;



public class CharacterAction : MonoBehaviour
{
    [SerializeField] private CinemachineFreeLook _thirdPerson = null;
    [SerializeField] private CinemachineFreeLook _overShoulder = null;


    private enum CharacterStates { Draw, Armed };
    private enum CharacterStats { Speed, Vertical, Horizontal };
    private enum CharacterActions { Attack };

    private Animator _animator;

    private float currentSpeed = 0.0f;


    //config
    private float cameraSensitivityX = 3f;
    private float cameraSensitivityY = 0.1f;
    private float speedBlendTime = 0.1f;
    private float distanceMelee = 10f;
    private float distanceRange = 20f;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    private void Start()
    {
        ConfigManager.FetchCompleted += UpdateConfig;
        FetchConfig();
    }

    private void Update()
    {

        //prevent snapping
        if (currentSpeed != OnScreenUI.Instance.TargetedSpeed)
        {
            currentSpeed = Mathf.MoveTowards(currentSpeed, OnScreenUI.Instance.TargetedSpeed, speedBlendTime * Time.deltaTime);
            _animator.SetFloat(CharacterStats.Speed.ToString(), currentSpeed * 2f);
        }
    }

    public void Move(Vector3 direction)
    {
        _animator.SetFloat(CharacterStats.Horizontal.ToString(), direction.x);
        _animator.SetFloat(CharacterStats.Vertical.ToString(), direction.y);
        _animator.SetFloat(CharacterStats.Speed.ToString(), currentSpeed * 2f);
    }

    public void Aim(Vector3 deltaPosition, float aimSensitivityX, float aimSensitivityY)
    {
        if (_animator.GetBool(CharacterStates.Draw.ToString()))
        {
            transform.Rotate(Vector3.up * deltaPosition.x * Time.deltaTime * aimSensitivityX);
            transform.Rotate(Vector3.left * deltaPosition.y * Time.deltaTime * aimSensitivityY);

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
            _thirdPerson.m_XAxis.Value += deltaPosition.x * Time.deltaTime * cameraSensitivityX;
            _thirdPerson.m_YAxis.Value -= deltaPosition.y * Time.deltaTime * cameraSensitivityY;
        }
    }

    public void Attack()
    {
        if (_animator.GetBool(CharacterStates.Draw.ToString()))
        {
            _animator.SetTrigger(CharacterActions.Attack.ToString());
        }
    }

    public bool GetToggleThirdCamera(Camera mainCam)
    {
        bool isThirdCam = false;

        if (_animator.GetBool(CharacterStates.Draw.ToString()))
        {
            _animator.SetBool(CharacterStates.Draw.ToString(), false);

            transform.localRotation = Quaternion.Euler(0, transform.localEulerAngles.y, 0);

            _thirdPerson.enabled = true;
            _overShoulder.enabled = false;

            isThirdCam = true;
        }
        else
        {
            _animator.SetBool(CharacterStates.Draw.ToString(), true);
            transform.localRotation = Quaternion.Euler(0, mainCam.transform.rotation.eulerAngles.y, 0);

            _thirdPerson.enabled = false;
            _overShoulder.enabled = true;

            isThirdCam = false;
        }

        return isThirdCam;
    }

    public void ToggleWeapon()
    {
        if (_animator.GetBool(CharacterStates.Armed.ToString()))
        {
            _animator.SetBool(CharacterStates.Armed.ToString(), false);
        }
        else
        {
            _animator.SetBool(CharacterStates.Armed.ToString(), true);
        }
    }

    public void EquipBow()
    {

    }

    public void DisarmBow()
    {

    }

    #region RemoteConfig
    struct userAttributes { };
    struct appAttributes { };

    public void FetchConfig()
    {
        ConfigManager.FetchConfigs(new userAttributes(), new appAttributes());
    }

    private void UpdateConfig(ConfigResponse response)
    {
        cameraSensitivityX = ConfigManager.appConfig.GetFloat(nameof(cameraSensitivityX));
        cameraSensitivityY = ConfigManager.appConfig.GetFloat(nameof(cameraSensitivityY));

        speedBlendTime = ConfigManager.appConfig.GetFloat(nameof(speedBlendTime));

        distanceMelee = ConfigManager.appConfig.GetFloat(nameof(distanceMelee));
        distanceRange = ConfigManager.appConfig.GetFloat(nameof(distanceRange));
    }

    #endregion
}
