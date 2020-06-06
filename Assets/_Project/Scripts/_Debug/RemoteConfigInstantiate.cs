using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.RemoteConfig;

public class RemoteConfigInstantiate : MonoBehaviour
{
    [SerializeField] GameObject instantiate = null;

    List<GameObject> enemies = new List<GameObject>();

    struct userAttributes { };
    struct appAttributes { };

    private int enemyCount = 1;

    private void Start()
    {
        ConfigManager.FetchCompleted += InstantiateData;
        //ConfigManager.FetchConfigs(new userAttributes(), new appAttributes());

    }

    void Update()
    {
        ConfigManager.FetchConfigs(new userAttributes(), new appAttributes());
    }

    public void InstantiateData(ConfigResponse response)
    {
        if (enemyCount != ConfigManager.appConfig.GetInt(nameof(enemyCount)))
        {
            foreach (var item in enemies)
            {
                Destroy(item);
            }

            enemies.Clear();
            enemyCount = ConfigManager.appConfig.GetInt(nameof(enemyCount));

            for (int i = 0; i < enemyCount; i++)
            {
                var go = Instantiate(instantiate);
                go.transform.localPosition = Vector3.zero;
                enemies.Add(go);
                go.transform.SetParent(transform);
                go.transform.localPosition = Vector3.right * i * 1.5f;
                go.SetActive(true);

            }
        }
    }
}
