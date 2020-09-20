using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class propertyblocks : MonoBehaviour
{
    public MeshRenderer meshRender;
    public Material material;
    private MaterialPropertyBlock block;

    // Start is called before the first frame update
    void Start()
    {
        block = new MaterialPropertyBlock();

        meshRender.material.SetFloat("_hue", 100);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
