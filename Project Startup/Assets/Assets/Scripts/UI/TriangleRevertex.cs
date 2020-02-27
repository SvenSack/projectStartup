using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TriangleRevertex : Image
{
    private Vector3 vert1;
    private Vector3 vert2;
    private Vector3 vert3;
    private Vector3 vert4;
    private Vector3[] verts;

    private void Start()
    {
        verts = new Vector3[] {vert1, vert2, vert3, vert4};
    }

    protected override void OnPopulateMesh(VertexHelper vh)
    {
        base.OnPopulateMesh(vh);
 
        for (int i = 0; i < vh.currentVertCount; i++)
        {
            UIVertex vert = UIVertex.simpleVert;
            vh.PopulateUIVertex(ref vert, i);
            Vector3 position = vert.position;
 
            position = verts[i];
            Debug.Log(gameObject.name + " " +  i + "  " + position);
            Debug.DrawRay(transform.position+position, transform.forward*1000, new Color(0.1f*i,0.1f*i,0.1f*i));

            vert.position = position;
            vh.SetUIVertex(vert, i);
        }
    }

    public void SetNewVerts(Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4)
    {
        Vector3 tramp = transform.position;
        verts[0] = v1-tramp;
        verts[1] = v2-tramp;
        verts[2] = v3-tramp;
        verts[3] = v4-tramp;
    }
}
