using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;

public class PlayerController : MonoBehaviour
{
    private Animator animator = null;
    private PlayerInput playerInput = null;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        playerInput = GetComponent<PlayerInput>();
    }

    public void OnMove(InputValue value)
    {
        Vector2 inputMovement = value.Get<Vector2>();

        animator.SetFloat("Horizontal", inputMovement.x * 2);
        animator.SetFloat("Vertical", inputMovement.y * 2);
    }

    public void OnCrouch()
    {
        if (animator.GetBool("Crouch") == true)
        {
            animator.SetBool("Crouch", false);
        }
        else
        {
            animator.SetBool("Crouch", true);
        }
    }

    public void OnDodge()
    {
        animator.SetTrigger("Dodge");
    }

    public void OnCameraLook(InputValue value)
    {
        Vector2 inputMovement = value.Get<Vector2>();
    }

    public void OnToggleWeapon()
    {
        if (animator.GetBool("Armed") == true)
        {
            animator.SetBool("Armed", false);
            animator.SetBool("Draw", false);
        }
        else
        {
            animator.SetBool("Armed", true);
        }
    }

    public void OnAim()
    {
        if (animator.GetBool("Draw"))
        {
            animator.SetBool("Draw", false);
        }
        else
        {
            animator.SetBool("Armed", true);
            animator.SetBool("Draw", true);
        }
    }

    public void OnAttack()
    {
        animator.SetTrigger("Attack");
        animator.SetBool("Armed", true);
        animator.SetBool("Draw", true);
    }
}
