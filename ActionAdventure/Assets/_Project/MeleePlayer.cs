using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class MeleePlayer : MonoBehaviour
{

    SmearEffect[] smearEffects = new SmearEffect[] { };
    float smearDuration = -10f;
    Animator animator = null;
    float speed = 1;

    [SerializeField] CinemachineFreeLook vcam = null;

    private void Awake()
    {
        animator = GetComponent<Animator>();

        smearEffects = GetComponentsInChildren<SmearEffect>();

    }

    private void Start()
    {

        foreach (var item in smearEffects)
        {
            item.ResetAll();
        }
        smearDuration = -10;
    }

    void Update()
    {
        var dir = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));

        if(Input.GetKeyDown(KeyCode.LeftShift))
        {
            speed = 2;
        }
        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            speed = 1;
        }
        if (dir != Vector3.zero)
        {
            transform.localRotation = Quaternion.Slerp(transform.localRotation, Quaternion.Euler(0, Camera.main.transform.rotation.eulerAngles.y, 0), 0.1f);

            animator.SetFloat("Horizontal", dir.x);
            animator.SetFloat("Vertical", dir.z);
            //transform.position += (transform.forward + dir) * 6 * Time.deltaTime;
        }
        animator.SetFloat("Speed", dir.normalized.magnitude * speed);



        if (Input.GetKeyDown(KeyCode.Space) && smearDuration == -10)
        {
            foreach (var item in smearEffects)
            {
                item.smearMat.SetVector("_CurrentPosition", transform.position);
                item.smearMat.SetVector("_PreviousPosition", transform.position);
                item.enabled = true;
            }

            smearDuration = 0.25f;


            GetComponent<Rigidbody>().AddRelativeForce(dir * 5000, ForceMode.Force);
            //transform.position += transform.right + dir * 2;
        }

        if (smearDuration > 0)
        {
            smearDuration -= Time.deltaTime;
        }
        else if (smearDuration > -10 && smearDuration < 0)
        {
            foreach (var item in smearEffects)
            {
                item.ResetAll();
            }
            smearDuration = -10;
        }

    }
}
