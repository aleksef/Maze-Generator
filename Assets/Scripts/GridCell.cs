using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridCell : MonoBehaviour
{
    [SerializeField] Material currentMaterial;
    [SerializeField] Material visitedMaterial;
    public int gridX;
    public int gridZ;
    public bool isVisited = false;

    public void SetCurrent()
    {
        gameObject.GetComponent<Renderer>().material = currentMaterial;
        isVisited = true;
    }

    public void SetVisited()
    {
        gameObject.GetComponent<Renderer>().material = visitedMaterial;
        isVisited = true;
    }
}
