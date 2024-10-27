using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuUI : MonoBehaviour
{

    public void Play()
    {

        SceneManager.LoadScene("Platformer");

    }

    public void Quit()
    {
        
        Application.Quit();
        
    }

    public void PlayPuzzle()
    {

        SceneManager.LoadScene("GridBased");

    }
    
}
