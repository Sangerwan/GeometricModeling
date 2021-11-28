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
        int index = 0;

        for (int i = 0; i < quads.Length / 4; i++)
        {
            Vector3 vertex1 = vertices[quads[index]];
            Vector3 vertex2 = vertices[quads[index + 1]];
            Vector3 vertex3 = vertices[quads[index + 2]];
            Vector3 vertex4 = vertices[quads[index + 3]];
            halfEdgeMesh.vertices.Add(vertex1);
            halfEdgeMesh.vertices.Add(vertex2);
            halfEdgeMesh.vertices.Add(vertex3);
            halfEdgeMesh.vertices.Add(vertex4); 

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

        List<HalfEdge> foundEdges = new List<HalfEdge>();
        foreach (var edge in halfEdgeMesh.edges)
        {
            if (foundEdges.Contains(edge)) continue;
            foreach(var twinEdge in halfEdgeMesh.edges)
            {
                if (Equals(twinEdge, edge))
                {
                    continue;
                }
                if (foundEdges.Contains(twinEdge))
                {
                    continue;
                }

                Vector3 edgeSource = edge.source;
                Vector3 edgeNext = edge.nextEdge.source;
                Vector3 twinEdgeSource  = twinEdge.source;
                Vector3 twinEdgeNext = twinEdge.nextEdge.source;


                if (edgeSource == twinEdgeNext && edgeNext == twinEdgeSource)
                {
                    edge.twinEdge = twinEdge;
                    twinEdge.twinEdge = edge;
                    foundEdges.Add(twinEdge);
                    foundEdges.Add(edge);
                }
            }
        }

        return halfEdgeMesh;

    }
    /*
    public static Mesh HalfEdgeToVertexFace(HalfEdgeMesh halfEdgeMesh)
    {
        List<HalfEdge> edges = halfEdgeMesh.edges;

    }*/
}
