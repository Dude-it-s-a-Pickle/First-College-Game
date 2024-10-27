using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{

    public Score ScoreManagerRef;

    // Start is called before the first frame update
    void Start()
    {
        ScoreManagerRef = FindObjectOfType<Score>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {

        var player = other.gameObject.GetComponent<PlayerMovement>();

        if (player != null)
        {
            ScoreManagerRef.Increment();
            Destroy(this.gameObject);
        }

    }
}
