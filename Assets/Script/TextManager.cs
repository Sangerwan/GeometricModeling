using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextManager : MonoBehaviour
{

    public GameObject collisionPoint;
    // Start is called before the first frame update
    void Start()
    {
        float x = collisionPoint.transform.position.x;
        float y = collisionPoint.transform.position.y;
        float z = collisionPoint.transform.position.z;
        GetComponent<TextMesh>().text = "("+x+","+y+","+z+")";
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
