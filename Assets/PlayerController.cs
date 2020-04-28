using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    Animator animator = null;
    bool crouch = false;
    

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }


    public void OnMove(InputValue value)
    {
        Vector2 inputMovement = value.Get<Vector2>();
        animator.SetFloat("Horizontal", inputMovement.x);
        animator.SetFloat("Vertical", inputMovement.y);
    }

    public void OnCrouch()
    {
        if(crouch)
        {
            crouch = false;
        }
        else
        {
            crouch = true;
        }
        animator.SetBool("Crouch", crouch);
    }

    public void OnRun(InputValue value)
    {
        float speed = value.Get<float>() + 1f;
        
        animator.SetFloat("Speed", speed);
    }

    public void OnDodge()
    {
        animator.SetTrigger("Dodge");
    }
}
