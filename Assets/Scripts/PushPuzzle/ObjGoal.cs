using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjGoal : MonoBehaviour
{

    public bool GoalOn = false;
    public GameObject sprite;
    public LayerMask push;

    private void Update()
    {
        
        if (Physics2D.OverlapBox(transform.position, new Vector2(0.9f, 0.9f), 0f, push))
        {

            GoalOn = true;

        }
        else
        {
            GoalOn = false;
        }

        if (GoalOn)
        {
            sprite.GetComponent<SpriteRenderer>().color = Color.green;
        }
        else if (!GoalOn)
        {
            sprite.GetComponent<SpriteRenderer>().color = Color.yellow;
        }

    }

}
