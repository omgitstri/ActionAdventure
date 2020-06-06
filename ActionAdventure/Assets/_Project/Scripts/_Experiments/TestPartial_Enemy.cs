using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class TestPartial
{
    //imagine this is the stats
    [System.Serializable]
    public struct Stats
    {
        public float hp;
        public float mp;
        public float atk;
    }

    public Stats stat;

    public void Rawr()
    {

    }
}
