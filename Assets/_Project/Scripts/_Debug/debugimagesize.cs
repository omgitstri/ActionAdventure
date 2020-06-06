using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class debugimagesize : MonoBehaviour
{
    RectTransform _parent = null;
    Text txt = null;

    private void Awake()
    {
        _parent = transform.parent.GetComponent<RectTransform>();
        txt = GetComponent<Text>();
    }

    void Update()
    {
        txt.text = _parent.rect.width.ToString();
    }
}
