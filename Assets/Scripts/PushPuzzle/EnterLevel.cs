using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EnterLevel : MonoBehaviour
{

    public LayerMask Player;

    public string levelName;

    private void Update()
    {

        if (Physics2D.OverlapBox(transform.position, new Vector2(0.9f, 0.9f), 0f, Player))
        {

            SceneManager.LoadScene(levelName);

        }
        

    }

}
