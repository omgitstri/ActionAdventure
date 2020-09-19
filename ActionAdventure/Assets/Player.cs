using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    void Start()
    {

    }

    [ContextMenu(nameof(AddAmmo))]
    public void AddAmmo()
    {
        Singleton.Instance.AddAmmo(50);
    }
}
