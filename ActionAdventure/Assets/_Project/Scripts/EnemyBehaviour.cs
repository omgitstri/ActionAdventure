using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

[ExecuteAlways]
public class EnemyBehaviour : MonoBehaviour
{
    [Range(0, 1)]
    [SerializeField] private float a = 0f;
    [SerializeField] private bool _dodge = false;
    [SerializeField] private bool _damage = false;

    [SerializeField] private Transform hitDirection = null;
    Vector3 collisionPoint = Vector3.zero;

    [SerializeField] CinemachineFreeLook overshoulder;

    void Update()
    {
        //if(!_dodge && !_damage)
        //{
        //    foreach (var item in transform.GetComponentsInChildren<EnemyVisual>(true))
        //    {
        //        item.Start();
        //    }
        //}

        if (_dodge)
        {
            foreach (var item in transform.GetComponentsInChildren<EnemyVisual>())
            {
                item.transform.localPosition = Vector3.LerpUnclamped(item.initPos, item.initPos + item.random, a);
                item.transform.localScale = Vector3.Lerp(Vector3.one * 0.125f, Vector3.zero, a - 0.5f);
                //overshoulder.GetRig(1).GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_FrequencyGain = Mathf.Lerp(0, 1, a);
            }
        }

        if (_damage)
        {
            foreach (var item in transform.GetComponentsInChildren<EnemyVisual>())
            {
                //item.transform.position = Vector3.Lerp(item.initPosWorld, (item.initPosWorld - hitDirection.position).normalized * 3 + item.random2, a);
                item.transform.localPosition = Vector3.Lerp(item.initPos, (item.initPosWorld - collisionPoint).normalized * 3 + item.random2, a);
                item.transform.localScale = Vector3.Lerp(Vector3.one * 0.125f, Vector3.zero, a - 0.2f);

                overshoulder.GetRig(1).GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_FrequencyGain = Mathf.Lerp(2, 0, a);
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        //collisionPoint = collision.GetContact(0).point;
        collisionPoint = collision.transform.position;



        if (Random.value > 0.25f)
        {
            collision.gameObject.SetActive(false);
            GetComponent<Animator>().SetTrigger("Damage");
            //collision.gameObject.SetActive(false);
        }
        else
        {
            GetComponent<Animator>().SetTrigger("Dodge");
        }
    }

    public void ResetAll()
    {
        foreach (var item in transform.GetComponentsInChildren<SmearEffect>())
        {
            item.ResetAll();
        }
    }
    public void Enable()
    {
        foreach (var item in transform.GetComponentsInChildren<EnemyVisual>())
        {
            item.Start();
        }

        foreach (var item in transform.GetComponentsInChildren<SmearEffect>(true))
        {
            item.enabled = true;
        }
    }

    public void Destroy()
    {
        Destroy(gameObject);
    }
}
