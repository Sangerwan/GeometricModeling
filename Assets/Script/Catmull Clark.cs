using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public class CatmullClark : MonoBehaviour
{
    // Start is called before the first frame update

    void explications()
    {
        /*Subdivision
         * 
         * 1) calcul de nouveaux points ( ! ne fonctionne que pour des vertices intérieures différentes des boundaries vertices)
         * * 
         *  -FacePoints : les isobarycentres des faces/centroïdes
         *      isobarycentre = moyenne des vertices qui définissent la face (somme des positions vertices divisés par le nombre)
         *      
         *  -EdgePoints = moyenne des extémités de l'edge et des deux FacePoints des faces adjacentes
         *      ! le point obtenu n'est pas forcément sur l'edge concerné mais dans son voisinnage 
         *      
         * FacePoints et EdgePoints sont des nouvelles vertices (points)
         * VertexPoints c'est la nouvelle position des vertices existantes (maj)
         *      
         *  -VertexPoints :  newPos = Q/n + 2R/n + (n-3)*currPos/n
         *      Q = moyenne des FacePoints de la vertice (faces adjacentes)
         *      R = moyenne des mid-Points de la vertice 
         *          mid-points : centre des edges = (startPosEdge + endPosEdge)/2
         *      n = Valence de la vertice : nombre d'edges arrivant sur la vertice
         * 
         * Cas bordure : (1ère version)
         *  -FacePoints même calcul
         *  -EdgePoints = mid_points
         *  -VertexPoints ne bouge (pas de changements de positions des points sur la bordure)
         * 
         *si la valence = nombre de faces d'adjacences le point est en bordure
         *
         *2) création des nouvelles faces
         *
         *  -Connexion des FacePoints et des EdgePoints
         *  -Connexion des EdgePoints et des Vertices (qui ont changés de position)
         *  
         *  
         *Pseudo Code
         *  1) création des nouveaux points : facePoints et edgePoints
         *      Rajouter dans les structure edge/ face les facepoints et edgePoints
         *  2) updtae de la position des vertices (vertex points)
         *  3) split des edges en y insérant l'edge point
         *      conserver l'edge avant split et modifier une de ses positions + insérer les nouvelles edges (perfo)
         *      methode SplitEdge(Edge, vertex)
         *  4) split des faces (seulement pour des faces ayant un nombre de vertices pair)
         *      - on ajoute le FacePoint
         *      - on créée k/2 nouvelles faces si k vertices(y compris les vertices issues de l'étape 3)
         *      methode splitFace(Face,vertex)
         * 
         * 
         * 
         */
    }

    public static void trianglesToQuads(Mesh mesh, out int[] quads)
    {
        Vector3[] triVertices = mesh.vertices;
        int[] triIndices = mesh.triangles;

        quads = new int[triIndices.Length / 6 * 4];

        int index = 0;
        for (int i = 0; i < triIndices.Length; i++)
        {
            quads[index++] = triIndices[i++];
            quads[index++] = triIndices[i++];
            quads[index++] = triIndices[i++];
            i += 2;
            quads[index++] = triIndices[i];
        }
    }

    public static HalfEdgeMesh VertexFaceToHalfEdge(Mesh mesh)
    {
        HalfEdgeMesh halfEdgeMesh = new HalfEdgeMesh();

        int[] quads;
        Vector3[] vertices = mesh.vertices;
        trianglesToQuads(mesh, out quads);

        for(int i =0; i < vertices.Length; i++)
        {
            halfEdgeMesh.vertices.Add(new Vertex(vertices[i],i));
        }

        int index = 0;

        for (int i = 0; i < quads.Length / 4; i++)
        {
            Vertex vertex1 = halfEdgeMesh.vertices[quads[index]];
            Vertex vertex2 = halfEdgeMesh.vertices[quads[index + 1]];
            Vertex vertex3 = halfEdgeMesh.vertices[quads[index + 2]];
            Vertex vertex4 = halfEdgeMesh.vertices[quads[index + 3]];

            HalfEdge halfEdge1 = new HalfEdge(index, vertex1);
            HalfEdge halfEdge2 = new HalfEdge(index + 1, vertex2);
            HalfEdge halfEdge3 = new HalfEdge(index + 2, vertex3);
            HalfEdge halfEdge4 = new HalfEdge(index + 3, vertex4);

            Face face = new Face(index);
            face.edge = halfEdge1;
            halfEdgeMesh.faces.Add(face);

            halfEdge1.prevEdge = halfEdge4;
            halfEdge2.prevEdge = halfEdge1;
            halfEdge3.prevEdge = halfEdge2;
            halfEdge4.prevEdge = halfEdge3;

            halfEdge1.nextEdge = halfEdge2;
            halfEdge2.nextEdge = halfEdge3;
            halfEdge3.nextEdge = halfEdge4;
            halfEdge4.nextEdge = halfEdge1;

            halfEdge1.face = face;
            halfEdge2.face = face;
            halfEdge3.face = face;
            halfEdge4.face = face;

            halfEdgeMesh.edges.Add(halfEdge1);
            halfEdgeMesh.edges.Add(halfEdge2);
            halfEdgeMesh.edges.Add(halfEdge3);
            halfEdgeMesh.edges.Add(halfEdge4);

            index += 4;
        }

        halfEdgeMesh.setTwinEdges();

        

        return halfEdgeMesh;

    }
    
    public static Mesh HalfEdgeToVertexFace(HalfEdgeMesh halfEdgeMesh)
    {
        Mesh newMesh = new Mesh();
        List<HalfEdge> edges = halfEdgeMesh.edges;
        List<Vector3> vertices = halfEdgeMesh.vertices.ConvertAll(x => x.position);
        List<int> quads = new List<int>();

        foreach( var face in halfEdgeMesh.faces)
        {
            quads.Add(vertices.FindIndex(x => x == face.edge.source.position));
            quads.Add(vertices.FindIndex(x => x == face.edge.nextEdge.source.position));
            quads.Add(vertices.FindIndex(x => x == face.edge.nextEdge.nextEdge.source.position));
            quads.Add(vertices.FindIndex(x => x == face.edge.nextEdge.nextEdge.nextEdge.source.position));
        }

        newMesh.vertices = vertices.ToArray();
        newMesh.SetIndices(quads, MeshTopology.Quads, 0);
        newMesh.RecalculateBounds();
        newMesh.RecalculateNormals();

        return newMesh;
    }

    public static Vertex facePoint(Face f)
    {
        Vertex vertice1 = f.edge.source;
        Vertex vertice2 = f.edge.nextEdge.source;
        Vertex vertice3 = f.edge.nextEdge.nextEdge.source;
        Vertex vertice4 = f.edge.nextEdge.nextEdge.nextEdge.source;
        Vertex result = new Vertex((vertice1.position + vertice2.position + vertice3.position + vertice4.position) / 4);
        return result;
    }

    public static Vertex edgePoint(HalfEdge edge)
    {
        Vertex vertice1 = edge.source;
        Vertex vertice2 = edge.nextEdge.source;
        Vertex result;
        if (edge.twinEdge != null)
        {
            Face face1 = edge.face;
            Face face2 = edge.twinEdge.face;            

            Vertex vertice3 = facePoint(edge.face);
            Vertex vertice4 = facePoint(edge.twinEdge.face);
            result = new Vertex((vertice1.position + vertice2.position + vertice3.position + vertice4.position) / 4);
        }
        else
        {
            result = new Vertex((vertice1.position + vertice2.position) / 2);
        }
        return result;
    }

    public static Face[] getNeighboursFaces(Vertex v, HalfEdgeMesh halfEdgeMesh)
    {
        List<Face> faces = new List<Face>();
        foreach(var edges in halfEdgeMesh.edges)
        {
            if(edges.source == v)
            {
                faces.Add(edges.face);
            }
        }
        return faces.ToArray();
    }

    public static HalfEdge[] getNeighboursEdges(Vertex v, HalfEdgeMesh halfEdgeMesh)
    {
        List<HalfEdge> edges = new List<HalfEdge>();
        foreach(var edge in halfEdgeMesh.edges)
        {
            if(edge.source == v)
            {
                edges.Add(edge);
            }
        }
        return edges.ToArray();
    }

    public static void getNeighboursFacesEdges(Vertex v, HalfEdgeMesh halfEdgeMesh,out Face[] faces, out HalfEdge[] edges)
    {
        faces = getNeighboursFaces(v, halfEdgeMesh);
        edges = getNeighboursEdges(v, halfEdgeMesh);
    }

    public static Vertex vertexPoint(Vertex v, HalfEdgeMesh halfEdgeMesh)
    {
        Face[] faces;
        HalfEdge[] edges;

        getNeighboursFacesEdges(v, halfEdgeMesh,out faces, out edges);

        int n = edges.Length;
        if(n >= 4)
        {
            Vector3 Q = new Vector3();
            Vector3 R = new Vector3();
            for (int i = 0; i < faces.Length; i++)
            {
                Q += facePoint(faces[i]).position;
            }
            Q /= faces.Length;

            for (int i = 0; i < edges.Length; i++)
            {
                R += edges[i].source.position + edges[i].nextEdge.source.position;
            }
            R /= edges.Length;
            return new Vertex(Q / n + (2 * R) / n + ((n - 3) / n) * v.position, v.index);
        }
        else
        {
            return v;
        }
    }

    public static void splitEdge(HalfEdge e, Vertex v, HalfEdgeMesh halfEdgeMesh)
    {
        HalfEdge newHalfEdge = new HalfEdge(v, e.face);
        newHalfEdge.nextEdge = e.nextEdge;
        newHalfEdge.prevEdge = e;
        e.nextEdge = newHalfEdge;
        halfEdgeMesh.edges.Add(newHalfEdge);
        newHalfEdge.index = halfEdgeMesh.edges.Count+1;
        v.index = halfEdgeMesh.vertices.Count+1;
        halfEdgeMesh.vertices.Add(v);
    }

    public static void setNextPrevEdge(HalfEdge startEdge, HalfEdge endEdge)
    {
        if (startEdge == endEdge) return;
        startEdge.nextEdge = endEdge;
        endEdge.prevEdge = startEdge;
    }

    public static void setNewEdgesFromSplitFace(Face f, Vertex v, HalfEdge startEdge, HalfEdgeMesh halfEdgeMesh)
    {
        HalfEdge edge1 = new HalfEdge(startEdge.nextEdge.source, f);
        HalfEdge edge2 = new HalfEdge(v, f);

        setNextPrevEdge(startEdge, edge1);
        setNextPrevEdge(edge1, edge2);
        setNextPrevEdge(edge2, startEdge.prevEdge);

        halfEdgeMesh.edges.Add(edge1);
        halfEdgeMesh.edges.Add(edge2);

        edge1.index = halfEdgeMesh.edges.Count+1;
        edge2.index = halfEdgeMesh.edges.Count+1;

        halfEdgeMesh.edges.Add(edge1);
        halfEdgeMesh.edges.Add(edge2);

        /*
        edge1.nextEdge = edge2;
        edge1.prevEdge = startEdge;
        edge2.nextEdge = startEdge.prevEdge;
        edge2.prevEdge = edge1;
        startEdge.nextEdge = edge1;
        startEdge.prevEdge.prevEdge = edge2;*/

    }

    public static void splitFace(Face f1, Vertex v, HalfEdgeMesh halfEdgeMesh)
    {
        Face f2 = new Face(halfEdgeMesh.faces.Count + 1);
        Face f3 = new Face(halfEdgeMesh.faces.Count + 2);
        Face f4 = new Face(halfEdgeMesh.faces.Count + 3);

        HalfEdge startEdge1 = f1.edge;
        HalfEdge startEdge2 = f1.edge.nextEdge.nextEdge;
        HalfEdge startEdge3 = startEdge2.nextEdge.nextEdge;
        HalfEdge startEdge4 = startEdge3.nextEdge.nextEdge;

        f2.edge = startEdge2;
        f3.edge = startEdge3;
        f4.edge = startEdge4;

        v.index = halfEdgeMesh.vertices.Count+1;
        halfEdgeMesh.vertices.Add(v);

        setNewEdgesFromSplitFace(f1, v, startEdge1, halfEdgeMesh);
        setNewEdgesFromSplitFace(f2, v, startEdge2, halfEdgeMesh);
        setNewEdgesFromSplitFace(f3, v, startEdge3, halfEdgeMesh);
        setNewEdgesFromSplitFace(f4, v, startEdge4, halfEdgeMesh);

        halfEdgeMesh.faces.Add(f2);
        halfEdgeMesh.faces.Add(f3);
        halfEdgeMesh.faces.Add(f4);
    }

    public static Mesh Catmull_Clark(Mesh mesh)
    {
        HalfEdgeMesh halfEdgeMesh = VertexFaceToHalfEdge(mesh);
        Catmull_Clark(halfEdgeMesh);
        Mesh newMesh = HalfEdgeToVertexFace(halfEdgeMesh);
        return newMesh;
    }

    public static void Catmull_Clark(HalfEdgeMesh halfEdgeMesh)
    {
        List<Face> faces = halfEdgeMesh.faces;
        List<HalfEdge> halfEdges = halfEdgeMesh.edges;
        List<Vertex> vertices = halfEdgeMesh.vertices;

        List<Vertex> facePoints = new List<Vertex>();
        List<Vertex> edgePoints = new List<Vertex>();

        foreach(Face face in faces)
        {
            facePoints.Add(facePoint(face));
        }

        foreach(HalfEdge halfEdge in halfEdges)
        {
            edgePoints.Add(edgePoint(halfEdge));
        }

        for(int i=0; i< vertices.Count; i++)
        {
            vertices[i] = vertexPoint(vertices[i], halfEdgeMesh);
        }
        int halfEdgesCount = halfEdges.Count;
        for(int i=0; i< halfEdgesCount; i++)
        {
            splitEdge(halfEdges[i], edgePoints[i], halfEdgeMesh); //count
        }

        int facesCount = faces.Count;
        for(int i = 0; i < facesCount; i++)
        {
            splitFace(faces[i], facePoints[i], halfEdgeMesh); //count
        }

        halfEdgeMesh.setTwinEdges();
    }
}
