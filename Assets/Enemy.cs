using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Character {
    public Dictionary<Character, float> detectionDictionary = new Dictionary<Character, float>();
    public float detectionDistance;
    // Use this for initialization
    public GameObject cone;
    private void OnMouseDown()
    {
        UIController.ui.AssignDebugTarget(this);
    }

    private void Start()
    {
        MakeDetectionCone();
    }

    void MakeDetectionCone ()
    { 
        GameObject coneObj = Instantiate(cone, transform.position, Quaternion.identity, this.transform);
        Mesh mesh;
        coneObj.GetComponent<MeshFilter>().mesh = mesh = new Mesh();
        mesh.name = "Cone Mesh";

        Vector3[] vertices = new Vector3[4];
        vertices[0] = Vector3.zero;
        Quaternion rot1 = Quaternion.AngleAxis(-30, Vector3.up);
        Vector3 addDist1 = rot1 * transform.forward * detectionDistance;
        Vector3 dest = transform.position + addDist1;
        vertices[1] = addDist1;
        Quaternion rot2 = Quaternion.AngleAxis(30, Vector3.up);
        Vector3 addDist2 = rot2 * transform.forward * detectionDistance;
        Vector3 dest2 = transform.position + addDist2;
        vertices[3] = addDist2;
        vertices[2] = transform.forward * detectionDistance;

        
        mesh.vertices = vertices;

        int[] triangles = new int[6];
        triangles[0] = 0;
        triangles[1] = 1;
        triangles[2] = 2;
        triangles[3] = 0;
        triangles[4] = 2;
        triangles[5] = 3;

        mesh.triangles = triangles;


    }

    void Old ()
    {
        Vector3[] vertices = new Vector3[4];
        vertices[0] = Vector3.zero;
        Quaternion rot1 = Quaternion.AngleAxis(-30, Vector3.up);
        Vector3 addDist1 = rot1 * transform.forward * detectionDistance;
        Vector3 dest = transform.position + addDist1;
        vertices[1] = dest;
        Quaternion rot2 = Quaternion.AngleAxis(30, Vector3.up);
        Vector3 addDist2 = rot2 * transform.forward * detectionDistance;
        Vector3 dest2 = transform.position + addDist2;
        vertices[3] = dest2;
        vertices[2] = transform.position + transform.forward * detectionDistance;
    }
}
