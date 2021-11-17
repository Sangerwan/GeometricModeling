using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public HalfEdgeMesh VertexFaceToHalfEdge(Mesh mesh)
    {
        return null;
    }

    public Mesh HalfEdgeToVertexFace(HalfEdgeMesh mesh)
    {
        return null;
    }
}

public class Vertex
{
    public int index;
    public Vector3 position;
    public Vertex(int i, Vector3 v)
    {
        this.index = i;
        this.position = v;
    }
}

public class Face
{
    public int index;
    public HalfEdge edge;

    public Face(int i)
    {
        this.index = i;
    }
}

public class HalfEdge
{
    public int index;

    public Vertex source;

    public HalfEdge prevEdge;
    public HalfEdge nextEdge;

    public HalfEdge twinEdge;

    public Face face;
    public HalfEdge(Vertex v, Face f)
    {
        this.source = v;
        this.face = f; 
    }

    public HalfEdge(int i, Vertex source, HalfEdge prev, HalfEdge next, HalfEdge twin, Face face)
    {
        this.index = i;
        this.source = source;
        this.prevEdge = prev;
        this.nextEdge = next;
        this.twinEdge = twin;
        this.face = face;
    }
}


public class HalfEdgeMesh
{
    int data = 0;

    public string name;
    public List<Vector3> vertices = new List<Vector3>();
    public List<HalfEdge> edges = new List<HalfEdge>();
    public List<Face> faces = new List<Face>();

    public bool hasNormal() { return true; }

    public bool hasUV() { return true; }

}





