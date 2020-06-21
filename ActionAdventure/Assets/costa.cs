using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class costa : MonoBehaviour
{
    int _currentAmmo = 0;
    float _canFire = 0f;
    bool _reloading = false;

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && CanFire())
        {

        }
    }


    bool CanFire()
    {
        bool _result = false;

        if (_currentAmmo > 0 && Time.time >= _canFire && _reloading == false)
        {
            _result = true;
        }
        else
        {
            _result = false;
        }

        return _result;
    }
}
