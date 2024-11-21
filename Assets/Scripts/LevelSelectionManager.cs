using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using static Cinemachine.DocumentationSortingAttribute;
using UnityEditor;
using Unity.VisualScripting;

public class LevelSelectionManager : MonoBehaviour
{
    public void loadWorld(string worldName)
    {
        SceneManager.LoadScene(worldName);
    }

    // Global Level Variables
    const int MAX_SIZE = 24;
    const int MAX_BLOCKS = 9;
    char[,] levelLayout = new char[MAX_SIZE, MAX_SIZE];
    short numWalls = 0;
    Vector2Int levelSize;
    int[,] blockLayer = new int[MAX_SIZE, MAX_SIZE];
    Vector2Int[,] blockPos = new Vector2Int[MAX_BLOCKS, MAX_BLOCKS];
    Vector2Int[] blockBuffer = new Vector2Int[8];
    short[] blockNums = new short[8];
    short numBlocks = 0;

    // GameObject References
    public Transform playerPos;
    public GameObject wallParent;
    public GameObject blockPrefab;
    public GameObject blockParentPrefab;

    GameObject[,] blockGOs = new GameObject[8, MAX_BLOCKS];
    GameObject[] blockParents = new GameObject[8];
    public GameObject[] wallGOs = new GameObject[MAX_SIZE * 4];
    public GameObject[] UIElements = new GameObject[5];

    // Color Properties
    [Header("Color Settings")]
    public Color WallColor = new Color(0.16f, 0.16f, 0.28f, 1.0f);
    public Color PlayerColor = new Color(0.811f, 0.243f, 0.243f, 1.0f);
    public Color BlockColor = new Color(0.337f, 0.267f, 0.62f, 1.0f);

    public Color Color = new Color(0.086f, 0.051f, 0.11f, 1.0f);
    public Color SpaceColor = new Color(0.6f, 0.6f, 0.741f, 1.0f);


    void Start()
    {
        string nextLevel = "Assets/Resources/Levels/LevelSelection.txt";
        readLevel(nextLevel);
    }

    void Update()
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
                    blockGOs[line[i] - 49, blockNums[line[i] - 49]].GetComponent<SpriteRenderer>().enabled = false;
                }
                else if (line[i] == 'P')
                {
                    //Debug.Log("Player Position- (" + i + ", " + vertI + ")");
                    playerPos.position = new Vector2(i, -vertI);
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
        levelFile.Close();
    }

    Color blockColor(char type)
    {
        if (type == 'W')
            return new Color(0.16f, 0.16f, 0.28f, 1.0f);
        else if (type == 'P')
            return new Color(0.811f, 0.243f, 0.243f, 1.0f);
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
                }
            }
            else
            {
                // Update Player Position
                playerPos.position = new Vector2(xOffset, -yOffset);
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
        for (int i = 0; i < blockNums[group - 1]; i++)
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
            Vector2 averagePosition = new Vector2(0f, 0f);
            for (int j = 0; j < blockNums[i]; j++)
            {
                // Updating Block Positions
                blockPos[i, j] += blockBuffer[i];

                // Updates Block Layer Position
                blockLayer[blockPos[i, j].y, blockPos[i, j].x] = i + 1;

                averagePosition.x += blockPos[i, j].x;
                averagePosition.y += blockPos[i, j].y;
            }
            // Move Block Parents
            blockParents[i].transform.position += new Vector3(blockBuffer[i].x, -blockBuffer[i].y, 0);

            // Move UI Elements
            averagePosition.x = (averagePosition.x/(float)blockNums[i]) - 8.5f;
            averagePosition.y = 5f - (averagePosition.y/(float)blockNums[i]);
            UIElements[i].GetComponent<RectTransform>().localPosition = (averagePosition * 100);

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
}
