using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TutorialText : MonoBehaviour
{
    public TextMeshProUGUI textBox;

    // Start is called before the first frame update
    void Start()
    {
        textBox.text = "Use arrow keys to move around the player and blocks!";
        StartCoroutine(tutorialText());
    }

    public IEnumerator tutorialText()
    {
        yield return new WaitForSeconds(3);

        textBox.text = "Push blocks into all the goals (small yellow boxes)";
        yield return new WaitForSeconds(3);

        textBox.text = "Put players into all the goals (small orange boxes)";
        yield return new WaitForSeconds(3);

        gameObject.SetActive(false);
    }
}
