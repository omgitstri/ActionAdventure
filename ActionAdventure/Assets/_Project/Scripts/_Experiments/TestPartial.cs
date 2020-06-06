using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class TestPartial : MonoBehaviour
{
    //main script that runs the logic
    public Transform test;

    void Start()
    {
        Rawr();
    }

    void Update()
    {
        Debug.Log(EntityTracker_Enemy.Instance.AreEnemiesInRange(transform.position, 5f));
    }
}
