/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
[Serializable]

public class GameStates
{
    #region dataToSave
    public Vector3 playerPos;
    public Sprite playerSprite;
    public List<Vector3> boxPosition;
    #endregion
    public static GameStates GetCurrentState()
    {
        GameStates gameStateToSave = new GameStates();
        SavedElements[] elementsToSaveOnScene = GameObject.FindObjectsOfType<SavedElements>();
        gameStateToSave.boxPositions = new List<Vector3>();
        foreach(SavedElements element in elementsToSaveOnScene)
        {
            if (element.type == SavedElements.Type.Player)
            {
                gameStateToSave.playerPos = element.transform.position;
                gameStateToSave.playerSprite = element.transform.GetComponent<SpriteRenderer>().sprite;
            }
            else if (element.type == SavedElement.Type.Box)
            {
                gameStateToSave.boxPositions.Add(element.transform.position);
            }
        }
        return gameStateToSave;
    }
    public void LoadGameState()
    {
        SavedElements[] elementsToLoadOnScene = GameObject.FindObjectsOfType<SavedElements>();
        List<Vector3> remainingBoxPosition = new List<Vector3>(boxPosition);
        foreach(SavedElements elementToload in elementsToLoadOnscene)
        {
            if (elementToload.type == SavedElements.Type.Player)
            {
                elementToload.transform.position = playerPos;
                elementToload.GetComponent<SpriteRenderer>().sprite = playerSprite;
                elementToload.GetComponent<Player>().UndoSpriteIndex();
            }
            else if(elementToload.type == SavedElements.Type.Box)
            {
                elementToload.transform.position = remainingBoxPosition[0];
                remainingBoxPosition.RemoveAt(0);
            }
        }
    }
}
*/