using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spin : MonoBehaviour
{
    
    void Update()
    {
        
        this.transform.Rotate(new Vector3(0, 1, 0), Time.deltaTime * 10);

    }
}
