using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class testcomponent : MonoBehaviour
{
    RectTransform rectTransform = null;
    public float xMin = 0;
    public float xMax = 0;
    public float yMin = 0;
    public float yMax = 0;

    public Vector3 offset = Vector3.zero;
    // Start is called before the first frame update
    void Start()
    {
        rectTransform = GetComponent<RectTransform>();

        var screenRect = new Rect(0f, 0f, Camera.main.pixelWidth, Camera.main.pixelHeight);
        var min = (Vector2)rectTransform.position - (Vector2.Scale(rectTransform.rect.size, rectTransform.lossyScale) * 0.5f);
        var max = (Vector2)rectTransform.position + (Vector2.Scale(rectTransform.rect.size, rectTransform.lossyScale) * 0.5f);
        Debug.Log(max);



        if (min.x < 0)
            rectTransform.position -= new Vector3(min.x, 0, 0);
        else if (max.x > screenRect.width)
            rectTransform.position -= new Vector3(max.x - screenRect.width, 0, 0);



        //foreach (var corner in pointerCorners)
        //{
        //    // screen does not contain pointer corner
        //    if (!screenRect.Contains(corner))
        //    {
        //        // move element to the left a bit
        //        rectTransform.localPosition += new Vector3(-50, 0, 0);
        //        break;
        //    }
        //}




    }

    // Update is called once per frame
    void Update()
    {

        //Vector3[] v = new Vector3[4];
        //rectTransform.GetWorldCorners(v);

        ////Bottom Left
        //Debug.Log(v[0]);
        //if (Camera.main.ScreenToViewportPoint(img.rect.min).x < 0)
        //    Debug.Log("out of screen");
    }
}
