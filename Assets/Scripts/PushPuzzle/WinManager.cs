using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class WinManager : MonoBehaviour
{

    public ObjGoal[] objectsToCheck;
    public bool finalBool;

    private void Start()
    {
        finalBool = false;
    }

    private void Update()
    {
        
        if (objectsToCheck != null && objectsToCheck.Length > 0)
        {
            CheckBools();
        }
        else
        {
            Debug.LogWarning("No Objects assigned or found");
        }
        
        
    }

    void CheckBools()
    {

        foreach (ObjGoal scrpt in objectsToCheck)
        {
            
            if (scrpt == null)
            {
                Debug.LogError("An object in the objectsToCheck array is not assigned");
                finalBool = false;
                return;
            }
            
            if (!scrpt.GoalOn)
            {
                finalBool = false;
                return;
            }
        }

        finalBool = true;

        if(finalBool)
        {

            NextLevel();

        }

    }

    void NextLevel()
    {

        Debug.Log("AllBoolsTrue");

    }




}
