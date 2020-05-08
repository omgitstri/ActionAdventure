using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class Cone : MonoBehaviour
{
    [Range(0.5f, 360f)]
    public float increments = 5f;
    public float topRadius = 5;
    public float bottomRadius = 5;
    public float height = 2;
    Quaternion rot = Quaternion.identity;
    public List<Vector3> circle1 = new List<Vector3>();
    public List<Vector3> circle2 = new List<Vector3>();
    public List<Vector3> circle3 = new List<Vector3>();

    List<Vector3> vertices = new List<Vector3>();
    List<int> triangles = new List<int>();
    Mesh mesh = null;

    private void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;


    }

    private void UpdateMesh()
    {
        mesh.Clear();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
    }

    private void CreateShape()
    {
        vertices.Clear();
        triangles.Clear();

        for (int i = 0; i < circle3.Count - 1; i++)
        {
            vertices.Add(circle3[i]);
            vertices.Add(circle3[i + 1]);
            vertices.Add(circle2[i]);
            vertices.Add(circle2[i + 1]);
        }

        vertices.Add(circle3[circle3.Count - 1]);
        vertices.Add(circle3[0]);
        vertices.Add(circle2[circle3.Count - 1]);
        vertices.Add(circle2[0]);

        for (int i = 0; i < vertices.Count / 4; i++)
        {
            triangles.Add(i * 4);
            triangles.Add(i * 4 + 1);
            triangles.Add(i * 4 + 2);

            triangles.Add(i * 4 + 1);
            triangles.Add(i * 4 + 3);
            triangles.Add(i * 4 + 2);
        }

        vertices.Add(transform.position);

        for (int i = 0; i < vertices.Count - 1; i++)
        {
            triangles.Add(vertices.Count - 1);
            triangles.Add(i);
            triangles.Add(i + 1);
        }
    }

    void Update()
    {
        circle1.Clear();
        circle2.Clear();
        circle3.Clear();

        for (int i = 0; i < 360 / increments; i++)
        {
            rot = Quaternion.AngleAxis(increments * i, Vector3.up);
            circle1.Add(transform.position + rot * Vector3.forward * bottomRadius);
            circle2.Add((transform.position + rot * Vector3.forward * topRadius) + Vector3.up * height);
        }

        RaycastHit hit;

        for (int i = 0; i < circle1.Count; i++)
        {
            if (Physics.Raycast(circle2[i], circle1[i] - circle2[i], out hit, Vector3.Distance(circle1[i], circle2[i])))
            {
                circle3.Add(hit.point);
            }
            else
            {
                circle3.Add(circle1[i]);
            }
        }


        CreateShape();
        UpdateMesh();
    }

    private void OnDrawGizmosSelected()
    {
        if (circle1.Count > 0)
        {
            for (int i = 0; i < circle1.Count; i++)
            {

                Gizmos.DrawLine(transform.position, circle1[i]);
                Gizmos.DrawLine(transform.position + Vector3.up * height, circle2[i]);
                Gizmos.DrawLine(circle1[i], circle2[i]);
            }
        }



    }
}
