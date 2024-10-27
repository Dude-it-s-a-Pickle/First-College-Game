using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallMove : MonoBehaviour
{

    public float inverse;
    
    // Start is called before the first frame update
    void Start()
    {
        


    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKey(KeyCode.W))
        {
            this.transform.position += new Vector3(0, (inverse), 0);
        }
        if (Input.GetKey(KeyCode.S))
        {
            this.transform.position += new Vector3(0, -(inverse), 0);
        }
        if (Input.GetKey(KeyCode.D))
        {
            this.transform.position += new Vector3((inverse), 0, 0);
        }
        if (Input.GetKey(KeyCode.A))
        {
            this.transform.position += new Vector3(-(inverse), 0, 0);
        }

    }
}
