using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField] GameObject nodePrefab;
    [SerializeField] GameObject wallPrefab;
    [SerializeField] Slider speedSlider;
    [SerializeField] GameObject mainCamera;

    private bool isGridCreated = false;

    private void Start()
    {
        speedSlider.onValueChanged.AddListener(delegate { ChangeSpeed(speedSlider.value); });
    }

    public void StartGeneration() 
    {
        if (!isGridCreated) 
        {
            CreateGrid(7, 8);
            GameObject[] nodes = GameObject.FindGameObjectsWithTag("Node");
            GameObject currentNode = nodes[Random.Range(0, nodes.Length)];
            StartCoroutine(DepthFirstSearch(currentNode, new List<GameObject>()));
        }
    }

    // Coroutines is just for animation purposes.
    IEnumerator DepthFirstSearch(GameObject currentNode, List<GameObject> visitedNodes) 
    {
        yield return new WaitForSeconds(0.1f);
        visitedNodes.Add(currentNode);
        currentNode.GetComponent<GridCell>().SetCurrent();
        List<GameObject> unvisitedNeighbours = CheckUnvisitedNeighbours(currentNode, 
            GameObject.FindGameObjectsWithTag("Node"));
        if (unvisitedNeighbours.Count != 0)
        {
            GameObject chosenNode = unvisitedNeighbours[Random.Range(0, unvisitedNeighbours.Count)];
            yield return new WaitForSeconds(0.1f);
            StartCoroutine(MarkWall(currentNode, chosenNode));
            currentNode.GetComponent<GridCell>().SetVisited();
            StartCoroutine(DepthFirstSearch(chosenNode, visitedNodes));
        }
        else
        {
            currentNode.GetComponent<GridCell>().SetVisited();
            visitedNodes.RemoveAt(visitedNodes.Count - 1);
            if (visitedNodes.Count == 0)
            {
                foreach (GridCell gridCell in GameObject.FindObjectsOfType<GridCell>())
                {
                    if (gridCell.isVisited)
                    {
                        Destroy(gridCell.gameObject);
                    }
                }
                Debug.Log("Maze generation finished.");
                yield break;
            }
            else
            {
                GameObject chosenNode = visitedNodes[visitedNodes.Count - 1];
                yield return new WaitForSeconds(0.1f);
                chosenNode.GetComponent<GridCell>().SetCurrent();
                visitedNodes.RemoveAt(visitedNodes.Count - 1);
                StartCoroutine(DepthFirstSearch(chosenNode, visitedNodes));
            }
        }
    }

    IEnumerator MarkWall(GameObject currentNode, GameObject chosenNode) 
    {
        int wallX = currentNode.GetComponent<GridCell>().gridX + (chosenNode.GetComponent<GridCell>().gridX - currentNode.GetComponent<GridCell>().gridX) / 2;
        int wallZ = currentNode.GetComponent<GridCell>().gridZ + (chosenNode.GetComponent<GridCell>().gridZ - currentNode.GetComponent<GridCell>().gridZ) / 2;
        GameObject[] walls = GameObject.FindGameObjectsWithTag("Wall");
        foreach (GameObject wall in walls)
        {
            if (wall.GetComponent<GridCell>().gridX == wallX &&
                wall.GetComponent<GridCell>().gridZ == wallZ)
            {
                wall.GetComponent<GridCell>().SetCurrent();
                yield return new WaitForSeconds(0.1f);
                wall.GetComponent<GridCell>().SetVisited();
            }
        }
    }

    private List<GameObject> CheckUnvisitedNeighbours(GameObject currentNode, GameObject[] nodes) 
    {
        GridCell currentNodeComponent = currentNode.GetComponent<GridCell>();
        List<GameObject> unvisitedNeighbours = new List<GameObject>();        

        foreach (GameObject node in nodes) 
        {
            GridCell nodeComponent = node.GetComponent<GridCell>();
            if (!nodeComponent.isVisited)
            {
                if (nodeComponent.gridZ == currentNodeComponent.gridZ + 2 &&
                    nodeComponent.gridX == currentNodeComponent.gridX) 
                { unvisitedNeighbours.Add(node); }
                else if (nodeComponent.gridZ == currentNodeComponent.gridZ &&
                    nodeComponent.gridX == currentNodeComponent.gridX + 2)
                { unvisitedNeighbours.Add(node); }
                else if (nodeComponent.gridZ == currentNodeComponent.gridZ - 2 &&
                    nodeComponent.gridX == currentNodeComponent.gridX)
                { unvisitedNeighbours.Add(node); }
                else if (nodeComponent.gridZ == currentNodeComponent.gridZ &&
                    nodeComponent.gridX == currentNodeComponent.gridX - 2)
                { unvisitedNeighbours.Add(node); }
            }
        }
        return unvisitedNeighbours;
    }

    private void CreateGrid(int lengthX, int lengthZ)
    {
        mainCamera.transform.position = new(lengthX, mainCamera.transform.position.y, lengthZ);
        for (int z = 0; z < lengthZ; z++)
        {
            for (int x = 0; x < lengthX; x++)
            {
                InstanciateGridCell(nodePrefab,new(x * 2, 0, z * 2));
                if (x != 4) 
                {
                    InstanciateGridCell(wallPrefab,new(x * 2 + 1, 0, z * 2));
                }
            }
            if (z < lengthZ - 1) 
            {
                // wall row on z axis
                for (int x = 0; x < lengthX * 2 - 1; x++)
                {
                    InstanciateGridCell(wallPrefab,new(x, 0, z * 2 + 1));
                }
            }
        }
        isGridCreated = true;
    }

    private void InstanciateGridCell(GameObject prefab, Vector3 position) 
    {
        GameObject newObj = Instantiate(prefab, position, prefab.transform.rotation);
        newObj.GetComponent<GridCell>().gridX = (int)position.x;
        newObj.GetComponent<GridCell>().gridZ = (int)position.z;
    }

    public void ChangeSpeed(float scale) 
    {
        Time.timeScale = scale;
    }
}
