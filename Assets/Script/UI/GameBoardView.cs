using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBoardView : MonoBehaviour
{
    [Header("Prefab References")]
    public GameObject dotPrefab;
    public GameObject linePrefab;
    public GameObject boxPrefab;

    [Header("Board Settings")]
    public float gridSpacing = 1.0f;
    public float lineWidth = 0.1f;
    public float dotSize = 0.15f;

    [Header("Colors")]
    public Color player1Color = Color.blue;
    public Color player2Color = Color.red;
    public Color defaultLineColor = Color.gray;
    public Color completedBoxColor = new Color(1f, 1f, 1f, 0.3f);

    private GameBoard gameBoard;
    private GameObject[,] dotObjects;
    private GameObject[,] horizontalLineObjects;
    private GameObject[,] verticalLineObjects;
    private GameObject[,] boxObjects;

    private Vector3 centerOffset; 

    public void InitializeBoardView(GameBoard board)
    {
        gameBoard = board;
        CreateBoardVisuals();
    }

    void CreateBoardVisuals()
    {
        int gridSize = gameBoard.gridSize;

        ClearBoardVisuals();

        float offsetX = ((gridSize - 1) * gridSpacing) / 2f;
        float offsetY = ((gridSize - 1) * gridSpacing) / 2f;
        centerOffset = new Vector3(offsetX, offsetY, 0);

        CreateDots(gridSize, centerOffset);

        CreateLines(gridSize, centerOffset);

        CreateBoxes(gridSize, centerOffset);
    }

    void CreateDots(int gridSize, Vector3 centerOffset)
    {
        dotObjects = new GameObject[gridSize, gridSize];

        for (int row = 0; row < gridSize; row++)
        {
            for (int col = 0; col < gridSize; col++)
            {
                Vector3 position = new Vector3(
                    col * gridSpacing,
                    (gridSize - 1 - row) * gridSpacing,
                    0
                ) - centerOffset;

                GameObject dot;
                if (dotPrefab != null)
                {
                    dot = Instantiate(dotPrefab, position, Quaternion.identity, transform);
                }
                else
                {
                    dot = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    dot.transform.position = position;
                    dot.transform.localScale = Vector3.one * dotSize;
                    dot.transform.parent = transform;
                    dot.GetComponent<Renderer>().material.color = Color.black;
                }

                dot.name = $"Dot_{row}_{col}";
                dotObjects[row, col] = dot;
            }
        }
    }

    void CreateLines(int gridSize, Vector3 centerOffset)
    {
        horizontalLineObjects = new GameObject[gridSize, gridSize - 1];
        for (int row = 0; row < gridSize; row++)
        {
            for (int col = 0; col < gridSize - 1; col++)
            {
                Vector3 startPos = new Vector3(col * gridSpacing, (gridSize - 1 - row) * gridSpacing, 0) - centerOffset;
                Vector3 endPos = new Vector3((col + 1) * gridSpacing, (gridSize - 1 - row) * gridSpacing, 0) - centerOffset;

                GameObject line = CreateLineObject(startPos, endPos, true);
                line.name = $"HLine_{row}_{col}";
                line.SetActive(false); 
                horizontalLineObjects[row, col] = line;
            }
        }

        verticalLineObjects = new GameObject[gridSize - 1, gridSize];
        for (int row = 0; row < gridSize - 1; row++)
        {
            for (int col = 0; col < gridSize; col++)
            {
                Vector3 startPos = new Vector3(col * gridSpacing, (gridSize - 1 - row) * gridSpacing, 0) - centerOffset;
                Vector3 endPos = new Vector3(col * gridSpacing, (gridSize - 2 - row) * gridSpacing, 0) - centerOffset;

                GameObject line = CreateLineObject(startPos, endPos, false);
                line.name = $"VLine_{row}_{col}";
                line.SetActive(false); 
                verticalLineObjects[row, col] = line;
            }
        }
    }

    GameObject CreateLineObject(Vector3 start, Vector3 end, bool isHorizontal)
    {
        if (linePrefab != null)
        {
            GameObject line = Instantiate(linePrefab, transform);
            Vector3 center = (start + end) / 2;
            line.transform.position = center;

            if (isHorizontal)
                line.transform.localScale = new Vector3(gridSpacing, lineWidth, 1);
            else
                line.transform.localScale = new Vector3(lineWidth, gridSpacing, 1);

            return line;
        }
        else
        {
            GameObject lineObj = new GameObject("Line");
            lineObj.transform.parent = transform;

            LineRenderer lr = lineObj.AddComponent<LineRenderer>();
            lr.material = new Material(Shader.Find("Sprites/Default"));
            lr.startColor = defaultLineColor;
            lr.endColor = defaultLineColor;
            lr.startWidth = lineWidth;
            lr.endWidth = lineWidth;
            lr.sortingOrder = 1;
            lr.useWorldSpace = false;

            lr.positionCount = 2;
            lr.SetPosition(0, start);
            lr.SetPosition(1, end);

            return lineObj;
        }
    }

    void CreateBoxes(int gridSize, Vector3 centerOffset)
    {
        boxObjects = new GameObject[gridSize - 1, gridSize - 1];

        for (int row = 0; row < gridSize - 1; row++)
        {
            for (int col = 0; col < gridSize - 1; col++)
            {
                Vector3 center = new Vector3(
                    (col + 0.5f) * gridSpacing,
                    (gridSize - 1.5f - row) * gridSpacing,
                    0.1f
                ) - centerOffset; 

                GameObject box;
                if (boxPrefab != null)
                {
                    box = Instantiate(boxPrefab, center, Quaternion.identity, transform);
                }
                else
                {
                    box = GameObject.CreatePrimitive(PrimitiveType.Quad);
                    box.transform.position = center;
                    box.transform.localScale = new Vector3(gridSpacing * 0.8f, gridSpacing * 0.8f, 1);
                    box.transform.parent = transform;

                    Renderer renderer = box.GetComponent<Renderer>();
                    renderer.material = new Material(Shader.Find("Sprites/Default"));
                    renderer.material.color = completedBoxColor;
                }

                box.name = $"Box_{row}_{col}";
                box.SetActive(false); 
                boxObjects[row, col] = box;
            }
        }
    }

    public void UpdateLineView(int row, int col, bool isHorizontal, Player player)
    {
        GameObject lineObj;

        if (isHorizontal)
        {
            lineObj = horizontalLineObjects[row, col];
        }
        else
        {
            lineObj = verticalLineObjects[row, col];
        }

        if (lineObj != null)
        {
            lineObj.SetActive(true);

            Color lineColor = player.playerId == 1 ? player1Color : player2Color;

            Renderer renderer = lineObj.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material.color = lineColor;
            }

            LineRenderer lr = lineObj.GetComponent<LineRenderer>();
            if (lr != null)
            {
                lr.startColor = lineColor;
                lr.endColor = lineColor;
            }
        }
    }

    public void UpdateBoxView(int row, int col, Player player)
    {
        if (boxObjects != null && row < boxObjects.GetLength(0) && col < boxObjects.GetLength(1))
        {
            GameObject box = boxObjects[row, col];
            if (box != null)
            {
                box.SetActive(true);

                Color boxColor = player.playerId == 1 ?
                    new Color(player1Color.r, player1Color.g, player1Color.b, 0.3f) :
                    new Color(player2Color.r, player2Color.g, player2Color.b, 0.3f);

                Renderer renderer = box.GetComponent<Renderer>();
                if (renderer != null)
                {
                    renderer.material.color = boxColor;
                }

                AddPlayerLabel(box, player);
            }
        }
    }

    void AddPlayerLabel(GameObject box, Player player)
    {
        GameObject textObj = new GameObject("PlayerLabel");
        textObj.transform.parent = box.transform;
        textObj.transform.localPosition = Vector3.zero;

        TextMesh textMesh = textObj.AddComponent<TextMesh>();
        textMesh.text = player.playerId.ToString();
        textMesh.fontSize = 20;
        textMesh.color = player.playerColor;
        textMesh.anchor = TextAnchor.MiddleCenter;
        textMesh.characterSize = 0.1f;

        MeshRenderer meshRenderer = textObj.GetComponent<MeshRenderer>();
        meshRenderer.sortingOrder = 2;
    }

    void ClearBoardVisuals()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(transform.GetChild(i).gameObject);
        }
    }

    public bool GetLineFromWorldPosition(Vector3 worldPos, out int row, out int col, out bool isHorizontal)
    {
        row = col = 0;
        isHorizontal = false;

        Vector3 localPos = worldPos + centerOffset;

        float gridX = localPos.x / gridSpacing;
        float gridY = (gameBoard.gridSize - 1) - (localPos.y / gridSpacing);

        int hRow = Mathf.RoundToInt(gridY);
        int hCol = Mathf.FloorToInt(gridX + 0.5f);

        if (hRow >= 0 && hRow < gameBoard.gridSize &&
            hCol >= 0 && hCol < gameBoard.gridSize - 1)
        {
            float distToHLine = Mathf.Abs(gridY - hRow);
            if (distToHLine < 0.2f && Mathf.Abs(gridX - (hCol + 0.5f)) < 0.4f)
            {
                row = hRow;
                col = hCol;
                isHorizontal = true;
                return true;
            }
        }

        int vRow = Mathf.FloorToInt(gridY + 0.5f);
        int vCol = Mathf.RoundToInt(gridX);

        if (vRow >= 0 && vRow < gameBoard.gridSize - 1 &&
            vCol >= 0 && vCol < gameBoard.gridSize)
        {
            float distToVLine = Mathf.Abs(gridX - vCol);
            if (distToVLine < 0.2f && Mathf.Abs(gridY - (vRow + 0.5f)) < 0.4f)
            {
                row = vRow;
                col = vCol;
                isHorizontal = false;
                return true;
            }
        }

        return false;
    }
}