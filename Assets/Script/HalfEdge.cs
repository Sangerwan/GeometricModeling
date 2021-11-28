using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
public class Vertex
{
    public int index;
    public Vector3 position;
    public Vertex(int i, Vector3 v)
    {
        this.index = i;
        this.position = v;
    }
}*/

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

    public Vector3 source;

    public HalfEdge prevEdge;
    public HalfEdge nextEdge;

    public HalfEdge twinEdge;

    public Face face;
    public HalfEdge(Vector3 v, Face f)
    {
        this.source = v;
        this.face = f; 
    }

    public HalfEdge(int i, Vector3 source, HalfEdge prev=null, HalfEdge next = null, HalfEdge twin = null, Face face = null)
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







