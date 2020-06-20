using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[ExecuteAlways]
public class SmearEffect : MonoBehaviour
{
    Queue<Vector3> _recentPositions = new Queue<Vector3>();

    [SerializeField]
    int _frameLag = 0;

    Material _smearMat = null;
    public Material smearMat
    {
        get
        {
            if (!_smearMat)
                //_smearMat = GetComponent<Renderer>().sharedMaterial;
                _smearMat = GetComponent<Renderer>().material;

            //if (!_smearMat.HasProperty("_PrevPosition"))
            //    _smearMat.shader = Shader.Find("Custom/Smear");

            return _smearMat;
        }
    }

    void LateUpdate()
    {
        if (_recentPositions.Count > _frameLag)
        {
            smearMat.SetVector("_PreviousPosition", _recentPositions.Dequeue());
        }

        smearMat.SetVector("_CurrentPosition", transform.position);
        _recentPositions.Enqueue(transform.position);
    }

    private void OnEnable()
    {
        smearMat.SetVector("_CurrentPosition", transform.position);
        smearMat.SetVector("_PreviousPosition", transform.position);
    }

    public void ResetAll()
    {
        smearMat.SetVector("_CurrentPosition", transform.position);
        smearMat.SetVector("_PreviousPosition", transform.position);
        _recentPositions.Clear();
        enabled = false;
    }


}
