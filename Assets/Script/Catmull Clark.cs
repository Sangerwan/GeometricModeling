using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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

    public static void TrianglesToQuads(Mesh mesh, out int[] quads)
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
        TrianglesToQuads(mesh, out quads);

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

            Face face = new Face(index/4);
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

        halfEdgeMesh.SetTwinEdges();
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
            quads.Add(face.edge.source.index);
            quads.Add(face.edge.nextEdge.source.index);
            quads.Add(face.edge.nextEdge.nextEdge.source.index);
            quads.Add(face.edge.nextEdge.nextEdge.nextEdge.source.index);
        }

        newMesh.vertices = vertices.ToArray();
        newMesh.SetIndices(quads, MeshTopology.Quads, 0);
        newMesh.RecalculateBounds();
        newMesh.RecalculateNormals();

        return newMesh;
    }

    public static Vertex FacePoint(Face f)
    {
        Vector3 vertex1 = f.edge.source.position;
        Vector3 vertex2 = f.edge.nextEdge.source.position;
        Vector3 vertex3 = f.edge.nextEdge.nextEdge.source.position;   
        Vector3 vertex4 = f.edge.nextEdge.nextEdge.nextEdge.source.position;
        Vector3 result = (vertex1 + vertex2 + vertex3 + vertex4);
        result /= 4;
        return new Vertex(result);
    }

    public static Vertex EdgePoint(HalfEdge edge)
    {
        Vertex vertex1 = edge.source;
        Vertex vertex2 = edge.nextEdge.source;
        Vertex result;
        if (edge.twinEdge != null)
        {
            Face face1 = edge.face;
            Face face2 = edge.twinEdge.face;            

            Vertex vertex3 = FacePoint(edge.face);
            Vertex vertex4 = FacePoint(edge.twinEdge.face);
            result = new Vertex((vertex1.position + vertex2.position + vertex3.position + vertex4.position) / 4);
        }
        else
        {
            result = new Vertex((vertex1.position + vertex2.position) / 2);
        }
        return result;
    }

    public static Face[] GetNeighboursFaces(Vertex v, HalfEdgeMesh halfEdgeMesh)
    {
        List<Face> faces = new List<Face>();
        foreach(var edges in halfEdgeMesh.edges)
        {
            if(edges.source.position == v.position)
            {
                faces.Add(edges.face);
            }
        }
        return faces.ToArray();
    }

    public static HalfEdge[] GetNeighboursEdges(Vertex v, HalfEdgeMesh halfEdgeMesh)
    {
        List<HalfEdge> edges = new List<HalfEdge>();
        foreach(var edge in halfEdgeMesh.edges)
        {
            if(edge.source.position == v.position)
            {
                edges.Add(edge);
            }
        }
        return edges.ToArray();
    }

    public static void GetNeighboursFacesEdges(Vertex v, HalfEdgeMesh halfEdgeMesh,out Face[] faces, out HalfEdge[] edges)
    {
        faces = GetNeighboursFaces(v, halfEdgeMesh);
        edges = GetNeighboursEdges(v, halfEdgeMesh);
    }

    public static Vertex VertexPoint(Vertex v, HalfEdgeMesh halfEdgeMesh)
    {
        Face[] faces;
        HalfEdge[] edges;

        GetNeighboursFacesEdges(v, halfEdgeMesh,out faces, out edges);

        int n = edges.Length;
        if(n >= 3)
        {
            Vector3 Q = new Vector3();
            Vector3 R = new Vector3();
            for (int i = 0; i < faces.Length; i++)
            {
                Q += FacePoint(faces[i]).position;
            }
            Q /= faces.Length;

            for (int i = 0; i < edges.Length; i++)
            {
                R += (edges[i].source.position + edges[i].nextEdge.source.position)/2;
            }
            R /= edges.Length;
            return new Vertex( Q / n + 2 * R / n + (n - 3) * v.position / n , v.index);
        }
        else //boundary
        {
            return v;
        }
    }

    public static HalfEdge SplitEdge(HalfEdge e, Vertex v, HalfEdgeMesh halfEdgeMesh)
    {
        HalfEdge newHalfEdge = new HalfEdge(v, e.face);
        newHalfEdge.nextEdge = e.nextEdge;
        newHalfEdge.prevEdge = e;
        e.nextEdge = newHalfEdge;

        halfEdgeMesh.Add(newHalfEdge);

        return newHalfEdge;
    }

    public static void SetNextPrevEdge(HalfEdge startEdge, HalfEdge endEdge)
    {
        if (startEdge == endEdge) return;
        startEdge.nextEdge = endEdge;
        endEdge.prevEdge = startEdge;
    }

    //public static void SetNewEdgesFromSplitFace(Face f, Vertex v, HalfEdge startEdge, HalfEdgeMesh halfEdgeMesh)
    //{
    //    HalfEdge edge1 = new HalfEdge(startEdge.nextEdge.source, f);
    //    HalfEdge edge2 = new HalfEdge(v, f);

    //    SetNextPrevEdge(startEdge, edge1);
    //    SetNextPrevEdge(edge1, edge2);
    //    SetNextPrevEdge(edge2, startEdge.prevEdge);

    //    halfEdgeMesh.edges.Add(edge1);
    //    halfEdgeMesh.edges.Add(edge2);

    //    edge1.index = halfEdgeMesh.edges.Count;
    //    halfEdgeMesh.edges.Add(edge1);
    //    edge2.index = halfEdgeMesh.edges.Count;
    //    halfEdgeMesh.edges.Add(edge2);

        

    //    /*
    //    edge1.nextEdge = edge2;
    //    edge1.prevEdge = startEdge;
    //    edge2.nextEdge = startEdge.prevEdge;
    //    edge2.prevEdge = edge1;
    //    startEdge.nextEdge = edge1;
    //    startEdge.prevEdge.prevEdge = edge2;*/

    //}

    //public static void SplitFace(Face f1, Vertex v, HalfEdgeMesh halfEdgeMesh)
    //{
    //    Face f2 = new Face(halfEdgeMesh.faces.Count);
    //    Face f3 = new Face(halfEdgeMesh.faces.Count + 1);
    //    Face f4 = new Face(halfEdgeMesh.faces.Count + 2);

    //    HalfEdge startEdge1 = f1.edge;
    //    HalfEdge startEdge2 = f1.edge.nextEdge.nextEdge;
    //    HalfEdge startEdge3 = startEdge2.nextEdge.nextEdge;
    //    HalfEdge startEdge4 = startEdge3.nextEdge.nextEdge;

    //    f2.edge = startEdge2;
    //    f3.edge = startEdge3;
    //    f4.edge = startEdge4;

    //    v.index = halfEdgeMesh.vertices.Count;

    //    SetNewEdgesFromSplitFace(f1, v, startEdge1, halfEdgeMesh);
    //    SetNewEdgesFromSplitFace(f2, v, startEdge2, halfEdgeMesh);
    //    SetNewEdgesFromSplitFace(f3, v, startEdge3, halfEdgeMesh);
    //    SetNewEdgesFromSplitFace(f4, v, startEdge4, halfEdgeMesh);

    //    halfEdgeMesh.faces.Add(f2);
    //    halfEdgeMesh.faces.Add(f3);
    //    halfEdgeMesh.faces.Add(f4);
    //}

    public static void ConnectEdgesToFacePoint(HalfEdge startEdge, Face face, Vertex facePoint, int index, HalfEdgeMesh halfEdgeMesh)
    {
        if (index < 0 || index >= 4) return;

        HalfEdge[] halfEdges = new HalfEdge[4];

        halfEdges[index] = startEdge;
        halfEdges[(index + 1) % 4] = new HalfEdge(startEdge.nextEdge.source, face);
        halfEdges[(index + 2) % 4] = new HalfEdge(facePoint, face);
        halfEdges[(index + 3) % 4] = startEdge.prevEdge;

        face.edge = halfEdges[0];

        SetNextPrevEdge(halfEdges[index], halfEdges[(index + 1) % 4]);
        SetNextPrevEdge(halfEdges[(index + 1) % 4], halfEdges[(index + 2) % 4]);
        SetNextPrevEdge(halfEdges[(index + 2) % 4], startEdge.prevEdge);

        halfEdgeMesh.Add(halfEdges[(index + 1) % 4]);
        halfEdgeMesh.Add(halfEdges[(index + 2) % 4]);
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

        for (int i = 0; i < vertices.Count; i++)
        {
            vertices[i] = VertexPoint(vertices[i], halfEdgeMesh);
        }

        foreach (Face face in faces)
        {
            Vertex facePoint = FacePoint(face);
            halfEdgeMesh.Add(facePoint);
            facePoints.Add(facePoint);
        }

        foreach(HalfEdge halfEdge in halfEdges)
        {
            Vertex edgePoint = EdgePoint(halfEdge);
            halfEdgeMesh.Add(edgePoint);
            edgePoints.Add(edgePoint);
        }

        

        int faceCount = faces.Count;
        Dictionary<Face, Face[]> subFace = new Dictionary<Face, Face[]>();
        for(int i=0; i< faceCount; i++)
        {
            HalfEdge halfEdge1 = faces[i].edge;
            HalfEdge halfEdge2 = faces[i].edge.nextEdge;
            HalfEdge halfEdge3 = faces[i].edge.nextEdge.nextEdge;
            HalfEdge halfEdge4 = faces[i].edge.nextEdge.nextEdge.nextEdge;

            Face f1 = faces[i];
            Face f2 = new Face(halfEdgeMesh.faces.Count);
            Face f3 = new Face(halfEdgeMesh.faces.Count + 1);
            Face f4 = new Face(halfEdgeMesh.faces.Count + 2);

            halfEdgeMesh.Add(f2);
            halfEdgeMesh.Add(f3);
            halfEdgeMesh.Add(f4);

            HalfEdge halfEdgeFace2 = SplitEdge(halfEdge1, edgePoints[halfEdge1.index], halfEdgeMesh);
            HalfEdge halfEdgeFace3 = SplitEdge(halfEdge2, edgePoints[halfEdge2.index], halfEdgeMesh);
            HalfEdge halfEdgeFace4 = SplitEdge(halfEdge3, edgePoints[halfEdge3.index], halfEdgeMesh);
            HalfEdge halfEdgeFace1 = SplitEdge(halfEdge4, edgePoints[halfEdge4.index], halfEdgeMesh);            

            halfEdge1.prevEdge = halfEdgeFace1;
            halfEdge2.prevEdge = halfEdgeFace2;
            halfEdge3.prevEdge = halfEdgeFace3;
            halfEdge4.prevEdge = halfEdgeFace4;

            halfEdgeFace1.face = f1;
            halfEdgeFace2.face = f2;
            halfEdgeFace3.face = f3;
            halfEdgeFace4.face = f4;

            halfEdge2.face = f2;
            halfEdge3.face = f3;
            halfEdge4.face = f4;

            Vertex facePoint = facePoints[faces[i].index];

            ConnectEdgesToFacePoint(halfEdge1, f1, facePoint, 0, halfEdgeMesh);
            ConnectEdgesToFacePoint(halfEdge2, f2, facePoint, 1, halfEdgeMesh);
            ConnectEdgesToFacePoint(halfEdge3, f3, facePoint, 2, halfEdgeMesh);
            ConnectEdgesToFacePoint(halfEdge4, f4, facePoint, 3, halfEdgeMesh);


            ///


            //HalfEdge subHalfEdge2 = new HalfEdge(halfEdge1.nextEdge.source, f1);
            //HalfEdge subHalfEdge3 = new HalfEdge(facePoints[faces[i].index], f1);
            //setNextPrevEdge(halfEdge1, subHalfEdge2);
            //setNextPrevEdge(subHalfEdge2, subHalfEdge3);
            //setNextPrevEdge(subHalfEdge3, halfEdge1.prevEdge);


            subFace[f1] = new Face[] { f1, f2, f3, f4 };

            //splitEdge(halfEdge1, )

        }
        halfEdgeMesh.SetTwinEdges();
        /*
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

        halfEdgeMesh.setTwinEdges();*/
    }
}
