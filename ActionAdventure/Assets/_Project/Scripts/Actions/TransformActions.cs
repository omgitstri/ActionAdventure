using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransformActions : MonoBehaviour
{
    [SerializeField] Vector3 rotation = Vector3.zero;

    private void Start()
    {
        
    }

    public void SetPosition()
    {
        //transform.position = _position;
    }

    public void SetRotation()
    {
        transform.localRotation = Quaternion.Euler(rotation);
    }
}
