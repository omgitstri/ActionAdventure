using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestSwapValue : MonoBehaviour
{
    public List<int> a = new List<int>();
    public List<int> b = new List<int>();

    // Start is called before the first frame update
    void Start()
    {
        (a, b) = (b, a);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
