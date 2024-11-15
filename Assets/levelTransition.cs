using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class levelTransition : MonoBehaviour
{
    public float currentPos;
    public float goalPos;
    public float moveSpeed;
    bool levelChanged = false;

    public TextLevels levelManager;

    void Start()
    {
        currentPos = -Screen.width;
        goalPos = -Screen.width;
    }

    void Update()
    {
        if (Mathf.Abs(goalPos - currentPos) >= (Screen.width * 0.05f))
        {
            currentPos = Mathf.MoveTowards(currentPos, goalPos, moveSpeed);
        }
        if (currentPos >= 0 && !levelChanged)
        {
            levelChanged = true;
            levelManager.nextLevel();
        }
        else if (levelChanged)
        {
            levelChanged = false;
            currentPos = -Screen.width;
            goalPos = -Screen.width;
            levelManager.transitioning = false;
        }
        gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(currentPos, 0);
    }
}
