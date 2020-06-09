using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.RemoteConfig;

public class CompareDistance : MonoBehaviour
{
    [SerializeField] private Transform target = null;
    [SerializeField] private float distanceMelee = 5f;
    [SerializeField] private float distanceRange = 5f;

    private void Awake()
    {
        ConfigManager.FetchCompleted += UpdateConfig;
    }

    void Start()
    {
        EntityTracker_Enemy.Instance.EnemyList.Add(this.transform);
    }

    void Update()
    {
        if ((transform.position - target.position).sqrMagnitude < distanceMelee * distanceMelee)
        {
            GetComponent<MeshRenderer>().material.SetColor("_BaseColor", Color.green);
        }
        else if ((transform.position - target.position).sqrMagnitude < distanceRange * distanceRange)
        {
            GetComponent<MeshRenderer>().material.SetColor("_BaseColor", Color.red);
        }
        else
        {
            GetComponent<MeshRenderer>().material.SetColor("_BaseColor", Color.grey);
        }
    }

    private void UpdateConfig(ConfigResponse response)
    {
        distanceMelee = ConfigManager.appConfig.GetFloat(nameof(distanceMelee));
        distanceRange = ConfigManager.appConfig.GetFloat(nameof(distanceRange));
    }

    private void OnDestroy()
    {
        EntityTracker_Enemy.Instance.EnemyList.Remove(this.transform);
    }
}
