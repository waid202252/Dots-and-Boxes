using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBoard : MonoBehaviour
{
    [Header("Board Settings")]
    public int gridSize = 6;
    
    [Header("Runtime Data")]
    public Line[,] horizontalLines;
    public Line[,] verticalLines;
    public Box[,] boxes;
    
    public void InitializeBoard()
    {
        horizontalLines = new Line[gridSize, gridSize - 1];
        for (int r = 0; r < gridSize; r++)
        {
            for (int c = 0; c < gridSize - 1; c++)
            {
                horizontalLines[r, c] = new Line(r, c, true);
            }
        }

        verticalLines = new Line[gridSize - 1, gridSize];
        for (int r = 0; r < gridSize - 1; r++)
        {
            for (int c = 0; c < gridSize; c++)
            {
                verticalLines[r, c] = new Line(r, c, false);
            }
        }

        boxes = new Box[gridSize - 1, gridSize - 1];
        for (int r = 0; r < gridSize - 1; r++)
        {
            for (int c = 0; c < gridSize - 1; c++)
            {
                boxes[r, c] = new Box(r, c);
            }
        }
    }

    public bool CanPlaceLine(int row, int col, bool isHorizontal)
    {
        if (isHorizontal)
        {
            if (row < 0 || row >= gridSize || col < 0 || col >= gridSize - 1)
                return false;
            return !horizontalLines[row, col].isPlaced;
        }
        else
        {
            if (row < 0 || row >= gridSize - 1 || col < 0 || col >= gridSize)
                return false;
            return !verticalLines[row, col].isPlaced;
        }
    }

    public List<Box> PlaceLine(int row, int col, bool isHorizontal, int playerId)
    {
        List<Box> completedBoxes = new List<Box>();

        if (!CanPlaceLine(row, col, isHorizontal))
            return completedBoxes;

        if (isHorizontal)
        {
            Line line = horizontalLines[row, col];
            line.isPlaced = true;
            line.playerId = playerId;
            horizontalLines[row, col] = line;
        }
        else
        {
            Line line = verticalLines[row, col];
            line.isPlaced = true;
            line.playerId = playerId;
            verticalLines[row, col] = line;
        }

        completedBoxes = CheckCompletedBoxes(row, col, isHorizontal, playerId);

        return completedBoxes;
    }

    private List<Box> CheckCompletedBoxes(int row, int col, bool isHorizontal, int playerId)
    {
        List<Box> completedBoxes = new List<Box>();

        if (isHorizontal)
        {
            if (row > 0 && IsBoxCompleted(row - 1, col))
            {
                Box box = boxes[row - 1, col];
                if (!box.isCompleted)
                {
                    box.isCompleted = true;
                    box.ownerId = playerId;
                    boxes[row - 1, col] = box;
                    completedBoxes.Add(box);
                }
            }

            if (row < gridSize - 1 && IsBoxCompleted(row, col))
            {
                Box box = boxes[row, col];
                if (!box.isCompleted)
                {
                    box.isCompleted = true;
                    box.ownerId = playerId;
                    boxes[row, col] = box;
                    completedBoxes.Add(box);
                }
            }
        }
        else 
        {
        
            if (col > 0 && IsBoxCompleted(row, col - 1))
            {
                Box box = boxes[row, col - 1];
                if (!box.isCompleted)
                {
                    box.isCompleted = true;
                    box.ownerId = playerId;
                    boxes[row, col - 1] = box;
                    completedBoxes.Add(box);
                }
            }

            if (col < gridSize - 1 && IsBoxCompleted(row, col))
            {
                Box box = boxes[row, col];
                if (!box.isCompleted)
                {
                    box.isCompleted = true;
                    box.ownerId = playerId;
                    boxes[row, col] = box;
                    completedBoxes.Add(box);
                }
            }
        }

        return completedBoxes;
    }

    private bool IsBoxCompleted(int boxRow, int boxCol)
    {
        if (boxRow < 0 || boxRow >= gridSize - 1 || boxCol < 0 || boxCol >= gridSize - 1)
            return false;

        bool topLine = horizontalLines[boxRow, boxCol].isPlaced;
        bool bottomLine = horizontalLines[boxRow + 1, boxCol].isPlaced;
        bool leftLine = verticalLines[boxRow, boxCol].isPlaced;
        bool rightLine = verticalLines[boxRow, boxCol + 1].isPlaced;

        return topLine && bottomLine && leftLine && rightLine;
    }

    public bool IsGameOver()
    {
        for (int r = 0; r < gridSize; r++)
        {
            for (int c = 0; c < gridSize - 1; c++)
            {
                if (!horizontalLines[r, c].isPlaced)
                    return false;
            }
        }

        for (int r = 0; r < gridSize - 1; r++)
        {
            for (int c = 0; c < gridSize; c++)
            {
                if (!verticalLines[r, c].isPlaced)
                    return false;
            }
        }

        return true;
    }

    public List<(int row, int col, bool isHorizontal)> GetAvailableMoves()
    {
        List<(int, int, bool)> moves = new List<(int, int, bool)>();

        for (int r = 0; r < gridSize; r++)
        {
            for (int c = 0; c < gridSize - 1; c++)
            {
                if (!horizontalLines[r, c].isPlaced)
                    moves.Add((r, c, true));
            }
        }

        for (int r = 0; r < gridSize - 1; r++)
        {
            for (int c = 0; c < gridSize; c++)
            {
                if (!verticalLines[r, c].isPlaced)
                    moves.Add((r, c, false));
            }
        }

        return moves;
    }
}