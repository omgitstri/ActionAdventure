using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GetSliderValue : MonoBehaviour
{
    public Animator animator = null;
    private Slider slider = null;

    void Awake()
    {
        slider = GetComponent<Slider>();
    }

    public void ValueChanged()
    {
        animator.SetFloat("Speed", slider.value);
    }
}
