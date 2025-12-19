using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.RemoteConfig;

public class DynamicConfig : MonoBehaviour
{
    private static DynamicConfig _instance;
    public static DynamicConfig Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new GameObject(nameof(DynamicConfig)).AddComponent<DynamicConfig>();
                DontDestroyOnLoad(_instance);
            }
            return _instance;
        }
    }

    struct userAttributes { };
    struct appAttributes { };

    public UI_PlayerController PlayerController { get; private set; }

    public void SetPlayerController(UI_PlayerController _PlayerController)
    {
        PlayerController = _PlayerController;
    }


    // private void Awake()
    // {
    //     ConfigManager.FetchCompleted += SetPlayerConfig;
    //     //FetchConfig();
    // }
    //
    // public void FetchConfig(MonoBehaviour test)
    // {
    //     ConfigManager.FetchConfigs(new userAttributes(), new appAttributes());
    // }
    //
    // public void SetPlayerConfig(ConfigResponse response)
    // {
    //
    // }
}
