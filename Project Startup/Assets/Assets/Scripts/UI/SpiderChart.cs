using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpiderChart : MonoBehaviour
{
    private Image target;
    private TriangleRevertex[] triangles = new TriangleRevertex[5];
    public Transform[] positions = new Transform[5];
    private Vector3[] corners = new Vector3[5];
    
    // Start is called before the first frame update
    void Start()
    {
        target = GetComponent<Image>();
        triangles = GetComponentsInChildren<TriangleRevertex>();
        for (int i = 0; i < positions.Length; i++)
        {
            corners[i] = positions[i].position;
        }
    }

    // Update is called once per frame
    void Update()
    {
        Spiderize(1,2,3,4,5);
    }


    public void Spiderize(float defense, float health, float range, float speed, float attack)
    {
        triangles[0].SetNewVerts(corners[1], corners[0], corners[4], corners[0]+new Vector3(100,100,0));
        triangles[1].SetNewVerts(corners[2], corners[1], corners[0], corners[0]+new Vector3(100,100,0));
        triangles[2].SetNewVerts(corners[3], corners[2], corners[1], corners[0]+new Vector3(100,100,0));
        triangles[3].SetNewVerts(corners[4], corners[3], corners[2], corners[0]+new Vector3(100,100,0));
        triangles[4].SetNewVerts(corners[0], corners[4], corners[3], corners[0]+new Vector3(100,100,0));
        foreach (var tri in triangles)
        {
            tri.GetComponent<Image>().SetVerticesDirty();
        }
    }
}
