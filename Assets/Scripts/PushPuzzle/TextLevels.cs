using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;


public class TextLevels : MonoBehaviour
{
    // Global Level Variables
    const int MAX_SIZE = 24;
    const int MAX_BLOCKS = 8;
    char[,] levelLayout = new char[MAX_SIZE, MAX_SIZE];
    short numWalls = 0;
    Vector2Int levelSize;
    Vector2Int[] goalPos = new Vector2Int[MAX_BLOCKS];
    short numGoals = 0;
    int[,] blockLayer = new int[MAX_SIZE, MAX_SIZE];
    Vector2Int[,] blockPos = new Vector2Int[MAX_BLOCKS, MAX_BLOCKS];
    Vector2Int[] blockBuffer = new Vector2Int[8];
    short[] blockNums = new short[8];
    short numBlocks = 0;
    public bool transitioning = false;
    short levelNum = 1;

    // GameObject References
    public Transform playerPos;
    public GameObject wallParent;
    public GameObject goalParent;
    public GameObject blockPrefab;
    public GameObject blockParentPrefab;
    public GameObject cameraObj;
    public GameObject playerGoal;
    public levelTransition levelTransition;

    GameObject[,] blockGOs = new GameObject[8, MAX_BLOCKS];
    GameObject[] blockParents = new GameObject[8];
    public GameObject[] wallGOs = new GameObject[MAX_SIZE * 4];
    public GameObject[] goalGOs = new GameObject[8];

    // Color Properties
    [Header("Color Settings")]
    public Color WallColor =  new Color(0.16f, 0.16f, 0.28f, 1.0f);
    public Color PlayerColor =  new Color(0.811f, 0.243f, 0.243f, 1.0f);
    public Color BlockGoalColor =  new Color(0.294f, 0.678f, 0.329f, 1.0f);
    public Color EmptyBlockGoalColor =  new Color(0.133f, 0.2f, 0.141f, 1.0f);
    public Color PlayerGoalColor =  new Color(0.91f, 0.647f, 0.239f, 1.0f);
    public Color EmptyPlayerGoal =  new Color(0.15f, 0.1f, 0.0f, 1.0f);
    public Color BlockColor = new Color(0.337f, 0.267f, 0.62f, 1.0f);

    public Color Color =  new Color(0.086f, 0.051f, 0.11f, 1.0f);
    public Color  SpaceColor =  new Color(0.6f, 0.6f, 0.741f, 1.0f);


    void Start()
    {
        string nextLevel = "Assets/Levels/lvl" + (levelNum / 5) + "-" + ((levelNum % 5) + 1) + ".txt";
        readLevel(nextLevel);
    }

    void Update()
    {
        if (!transitioning)
        {
            if (Input.GetKeyDown(KeyCode.W))
                movePlayer('U');
            if (Input.GetKeyDown(KeyCode.S))
                movePlayer('D');
            if (Input.GetKeyDown(KeyCode.A))
                movePlayer('L');
            if (Input.GetKeyDown(KeyCode.D))
                movePlayer('R');
        }
    }

    // Game Functions
    void readLevel(String levelName)
    {
        //StreamReader levelFile = new StreamReader("C:\\Users\\gapam\\Desktop\\TestFileReader\\TestFileReader\\levels\\lvl0-1.txt");
        StreamReader levelFile = new StreamReader(levelName);
        String line;
        int vertI = 0;
        int horizI = 0;

        // Assignment Loop
        line = levelFile.ReadLine();
        while (line != null)
        {
            //Debug.Log("Line Read: " + line);
            for (int i = 0; i < line.Length; i++)
            {
                // Block Checks
                if (line[i] >= 49 && (line[i]) <= 57)
                {
                    // Adding Block position
                    blockPos[line[i] - 49, blockNums[line[i] - 49]].x = i;
                    blockPos[line[i] - 49, blockNums[line[i] - 49]].y = vertI;
                    blockNums[line[i] - 49]++;
                    levelLayout[vertI, i] = '0';
                    blockLayer[vertI, i] = line[i] - 48;
                    horizI++;

                    // Creating Block Parent
                    if (blockParents[line[i] - 49] == null)
                    {
                        blockParents[line[i] - 49] = Instantiate(blockParentPrefab, new Vector3(0, 0, 0), Quaternion.identity);
                        blockParents[line[i] - 49].name = ("Block Group " + (line[i] - 48));
                    }

                    // Creating Block GameObject
                    blockGOs[line[i] - 49, blockNums[line[i] - 49]] = Instantiate(blockPrefab, new Vector3(i, -vertI, 0), Quaternion.identity, blockParents[line[i] - 49].transform);
                    blockGOs[line[i] - 49, blockNums[line[i] - 49]].GetComponent<SpriteRenderer>().color = blockColor((char)(line[i] - 49));
                }
                else if (line[i] == 'P')
                {
                    //Debug.Log("Player Position- (" + i + ", " + vertI + ")");
                    playerPos.position = new Vector2(i, -vertI);
                    levelLayout[vertI, i] = '0';
                    blockLayer[vertI, i] = 0;
                    horizI++;
                }
                else if (line[i] == 'G')
                {
                    goalPos[numGoals].x = i;
                    goalPos[numGoals].y = vertI;

                    // Creating Goal GameObject
                    goalGOs[numGoals] = Instantiate(blockPrefab, new Vector3(i, -vertI, 0), Quaternion.identity, goalParent.transform);
                    goalGOs[numGoals].name = ("Goal " + (numGoals + 1));
                    goalGOs[numGoals].transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
                    goalGOs[numGoals].GetComponent<SpriteRenderer>().color = blockColor('F');
                    goalGOs[numGoals].GetComponent<SpriteRenderer>().sortingOrder = 5;

                    levelLayout[vertI, i] = '0';
                    blockLayer[vertI, i] = 0;
                    numGoals++;
                    horizI++;
                }
                else if (line[i] == 'J')
                {
                    playerGoal = Instantiate(blockPrefab, new Vector3(i, -vertI, 0), Quaternion.identity);
                    playerGoal.name = ("Player Goal");
                    playerGoal.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
                    playerGoal.GetComponent<SpriteRenderer>().color = blockColor('K');
                    playerGoal.GetComponent<SpriteRenderer>().sortingOrder = 5;

                    levelLayout[vertI, i] = '0';
                    blockLayer[vertI, i] = 0;
                    horizI++;
                }
                else if (line[i] == 'W')
                {
                    //Debug.Log("Wall created at " + "(" + i + ", " + vertI + ")");
                    wallGOs[numWalls] = Instantiate(blockPrefab, new Vector3(i, -vertI, 0), Quaternion.identity, wallParent.transform);
                    wallGOs[numWalls].GetComponent<SpriteRenderer>().color = blockColor('W');
                    numWalls++;

                    levelLayout[vertI, i] = 'W';
                    blockLayer[vertI, i] = 0;
                    horizI++;
                }
                else
                {
                    levelLayout[vertI, i] = line[i];
                    blockLayer[vertI, i] = 0;
                    horizI++;
                }
            }
            levelSize.x = horizI;
            horizI = 0;
            vertI++;
            line = levelFile.ReadLine();
        }
        levelSize.y = vertI;

        // Updating Block Count
        for (int i = 0; i < 8; i++)
        {
            if (blockNums[i] == 0)
                break;
            else
                numBlocks++;
        }

        // Setting Camera Size Factor
        if ((levelSize.x / levelSize.y) >= (16f / 9f))
            cameraObj.GetComponent<Camera>().orthographicSize = (levelSize.x) * 0.5f * (9f / 16f);
        else
            cameraObj.GetComponent<Camera>().orthographicSize = (levelSize.y) * 0.5f;

        // Setting Camera Position
        cameraObj.transform.position = new Vector3((levelSize.x - 1) * 0.5f, (levelSize.y - 1) * -0.5f, -10f);

        levelFile.Close();
    }
    
    Color blockColor(char type)
    {
        if (type == 'W')
            return new Color(0.16f, 0.16f, 0.28f, 1.0f);
        else if (type == 'P')
            return new Color(0.811f, 0.243f, 0.243f, 1.0f);
        else if (type == 'G')
            return new Color(0.294f, 0.678f, 0.329f, 1.0f);
        else if (type == 'F')
            return new Color(0.133f, 0.2f, 0.141f, 1.0f);
        else if (type == 'J')
            return new Color(0.91f, 0.647f, 0.239f, 1.0f);
        else if (type == 'K')
            return new Color(0.15f, 0.1f, 0.0f, 1.0f);
        else if (type == 'B')
            return new Color(0.086f, 0.051f, 0.11f, 1.0f);
        else if (type == 'S')
            return new Color(0.6f, 0.6f, 0.741f, 1.0f);

        else if (type != '0') // Varying Block Colors
        {
            float bColor = 0.1f * (type) + 1.0f;
            return new Color(0.337f * bColor, 0.267f * bColor, 0.62f * bColor, 1.0f);
        }

        else
            return new Vector4(0.22f, 0.22f, 0.341f, 1.0f);
    }

    void movePlayer(char direction)
    {
        int xOffset = (int)playerPos.position.x, yOffset = -(int)playerPos.position.y;

        // Setting Check Direction
        if (direction == 'U')
            yOffset += -1;
        else if (direction == 'D')
            yOffset += 1;
        else if (direction == 'L')
            xOffset += -1;
        else if (direction == 'R')
            xOffset += 1;

        // Checks for walls
        if (levelLayout[yOffset, xOffset] != 'W')
        {
            // Check for Blocks
            if (blockLayer[yOffset, xOffset] >= 1 && blockLayer[yOffset, xOffset] <= 9)
            {
                if (checkBlocks(blockLayer[yOffset, xOffset], direction))
                {
                    // Update Player Position
                    playerPos.position = new Vector2(xOffset, -yOffset);

                    // Update Block Positions
                    updateBlockBuffer();

                    // Checking Goal(s) Status
                    checkGoal();
                }
            }
            else
            {
                // Update Player Position
                playerPos.position = new Vector2(xOffset, -yOffset);

                // Checking Goal(s) Status
                checkGoal();
            }
        }
    }

    bool checkBlocks(int group, char direction)
    {
        bool canMove = true;
        int xOffset = 0, yOffset = 0;

        // Setting Check Direction
        if (direction == 'U')
            yOffset = -1;
        else if (direction == 'D')
            yOffset = 1;
        else if (direction == 'L')
            xOffset = -1;
        else if (direction == 'R')
            xOffset = 1;

        // Checking for movability
        for (int i = 0; i < blockNums[group-1]; i++)
        {
            int xPos = blockPos[group - 1, i].x + xOffset,
                yPos = blockPos[group - 1, i].y + yOffset;

            // Check for Wall Collisions
            if (levelLayout[yPos, xPos] == 'W')
            {
                canMove = false;
            }

            // Check for Block Collisions
            else if ((blockLayer[yPos, xPos] >= 1 && blockLayer[yPos, xPos] <= 9) && blockLayer[yPos, xPos] != group)
            {
                if (checkBlocks(blockLayer[yPos, xPos], direction))
                    canMove = true;
                else
                    return false;
            }
        }
        // No Collisions
        if (canMove)
            addBlockBuffer(group, direction);
        else
            clearBlockBuffer();

        return canMove;
    }

    void addBlockBuffer(int group, char direction)
    {
        // Adds group movement
        if ((blockBuffer[group - 1].x + blockBuffer[group - 1].y) == 0) // Makes sure buffer isn't slready written
        {
            if (direction == 'U')
                blockBuffer[group - 1].y -= 1;
            else if (direction == 'D')
                blockBuffer[group - 1].y += 1;
            else if (direction == 'L')
                blockBuffer[group - 1].x -= 1;
            else if (direction == 'R')
                blockBuffer[group - 1].x += 1;
        }
    }

    void updateBlockBuffer()
    {
        for (int i = 0; i < numBlocks; i++)
        {
            if ((blockBuffer[i].x + blockBuffer[i].y) != 0)
            {
                // Clearing Block Positions
                for (int j = 0; j < blockNums[i]; j++)
                {
                    if (blockLayer[blockPos[i, j].y, blockPos[i, j].x] != i)
                        blockLayer[blockPos[i, j].y, blockPos[i, j].x] = 0;
                }
            }
        }
        for (int i = 0; i < numBlocks; i++)
        {
            // Replacing Blocks
            for (int j = 0; j < blockNums[i]; j++)
            {
                // Updating Block Positions
                blockPos[i, j] += blockBuffer[i];

                // Updates Block Layer Position
                blockLayer[blockPos[i, j].y, blockPos[i, j].x] = i + 1;
            }
            // Move Block Parents
            blockParents[i].transform.position += new Vector3(blockBuffer[i].x, -blockBuffer[i].y, 0);

            // Resets Buffer
            blockBuffer[i].x = 0;
            blockBuffer[i].y = 0;
        }
    }

    void clearBlockBuffer()
    {
        for (int i = 0; i < numBlocks; i++)
        {
            // Resets Buffer
            blockBuffer[i].x = 0;
            blockBuffer[i].y = 0;
        }
    }

    void checkGoal()
    {
        bool allGoals = true;

        // Block Goal Checks
        for (int i = 0; i < numGoals; i++)
        {
            if ((blockLayer[goalPos[i].y, goalPos[i].x] != 0))
                goalGOs[i].GetComponent<SpriteRenderer>().color = blockColor('G');
            else
            {
                goalGOs[i].GetComponent<SpriteRenderer>().color = blockColor('F');
                allGoals = false;
            }
        }

        // Player Goal Checks
        if (playerGoal != null)
        {
            if (playerPos.position == playerGoal.transform.position)
                playerGoal.GetComponent<SpriteRenderer>().color = blockColor('J');
            else
            {
                playerGoal.GetComponent<SpriteRenderer>().color = blockColor('K');
                allGoals = false;
            }
        }

        // Win Check
        if (allGoals)
        {
            transitioning = true;
            levelTransition.goalPos = Screen.width;
        }
    }

    void deleteGameObject(GameObject[] GO)
    {
        for (int i = 0; i < GO.Length; i++)
        {
            if (GO[i] == null)
                break;
            else
            {
                GO[i].GetComponent<TileManager>().deleteTile();
                GO[i] = null;
            }
        }
    }

    void clearData()
    {
        // Resetting Arrays
        Array.Clear(levelLayout, 0, levelLayout.Length);
        Array.Clear(blockLayer, 0, blockLayer.Length);
        Array.Clear(blockPos, 0, blockPos.Length);
        Array.Clear(blockBuffer, 0, blockBuffer.Length);
        Array.Clear(blockNums, 0, blockNums.Length);
        Array.Clear(goalPos, 0, goalPos.Length);

        // Deleting GameObjects
        deleteGameObject(wallGOs);
        Array.Clear(wallGOs, 0, wallGOs.Length);
        deleteGameObject(goalGOs);
        Array.Clear(goalGOs, 0, goalGOs.Length);
        deleteGameObject(blockParents);
        Array.Clear(blockParents, 0, blockParents.Length);
        Destroy(playerGoal);

        /*for (int i = 0; i < blockGOs.GetLength(0); i++)
        {
            for (int j = 0; j < blockGOs.GetLength(1); j++)
            {
                if (blockGOs[i, j] == null)
                    break;
                else
                {
                    blockGOs[i, j].GetComponent<TileManager>().deleteTile();
                    blockGOs[i, j] = null;
                }
            }
        }*/

        // Reseting Numbers
        numBlocks = 0;
        numGoals = 0;
        numWalls = 0;

        // Don't need to reset
        /*scaleFactor = 0.0f;
        levelSize[0] = 0;
        levelSize[1] = 0;
        playerPos.position = new Vector2(0);*/
    }

    public void nextLevel()
    {
        clearData();
        levelNum++;
        string nextLevel = "Assets/Levels/lvl" + (levelNum / 5) + "-" + ((levelNum % 5) + 1) + ".txt";
        readLevel(nextLevel);
    }
}