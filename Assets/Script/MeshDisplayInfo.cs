using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


[RequireComponent(typeof(MeshFilter))]
public class MeshDisplayInfo : MonoBehaviour
{
    MeshFilter m_Mf;
    [Header("edges")]
    [SerializeField] bool m_DisplayEdges;
    [SerializeField] int m_NMaxEdges;

    [Header("normals")]
    [SerializeField] bool m_DisplayNormals;
    [SerializeField] int m_NMaxNormals;
    [SerializeField] int m_NormalScaleFactor;

    [Header("Vertices")]
    [SerializeField] bool m_DisplayVertices;
    [SerializeField] int m_NMaxVertices;

    [Header("Faces")]
    [SerializeField] bool m_DisplayFaces;
    [SerializeField] int m_NMaxfaces;


    private void Awake()
    {
        m_Mf = GetComponent<MeshFilter>();
    }

    private void OnDrawGizmos()
    {
        if (m_Mf==null) return;
        Debug.Log("je passe 2");

        Vector3[] vertices = m_Mf.sharedMesh.vertices;

        //EDGes
        if (m_DisplayEdges || m_DisplayFaces)
        {
            GUIStyle style = new GUIStyle();
            style.fontSize = 16;
            style.normal.textColor = Color.blue;

            int[] quads = m_Mf.sharedMesh.GetIndices(0);
            int index = 0;
            
            //    setindexes
            for (int i = 0; i < Mathf.Min(quads.Length,Mathf.Max(m_NMaxEdges,m_NMaxfaces)); i++)
            {
                int index1 = quads[index++];
                int index2 = quads[index++];
                int index3 = quads[index++];
                int index4 = quads[index++];

                Vector3 pt1 = transform.TransformPoint(vertices[index1]);
                Vector3 pt2 = transform.TransformPoint(vertices[index2]);
                Vector3 pt3 = transform.TransformPoint(vertices[index3]);
                Vector3 pt4 = transform.TransformPoint(vertices[index4]);
                // il faut mettre les point en global pour gizmo

                if (m_DisplayEdges && i < m_NMaxEdges)
                {                  
                    Gizmos.color = Color.black;
                    Gizmos.DrawLine(pt1, pt2);
                    Gizmos.DrawLine(pt3, pt2);
                    Gizmos.DrawLine(pt1, pt4);
                    Gizmos.DrawLine(pt4, pt3);
                }
                if (m_DisplayEdges && i < m_NMaxfaces)
                {
                    string str = $"{i}:{index1},{index2},{index3},{index4}";
                    Vector3 faceCenter = (pt1 + pt2 + pt3 + pt4) * .25f;
                    Gizmos.color = Color.blue;
                    Gizmos.DrawSphere(faceCenter, .01f);
                    Handles.Label(faceCenter, str, style);
                }
            }
        }

        //normal
        if (m_DisplayNormals)
        {
            Vector3[] normals = m_Mf.sharedMesh.normals;

            int index = 0;
            Gizmos.color = Color.red;
            //    setindexes
            for (int i = 0; i < Mathf.Min(normals.Length, m_NMaxNormals); i++)
            {
                Vector3 pos = transform.TransformPoint(vertices[i]);
                Vector3 normal = transform.TransformDirection(normals[i]);
                // il faut mettre les point en global pour gizmo
                Gizmos.DrawLine(pos, pos+normal);


            }
        }
        //normal
        if (m_DisplayVertices)
        {
            GUIStyle style = new GUIStyle();
            style.fontSize = 16;
            style.normal.textColor = Color.red;
            Gizmos.color = Color.red;
            //    setindexes
            for (int i = 0; i < Mathf.Min(vertices.Length, m_NMaxVertices); i++)
            {
                Vector3 pos = transform.TransformPoint(vertices[i]);
                // il faut mettre les point en global pour gizmo
                Gizmos.DrawSphere(pos,0.01f);
                Handles.Label(pos, i.ToString(),style);

            }
        }
    }

    public static string ExportMeshCSV(Mesh mesh)
    {
        if (!mesh) return "";
        //header des colonnes
        //vertexIndex,VertexPosX,VertexPosY,VertexPosZ,QuadIndex,QuadVertexIndex1,QuadVertexIndex2,QuadVertexIndex3,QuadVertexIndex4

        List<string> strings = new List<string>();
        strings.Add("vertexIndex\tVertexPosX\tVertexPosY\tVertexPosZ\tQuadIndex\tQuadVertexIndex1\tQuadVertexIndex2\tQuadVertexIndex3\tQuadVertexIndex4");

        Vector3[] vertices = mesh.vertices;
        int[] quads = mesh.GetIndices(0);

        for (int i = 0; i < vertices.Length; i++)
        {
            Vector3 pos = vertices[i];
            strings.Add($"{i}\t{pos.x.ToString("N02")}\t{pos.y.ToString("N02")}\t{pos.z.ToString("N02")}\t");
        }

        int index = 0;
        for (int i = 0; i < quads.Length / 4; i++)
        {
            string tmp = $"{i}\t{quads[index++]}\t{quads[index++]}\t{quads[index++]}\t{quads[index++]}";
            if (i + 1 < strings.Count) strings[i + 1] += tmp;
            else strings.Add("\t\t\t\t" + tmp);
        }

        return string.Join("\n",strings);
    }

}
