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
        currentPos = -1280;
        goalPos = -1280;
    }

    void FixedUpdate()
    {
        if (Mathf.Abs(goalPos - currentPos) >= (Screen.width * 0.01f))
        {
            currentPos = Mathf.MoveTowards(currentPos, goalPos, moveSpeed);
        }
        else if (levelChanged)
        {
            levelChanged = false;
            currentPos = -1280;
            goalPos = -1280;
            levelManager.transitioning = false;
        }

        if (currentPos >= 0 && !levelChanged)
        {
            levelChanged = true;
            levelManager.nextLevel();
        }

        gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(currentPos, 0);
    }
}
