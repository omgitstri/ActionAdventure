using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityTracker_Enemy : MonoBehaviour
{
    private static EntityTracker_Enemy _instance;
    public static EntityTracker_Enemy Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new GameObject(nameof(EntityTracker_Enemy)).AddComponent<EntityTracker_Enemy>();
            }
            return _instance;
        }
    }

    public List<Transform> EnemyList = new List<Transform>();

    public bool AreEnemiesInRange(Vector3 _position, float _distance)
    {
        for (int i = 0; i < EnemyList.Count; i++)
        {
            if ((EnemyList[i].position - _position).sqrMagnitude < _distance * _distance)
            {
                return true;
            }
        }
        return false;
    }

}
