using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager.UI;
using UnityEngine;
using System.IO;
using System;


public class TextLevels : MonoBehaviour
{
    // Global Level Variables
    const int MAX_SIZE = 24;
    const int MAX_BLOCKS = 8;
    char[,] levelLayout = new char[MAX_SIZE, MAX_SIZE];
    float scaleFactor = (1.0f / 9.0f);
    Vector2Int levelSize;
    Vector2Int[] goalPos = new Vector2Int[MAX_BLOCKS];
    short numGoals = 0;
    bool[] goalStatus = { false, false, false, false, false, false, false, false }; // Fix This System
    int[,] blockLayer = new int[MAX_SIZE, MAX_SIZE];
    Vector2Int[,] blockPos = new Vector2Int[MAX_BLOCKS, MAX_BLOCKS];
    Vector2Int[] blockBuffer = new Vector2Int[8];
    short[] blockNums = new short[8];
    short numBlocks = 0;
    bool transitioning = false;
    short levelNum = 0;

    // GameObject References
    public Transform playerPos;
    public Transform wallParent;
    public GameObject blockPrefab;
    GameObject[,] blockGOs = new GameObject[8, MAX_BLOCKS];
    GameObject[] wallGOs = new GameObject[MAX_SIZE * 4];


    void Start()
    {
        readLevel("Assets/Levels/lvl0-1.txt");
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

                    // Creating Block GameObject
                    //Debug.Log("Block created at " + "(" + i + ", " + vertI + ")");
                    blockGOs[line[i] - 49, blockNums[line[i] - 49]] = Instantiate(blockPrefab, new Vector3(i, vertI, 0), Quaternion.identity);
                    blockGOs[line[i] - 49, blockNums[line[i] - 49]].GetComponent<SpriteRenderer>().color = blockColor((char)(line[i] - 49));
                }
                else if (line[i] == 'P')
                {
                    //Debug.Log("Player Position- (" + i + ", " + vertI + ")");
                    playerPos.position = new Vector2(i, vertI);
                    levelLayout[vertI, i] = '0';
                    blockLayer[vertI, i] = 0;
                    horizI++;
                }
                else if (line[i] == 'G')
                {
                    goalPos[numGoals].x = i;
                    goalPos[numGoals].y = vertI;

                    levelLayout[vertI, i] = '0';
                    blockLayer[vertI, i] = 0;
                    horizI++;
                }
                else if (line[i] == 'W')
                {
                    //Debug.Log("Wall created at " + "(" + i + ", " + vertI + ")");
                    wallGOs[i] = Instantiate(blockPrefab, new Vector3(i, vertI, 0), Quaternion.identity, wallParent);
                    wallGOs[i].GetComponent<SpriteRenderer>().color = blockColor('W');

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
        levelSize.y = vertI + 1;

        // Updating Block Count
        for (int i = 0; i < 8; i++)
        {
            if (blockNums[i] == 0)
                break;
            else
                numBlocks++;
        }

        // Setting Scale Factor
        if (levelSize.x >= levelSize.y)
            scaleFactor = (1.0f / (levelSize.x));
        else
            scaleFactor = (1.0f / (levelSize.y));

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
        int xOffset = (int)playerPos.position.x, yOffset = (int)playerPos.position.y;

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
                    playerPos.position = new Vector2(xOffset, yOffset);

                    // Update Block Positions
                    updateBlockBuffer();

                    // Checking Goal(s) Status
                    checkGoal();
                }
            }
            else
            {
                // Update Player Position
                playerPos.position = new Vector2(xOffset, yOffset);

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
        for (int i = 0; i < blockPos.GetLength(group - 1); i++)
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
                for (int j = 0; j < blockPos.GetLength(i); j++)
                {
                    if (blockLayer[blockPos[i, j].y, blockPos[i, j].x] != i)
                        blockLayer[blockPos[i, j].y, blockPos[i, j].x] = 0;
                }
            }
        }
        for (int i = 0; i < numBlocks; i++)
        {
            // Replacing Blocks
            for (int j = 0; j < blockPos.GetLength(i); j++)
            {
                // Updating Block Positions
                blockPos[i, j] += blockBuffer[i];

                // Updating Block GameObjects
                blockGOs[i, j].transform.position = new Vector3(blockBuffer[i].x, blockBuffer[i].y);

                // Updates Block Layer Position
                blockLayer[blockPos[i, j].y, blockPos[i, j].x] = i + 1;
            }
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

        // Goal Checks
        for (int i = 0; i < goalPos.Length; i++)
        {
            if ((blockLayer[goalPos[i].y, goalPos[i].x] != 0) || (playerPos.position.y == goalPos[i].x && playerPos.position.x == goalPos[i].y))
                goalStatus[i] = true;
            else
                goalStatus[i] = allGoals = false;
        }

        // Win Check
        if (allGoals)
        {
            transitioning = true;
        }
    }

    void clearData()
    {
        // Resetting Arrays
        Array.Clear(levelLayout, 0, levelLayout.Length);
        Array.Clear(blockLayer, 0, blockLayer.Length);
        Array.Clear(blockPos, 0, blockPos.Length);
        Array.Clear(blockBuffer, 0, blockBuffer.Length);
        Array.Clear(goalPos, 0, goalPos.Length);
        Array.Clear(goalStatus, 0, goalStatus.Length);

        // Reseting Numbers
        numBlocks = 0;

        // Don't need to reset
        /*scaleFactor = 0.0f;
        levelSize[0] = 0;
        levelSize[1] = 0;
        playerPos.position = new Vector2(0);*/
    }

    /*
    void drawLevel(Shader ourShader)
    {
        // Drawing Goals
        for (int i = 0; i < goalPos.Length; i++)
        {
            glm::mat4 model = glm::mat4(1.0f);
            model = glm::scale(model, glm::vec3(scaleFactor));
            model = glm::translate(model, glm::vec3((goalPos[i].x * 2) - (levelSize[0] - 1), (levelSize[1] - 1) - (goalPos[i].y * 2), 0));
            model = glm::scale(model, glm::vec3(0.5f));
            ourShader.setMat4("model", model);

            if (!goalStatus[i])
                ourShader.setVec4("blockColor", blockColor('F'));
            else
                ourShader.setVec4("blockColor", blockColor('G'));
            glDrawElements(GL_TRIANGLES, 6, GL_UNSIGNED_INT, 0);
        }

        // Drawing Player
        glm::mat4 model = glm::mat4(1.0f);
        model = glm::scale(model, glm::vec3(scaleFactor));
        model = glm::translate(model, glm::vec3((playerPos[0] * 2) - (levelSize[0] - 1), (levelSize[1] - 1) - (playerPos[1] * 2), 0));
        ourShader.setMat4("model", model);

        ourShader.setVec4("blockColor", blockColor('P'));
        glDrawElements(GL_TRIANGLES, 6, GL_UNSIGNED_INT, 0);

        // Drawing Blocks
        for (int i = 0; i < numBlocks; i++)
        {
            if (blockNums[i] > 0)
            {
                for (int j = 0; j < blockNums[i]; j++)
                {
                    glm::mat4 model = glm::mat4(1.0f);
                    model = glm::scale(model, glm::vec3(scaleFactor));
                    model = glm::translate(model, glm::vec3((blockPos[i, j].x * 2) - (levelSize[0] - 1), (levelSize[1] - 1) - (blockPos[i, j].y * 2), 0));
                    ourShader.setMat4("model", model);

                    ourShader.setVec4("blockColor", blockColor(i));
                    glDrawElements(GL_TRIANGLES, 6, GL_UNSIGNED_INT, 0);
                }
            }
        }

        // Drawing Puzzle
        for (int i = 0; i < levelSize[1]; i++)
        {
            for (int j = 0; j < levelSize[0]; j++)
            {
                glm::mat4 model = glm::mat4(1.0f);
                model = glm::scale(model, glm::vec3(scaleFactor));
                model = glm::translate(model, glm::vec3((j * 2) - (levelSize[0] - 1), (levelSize[1] - 1) - (i * 2), 0));
                ourShader.setMat4("model", model);

                ourShader.setVec4("blockColor", blockColor(levelLayout[i, j]));

                if (levelLayout[i, j] != '0')
                    glDrawElements(GL_TRIANGLES, 6, GL_UNSIGNED_INT, 0);
            }
        }

        // Drawing Background Piece
        model = glm::mat4(1.0f);
        model = glm::scale(model, glm::vec3(scaleFactor));
        model = glm::scale(model, glm::vec3(levelSize[0], levelSize[1], 1));

        ourShader.setMat4("model", model);
        ourShader.setVec4("blockColor", blockColor('0'));

        glDrawElements(GL_TRIANGLES, 6, GL_UNSIGNED_INT, 0);
    }*/
}