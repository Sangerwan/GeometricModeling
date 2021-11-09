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
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
