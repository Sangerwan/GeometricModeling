using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class MeshGenerator : MonoBehaviour
{
    void explication()
    {


        // Start is called before the first frame update
        // genération auto face
        //nécessite 3 vertices qui forme les edges
        /* VERTEX-VERTEX
         * on donne les coordonnées des 3 points p1,p2,p3
         * et l'ordre des vertex avec les 3 points p1,p3,p2
         * 
         * VERTEX-FACE (unity)
         * Tableau de vertices (Vector3[]) taille N
         * Tableau de triangles (int[]) taille N*3
         *      On y donne les indices des points (dans le tableau de vertices) 
         *      Les trois premiers int correspondent aux indices des trois points de la première face/triangle
         *          *     
         *  Pour chaque triangle on peut commencer par n'importe quel points tant qu'on tourne dans le sens horaire 
         * */
    }
    delegate Vector3 ComputeVector3FromKxKz(float kX, float Kz);
    MeshFilter m_Mf;

    private void Awake()
    {
        m_Mf = GetComponent<MeshFilter>();
        //m_Mf.sharedMesh = CreateTriangle();
        //m_Mf.sharedMesh = CreatePlane(new Vector3(4,0,6),8,8);
        
        m_Mf.sharedMesh = WrapNormalizeCreatePlane(5, 5,
            (kX, kZ) => new Vector3((kX - .5f) * 4.0f, 0, (kZ - .5f) * 2)
            /*
            (kX, kZ) =>
            {
                float theta = kX * 2 * Mathf.PI;
                float z = kZ * 4;
                float rho = 2;
                return new Vector3(rho * Mathf.Cos(theta), z, rho * Mathf.Sin(theta));
            }
            */
            /*
            (kX, kZ) =>
            {
                float theta = kX * 2 * Mathf.PI;
                float phi = (1-kZ) * Mathf.PI;
                float rho = 2;
                return new Vector3(rho * Mathf.Cos(theta) * Mathf.Sin(phi), rho * Mathf.Cos(phi), rho * Mathf.Sin(theta) * Mathf.Sin(phi));
            }*/

         );
         Debug.Log(MeshDisplayInfo.ExportMeshCSV(m_Mf.sharedMesh));
        // m_Mf.sharedMesh = WrapNormalizeCreatePlane(5, 5, (kX, kZ) => { return new Vector3(0, 0, 0); });
        gameObject.AddComponent<MeshCollider>();

    }

    Mesh CreateTriangle()
    {

        Mesh newMesh = new Mesh();
        newMesh.name = "triangle";
        Vector3[] vertices = new Vector3[3];
        int[] triangles = new int[1 * 3];
        vertices[0] = Vector3.right;
        vertices[1] = Vector3.up;
        vertices[2] = Vector3.forward;

        triangles[0] = 0;
        triangles[1] = 1;
        triangles[2] = 2;

        newMesh.vertices = vertices;
        newMesh.triangles = triangles;
        // on recalcule la bounding box du mesh 
        // C'est pour agir sur la boite englobante de l'objet 3D, pour le calcul de visibilité de la caméra
        newMesh.RecalculateBounds();


        return newMesh;
    }

    Mesh CreateQuadXZ(Vector3 size)
    {
        Vector3 halfSize = size * .5f;

        Mesh newMesh = new Mesh();
        newMesh.name = "quad";

        Vector3[] vertices = new Vector3[4];
        int[] triangles = new int[2 * 3];

        vertices[0] = new Vector3(-halfSize.x,0,-halfSize.z);
        vertices[1] = new Vector3(-halfSize.x, 0, halfSize.z);
        vertices[2] = new Vector3(halfSize.x, 0, halfSize.z);
        vertices[3] = new Vector3(halfSize.x, 0, -halfSize.z);

        triangles[0] = 0;
        triangles[1] = 1;
        triangles[2] = 2;

        triangles[3] = 0;
        triangles[4] = 2;
        triangles[5] = 3;

        newMesh.vertices = vertices;
        newMesh.triangles = triangles;
        // on recalcule la bounding box du mesh 
        // C'est pour agir sur la boite englobante de l'objet 3D, pour le calcul de visibilité de la caméra
        newMesh.RecalculateBounds();


        return newMesh;
    }

    Mesh CreateStrip(Vector3 size,int nSegments)
    {
        Vector3 halfSize = size * .5f;

        Mesh newMesh = new Mesh();
        newMesh.name = "strip";

        Vector3[] vertices = new Vector3[(nSegments+1)*2];
        int[] triangles = new int[nSegments  * 2 * 3];
        //vertices
        for (int i=0;i< (nSegments + 1); i++){
            vertices[i*2] = new Vector3(-halfSize.x+ (size.x*i/ (nSegments + 1)), 0, halfSize.z);
            vertices[i*2+1] = new Vector3(-halfSize.x + (size.x * i / (nSegments + 1)), 0, -halfSize.z);
        }
        //triangles
        int compt = 0;
        for (int i = 0; i <triangles.Length; i=i+6)
        {
            
            triangles[i] = 0 + compt;
            triangles[i + 1] = 2 + compt;
            triangles[i + 2] = 1 + compt;

            triangles[i + 3] = 1 + compt;
            triangles[i + 4] = 2 + compt;
            triangles[i + 5] = 3 + compt;
            compt += 2;
        }
        

        newMesh.vertices = vertices;
        newMesh.triangles = triangles;
        // on recalcule la bounding box du mesh 
        // C'est pour agir sur la boite englobante de l'objet 3D, pour le calcul de visibilité de la caméra
        newMesh.RecalculateBounds();


        return newMesh;
    }

    Mesh CreatePlane(Vector3 size, int nSegmentsX, int nSegmentsZ)
    {
        Vector3 halfSize = size * .5f;

        Mesh newMesh = new Mesh();
        newMesh.name = "plane";

        Vector3[] vertices = new Vector3[(nSegmentsX + 1) * (nSegmentsZ + 1)];
        int[] triangles = new int[nSegmentsX* nSegmentsZ * 2 * 3];
        //vertices
        int index = 0;
        for (int i = 0; i < (nSegmentsX + 1); i++)
        {
            //float kx = (float)i / nSegmentsX;
            for (int j = 0; j < (nSegmentsZ + 1); j++)
            {               
                //float kz = (float)j / nSegmentsZ;
                //vertices[i * nSegmentsZ + j];
                vertices[index++] = new Vector3(-halfSize.x + (size.x * i / nSegmentsX),
                                                                 0,
                                                                 halfSize.z - (size.z * j / nSegmentsZ) );
            }
        }
        //triangles
        int compt = 0;
        index = 0;
        for (int i = 0; i < nSegmentsX ; i++)
        {
            for (int j = 0; j < nSegmentsZ; j++)
            {
                compt = j + i * (nSegmentsZ + 1);

                triangles[index] = 0 + compt;
                triangles[index + 1] = nSegmentsZ + 1 + 0 + compt;
                triangles[index + 2] = 1 + compt;

                triangles[index + 3] = 1 + compt;
                triangles[index + 4] = nSegmentsZ + 1 + 0 + compt;
                triangles[index + 5] = nSegmentsZ + 1 + 1 + compt;

                index += 6;
            }
        }
      
        newMesh.vertices = vertices;
        newMesh.triangles = triangles;
        // on recalcule la bounding box du mesh 
        // C'est pour agir sur la boite englobante de l'objet 3D, pour le calcul de visibilité de la caméra
        newMesh.RecalculateBounds();

        return newMesh;
    }

    Mesh WrapNormalizeCreatePlane(int nSegmentsX, int nSegmentsZ, ComputeVector3FromKxKz computePosition)
    {
        Mesh newMesh = new Mesh();
        newMesh.name = "wrapNormalizedPlane";

        Vector3[] vertices = new Vector3[(nSegmentsX + 1) * (nSegmentsZ + 1)];
        int[] triangles = new int[nSegmentsX * nSegmentsZ * 2 * 3];
        //vertices
        int index = 0;
        for (int i = 0; i < (nSegmentsX + 1); i++)
        {
            float kx = (float)i / nSegmentsX;
            for (int j = 0; j < (nSegmentsZ + 1); j++)
            {
                float kz = (float)j / nSegmentsZ;
                //vertices[i * nSegmentsZ + j];
                //vertices[i * nSegmentsZ + j] = new Vector3(-halfSize.x + (size.x * i / nSegmentsX),
                //                                                0,
                //                                              halfSize.z - (size.z * j / nSegmentsZ));
                //mathf.Lerp()
                vertices[index++] = computePosition(kx, kz);
            }
        }
        //triangles
        int compt = 0;
        index = 0;
        for (int i = 0; i < nSegmentsX; i++)
        {
            for (int j = 0; j < nSegmentsZ; j++)
            {
                compt = j + i * (nSegmentsZ+1);

                triangles[index] = 0 + compt;
                triangles[index + 1] = 1 + compt; 
                triangles[index + 2] = nSegmentsZ + 1 + 0 + compt;

                triangles[index + 3] = 1 + compt;
                triangles[index + 4] = nSegmentsZ + 1 + 1 + compt; 
                triangles[index + 5] = nSegmentsZ + 1 + 0 + compt;
                index += 6;
            }
        }

        newMesh.vertices = vertices;
        newMesh.triangles = triangles;
        // on recalcule la bounding box du mesh 
        // C'est pour agir sur la boite englobante de l'objet 3D, pour le calcul de visibilité de la caméra
        newMesh.RecalculateBounds();

        return newMesh;
    }
    Mesh WrapNormalizeCreatePlaneQuads(int nSegmentsX, int nSegmentsZ, ComputeVector3FromKxKz computePosition)
    {
        Mesh newMesh = new Mesh();
        newMesh.name = "wrapNormalizedPlane";

        Vector3[] vertices = new Vector3[(nSegmentsX + 1) * (nSegmentsZ + 1)];
        int[] quads = new int[nSegmentsX * nSegmentsZ * 3];
        //vertices
        int index = 0;
        for (int i = 0; i < (nSegmentsX + 1); i++)
        {
            float kx = (float)i / nSegmentsX;
            for (int j = 0; j < (nSegmentsZ + 1); j++)
            {
                float kz = (float)j / nSegmentsZ;
                vertices[index++] = computePosition(kx, kz);
            }
        }
        //triangles
        int compt = 0;
        index = 0;
        for (int i = 0; i < nSegmentsX; i++)
        {
            for (int j = 0; j < nSegmentsZ; j++)
            {
                compt =  (nSegmentsZ + 1);

                quads[index++] = 0 + i* compt +j;
                quads[index++] = 1 + i * compt* + j;
                quads[index++] = 1 + 0 + (i+1)*compt + j;
                quads[index++] = 0 + 0 + (i+1)*compt + j;

                index += 4;
            }
        }

        newMesh.vertices = vertices;
        newMesh.SetIndices(quads,new MeshTopology(),0);


        newMesh.RecalculateBounds();

        return newMesh;
    }
    /*
    Mesh CreateRegularPolygonXZQuads(float radius, int nQuads)

    {

        Mesh newMesh = new Mesh();

        newMesh.name = "RegularPolygonQuads";


        Vector3[] vertices = new Vector3[??];

        int[] quads = new int[nQuads * 4];


        vertices[0] = Vector3.zero;


        //Vertices

        for (int i = 0; i < ???; i++)

        {

        }


        //Quads

        int index = 0;

        for (int i = 0; i < nQuads; i++)

        {

            quads[index++] =????;

            quads[index++] =????;

            quads[index++] =????;

            quads[index++] =????;

        }


        newMesh.vertices = vertices;

        newMesh.SetIndices(quads, MeshTopology.Quads, 0);

        newMesh.RecalculateBounds();

        newMesh.RecalculateNormals();

        return newMesh;

    }*/
}
