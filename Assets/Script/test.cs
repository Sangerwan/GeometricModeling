using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test : MonoBehaviour
{
    MeshFilter m_Mf;
    [Header("Test")]
    [SerializeField] int m_test;
    [SerializeField] bool m_debug;
    // Start is called before the first frame update
    void Start()
    {
        m_Mf = GetComponent<MeshFilter>();
        if (m_test == 0)
        {            
            for(int i=0; i<m_test; i++)
                m_Mf.sharedMesh = CatmullClark.HalfEdgeToVertexFace(CatmullClark.VertexFaceToHalfEdge(m_Mf.sharedMesh));
            Debug.Log("Mesh");
        }
        if(m_test != 0)
        {
            m_Mf.sharedMesh = CatmullClark.Catmull_Clark(m_Mf.sharedMesh);
            Debug.Log("Catmull");
        }
        Debug.Log(MeshDisplayInfo2.ExportMeshCSV(m_Mf.sharedMesh));
    }

    // Update is called once per frame
    void Update()
    {
        if (m_debug)
        {
            m_Mf = GetComponent<MeshFilter>();
            Mesh mesh = m_Mf.sharedMesh;
            if(mesh.vertices[8] == mesh.vertices[22])
            {
                Debug.Log("why");
            }
        }
    }

    private void OnValidate()
    {
        if (m_test != 0)
        {
            for (int i = 0; i < m_test; i++)
                m_Mf.sharedMesh = CatmullClark.Catmull_Clark(m_Mf.sharedMesh);
            Debug.Log("Mesh");
        }
    }
}
