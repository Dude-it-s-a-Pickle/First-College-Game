using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjGoal : MonoBehaviour
{

    public bool GoalOn = false;
    public bool box = true;
    public GameObject sprite;
    public LayerMask ObjforGoal;

    private void Start()
    {
        if(box == true)
        {
            ObjforGoal = LayerMask.GetMask("Pushable");
        }
        else if (box == false)
        {
            ObjforGoal = LayerMask.GetMask("Wall");
        }
    }

    private void Update()
    {
        
        if (Physics2D.OverlapBox(transform.position, new Vector2(0.9f, 0.9f), 0f, ObjforGoal))
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

            if (box == true)
            {
                sprite.GetComponent<SpriteRenderer>().color = Color.yellow;
            }
            else if (box == false)
            {
                sprite.GetComponent<SpriteRenderer>().color = new Color(1.0f, 0.5f, 0.0f);
            }
            
        }

    }

}
