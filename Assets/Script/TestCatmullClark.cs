using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestCatmullClark : MonoBehaviour
{
    MeshFilter m_Mf;
    [Header("Parameters")]
    [SerializeField] int nb_iterations;
    Mesh m_base;
    // Start is called before the first frame update
    void Start()
    {
        m_Mf = GetComponent<MeshFilter>();
        if (nb_iterations == 0)
        {                  
            m_Mf.sharedMesh = CatmullClark.HalfEdgeToVertexFace(CatmullClark.VertexFaceToHalfEdge(m_Mf.sharedMesh));
            Debug.Log("Mesh");
        }
        if(nb_iterations != 0)
        {
            m_Mf.sharedMesh = CatmullClark.Catmull_Clark(m_Mf.sharedMesh, nb_iterations);
            Debug.Log("Catmull");
        }
        Debug.Log(MeshDisplayInfo.ExportMeshCSV(m_Mf.sharedMesh));
    }
}
