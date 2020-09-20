using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.RemoteConfig;


public class CharacterAction : MonoBehaviour
{
    private enum CharacterStates { Draw, Armed };
    private enum CharacterStats { Speed, Vertical, Horizontal };
    private enum CharacterActions { Attack };

    private Animator _animator;

    private float currentSpeed = 0.0f;


    //config
    private float speedBlendTime = 0.1f;

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

    public void Attack()
    {
        _animator.SetTrigger(CharacterActions.Attack.ToString());
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
        speedBlendTime = ConfigManager.appConfig.GetFloat(nameof(speedBlendTime));
    }

    #endregion
}
