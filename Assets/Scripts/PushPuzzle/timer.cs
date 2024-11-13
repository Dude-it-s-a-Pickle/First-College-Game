using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class timer : MonoBehaviour
{
    public GridMovement movement;
    public WinManager Winstate;

    public GameObject loseWindow;

    private Scene currentScene;

    public float timerTime;

    public TextMeshProUGUI timerText;

    private bool canSkip;

    private void Start()
    {
        currentScene = SceneManager.GetActiveScene();

        loseWindow.SetActive(false);

    }


    void Update()
    {

        if (canSkip)
        {
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Escape))
            {

                SceneManager.LoadScene(currentScene.name);

            }
        }

    }

    private void FixedUpdate()
    {

        if (timerTime <= 0 && !Winstate.finalBool)
        {

            StartCoroutine(loseState());

        }
        else if (!Winstate.finalBool)
        {

            timerTime--;

            timerText.text = timerTime.ToString();

        }

    }

    public IEnumerator loseState()
    {
        canSkip = true;
        movement.active = false;
        loseWindow.SetActive(true);
        yield return new WaitForSeconds(3);
        SceneManager.LoadScene(currentScene.name);

    }
}
