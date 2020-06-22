using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdjustMass : MonoBehaviour
{
    [SerializeField] Vector3 localMass = Vector3.zero;
    Rigidbody rigidbody = null;
    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    void Start()
    {
        rigidbody.centerOfMass = Vector3.up + localMass;
    }
}
