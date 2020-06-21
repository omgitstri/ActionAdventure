using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private CinemachineFreeLook thirdPerson = null;
    [SerializeField] private CinemachineFreeLook overShoulder = null;
    [SerializeField] private Transform spine = null;

    private bool isOverShoulder = false;
    private Vector2 rotateAxis = Vector2.zero;

    private Animator animator = null;
    private PlayerInput playerInput = null;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        playerInput = GetComponent<PlayerInput>();
    }

    private void Update()
    {
        if (EntityTracker_Enemy.Instance.AreEnemiesInRange(transform.position, 10f))
        {
            animator.SetBool("Armed", true);
            //animator.SetBool("Draw", true);
        }
        else
        {
            overShoulder.enabled = false;
            animator.SetBool("Armed", false);
            animator.SetBool("Draw", false);
        }

        if (isOverShoulder)
        {
            OnRotate();
        }
    }

    public void OnMove(InputValue value)
    {
        Vector2 inputMovement = value.Get<Vector2>();

        animator.SetFloat("Horizontal", inputMovement.x * 2);
        animator.SetFloat("Vertical", inputMovement.y * 2);
    }

    //public void OnCrouch()
    //{
    //    if (animator.GetBool("Crouch") == true)
    //    {
    //        animator.SetBool("Crouch", false);
    //    }
    //    else
    //    {
    //        animator.SetBool("Crouch", true);
    //    }
    //}

    //public void OnDodge()
    //{
    //    animator.SetTrigger("Dodge");
    //}

    public void OnRotate()
    {
        transform.Rotate(Vector3.up, rotateAxis.x);


        spine.Rotate(Vector3.forward, rotateAxis.y);

        //float _z = Mathf.Clamp(spine.localRotation.eulerAngles.z, -30, 30);
        //spine.localRotation = Quaternion.Euler(spine.localRotation.eulerAngles.x, spine.localRotation.eulerAngles.y, _z);
    }

    public void OnCameraLook(InputValue value)
    {
        Vector2 inputLook = value.Get<Vector2>();

        thirdPerson.m_XAxis.m_InputAxisValue = inputLook.x;
        thirdPerson.m_YAxis.m_InputAxisValue = inputLook.y;

        if (!animator.GetBool("Armed"))
        {
            isOverShoulder = false;
            rotateAxis = Vector2.zero;
        }
        else if (animator.GetBool("Armed") && inputLook != Vector2.zero)
        {
            overShoulder.enabled = true;
            rotateAxis = inputLook;
            isOverShoulder = true;
            animator.SetBool("Draw", true);
        }
        else if (inputLook == Vector2.zero)
        {
            animator.SetTrigger("Attack");
            rotateAxis = Vector2.zero;
        }
    }

    //public void OnToggleWeapon()
    //{
    //    if (animator.GetBool("Armed") == true)
    //    {
    //        animator.SetBool("Armed", false);
    //        animator.SetBool("Draw", false);
    //    }
    //    else
    //    {
    //        animator.SetBool("Armed", true);
    //    }
    //}

    //public void OnAim()
    //{
    //    if (animator.GetBool("Draw"))
    //    {
    //        animator.SetBool("Draw", false);
    //    }
    //    else
    //    {
    //        animator.SetBool("Armed", true);
    //        animator.SetBool("Draw", true);
    //    }
    //}

    //public void OnAttack()
    //{
    //    animator.SetTrigger("Attack");
    //    animator.SetBool("Armed", true);
    //    animator.SetBool("Draw", true);
    //}
}
