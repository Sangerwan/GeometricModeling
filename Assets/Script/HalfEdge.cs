using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Vertex
{
    public int index;
    public Vector3 position;
    public Vertex(Vector3 v = default(Vector3), int i = -1)
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

    public HalfEdge(int i, Vertex source, HalfEdge prev=null, HalfEdge next = null, HalfEdge twin = null, Face face = null)
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
    public List<Vertex> vertices = new List<Vertex>();
    public List<HalfEdge> edges = new List<HalfEdge>();
    public List<Face> faces = new List<Face>();

    public bool hasNormal() { return true; }

    public bool hasUV() { return true; }

    public void setTwinEdges()
    {
        List<HalfEdge> foundEdges = new List<HalfEdge>();
        foreach (var edge in edges)
        {
            if (foundEdges.Contains(edge)) continue;
            foreach (var twinEdge in edges)
            {
                if (Equals(twinEdge, edge))
                {
                    continue;
                }
                if (foundEdges.Contains(twinEdge))
                {
                    continue;
                }

                Vertex edgeSource = edge.source;
                Vertex edgeNext = edge.nextEdge.source;
                Vertex twinEdgeSource = twinEdge.source;
                Vertex twinEdgeNext = twinEdge.nextEdge.source;


                if (edgeSource.position == twinEdgeNext.position && edgeNext.position == twinEdgeSource.position)
                {
                    edge.twinEdge = twinEdge;
                    twinEdge.twinEdge = edge;
                    foundEdges.Add(twinEdge);
                    foundEdges.Add(edge);
                }
            }
        }
    }
}







