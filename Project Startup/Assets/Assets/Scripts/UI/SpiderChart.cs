using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpiderChart : MonoBehaviour
{
    private TriangleRevertex[] triangles = new TriangleRevertex[5];
    public Transform[] positions = new Transform[5];
    private Vector3[] corners = new Vector3[5];
    public float scale = 200;
    
    // Start is called before the first frame update
    void Start()
    {
        triangles = GetComponentsInChildren<TriangleRevertex>();
        for (int i = 0; i < positions.Length; i++)
        {
            corners[i] = positions[i].position;
        }
    }

    // Update is called once per frame
    void Update()
    {
    }


    public void Spiderize(float defense, float health, float range, float speed, float attack)
    {
        Vector3 point0 = defense * scale * Vector2.Perpendicular(corners[1] - corners[4]).normalized;
        Vector3 point1 =health * scale * Vector2.Perpendicular(corners[2] - corners[0]).normalized;
        Vector3 point2 =range * scale * Vector2.Perpendicular(corners[3] - corners[1]).normalized;
        Vector3 point3 =speed * scale * Vector2.Perpendicular(corners[4] - corners[2]).normalized;
        Vector3 point4 =attack * scale * Vector2.Perpendicular(corners[0] - corners[3]).normalized;
        triangles[0].SetNewVerts(point1, transform.position, point4, point0);
        triangles[1].SetNewVerts(point2, transform.position, point0, point1);
        triangles[2].SetNewVerts(point3, transform.position, point1, point2);
        triangles[3].SetNewVerts(point4, transform.position, point2, point3);
        triangles[4].SetNewVerts(point0, transform.position, point3, point4);
        foreach (var tri in triangles)
        {
            tri.GetComponent<Image>().SetVerticesDirty();
        }
    }
}
