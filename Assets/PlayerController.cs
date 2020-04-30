using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;

public class PlayerController : MonoBehaviour
{
    Animator animator = null;
    bool crouch = false;
    public CinemachineFreeLook cam = null;
    public PlayerInput playerInput = null;
    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        transform.rotation = Quaternion.identity;
    }

    public void OnMove(InputValue value)
    {
        Vector2 inputMovement = value.Get<Vector2>();
        //animator.SetFloat("Speed", inputMovement.magnitude + 1f);
        animator.SetFloat("Horizontal", inputMovement.x * 2);
        animator.SetFloat("Vertical", inputMovement.y * 2);
    }

    public void OnCrouch()
    {
        if (crouch)
        {
            crouch = false;
        }
        else
        {
            crouch = true;
        }
        animator.SetBool("Crouch", crouch);
    }

    public void OnDodge()
    {
        animator.SetTrigger("Dodge");
    }

    public void OnCameraLook(InputValue value)
    {
        Vector2 inputMovement = value.Get<Vector2>();

        cam.m_XAxis.m_InputAxisValue = inputMovement.x;
        cam.m_YAxis.m_InputAxisValue = inputMovement.y;
    }
}
