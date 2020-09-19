using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton : MonoBehaviour
{
    private static Singleton _instance;
    public static Singleton Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new GameObject(nameof(Singleton)).AddComponent<Singleton>();
                DontDestroyOnLoad(_instance);
            }
            return _instance;
        }
    }

    private int handgunAmmo = 0;
    public int HandgunAmmo => handgunAmmo;

    public void AddAmmo(int increment)
    {
        handgunAmmo += increment;
    }
}
