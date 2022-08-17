using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public List<GameObject> grid;
    [SerializeField] GameObject nodePrefab;
    [SerializeField] GameObject wallPrefab;
    [SerializeField] Slider speedSlider;
    [SerializeField] GameObject mainCamera;
    [SerializeField] GameObject ground;

    private void Start()
    {
        speedSlider.onValueChanged.AddListener(delegate { ChangeSpeed(speedSlider.value); });
    }

    public void CreateGrid() 
    {
        CreateGridCells(5, 5);
    }

    private void CreateGridCells(int lengthX, int lengthZ)
    {
        mainCamera.transform.position = new(lengthX - 1, mainCamera.transform.position.y, lengthZ + 1);
        ground.transform.position = new(lengthX - 1, 0, lengthZ + 1);

        for (int z = lengthZ; z > 0; z--)
        {
            for (int x = 0; x < lengthX; x++)
            {
                InstanciateGridCell(nodePrefab,
                        new(x * 2, 0, z * 2)
                        );
                if (x != 4) 
                {
                    InstanciateGridCell(wallPrefab,
                        new(x * 2 + 1, 0, z * 2)
                        );
                }
            }
            if (z > 1) 
            {
                // instantiate wall row
                for (int x = 0; x < lengthX * 2 - 1; x++)
                {
                    InstanciateGridCell(wallPrefab,
                        new(x, 0, z * 2 - 1)
                        );
                }
            }
        }
    }

    private void InstanciateGridCell(GameObject prefab, Vector3 position) 
    {
        GameObject newObj = Instantiate(prefab, position, prefab.transform.rotation);
        newObj.GetComponent<GridCell>().gridX = (int)position.x;
        newObj.GetComponent<GridCell>().gridZ = (int)position.z;
        grid.Add(newObj);
    }

    public void ChangeSpeed(float scale) 
    {
        Time.timeScale = scale;
    }
}
