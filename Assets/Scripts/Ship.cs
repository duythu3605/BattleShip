using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ship : MonoBehaviour
{

    List<GameObject> touchGrids = new List<GameObject>();
    public float xOffset = 0;
    public float zOffset = 0;

    private float nextZRotate = 90f;

    private GameObject clickedGrid;

    private int hitCount = 0;

    public int shipSize;

    private Material[] allMaterials;
    List<Color> allColors = new List<Color>();

    private void Start()
    {
        allMaterials = GetComponent<Renderer>().materials;
        for(int i =0; i <allMaterials.Length; i++)
        {
            allColors.Add(allMaterials[i].color);
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Grid"))
        {
            touchGrids.Add(collision.gameObject);
        }

    }
    public void ClearGridList()
    {
        touchGrids.Clear();
    }
    public Vector3 GetOffSetVec(Vector3 gridPos)
    {
        return new Vector3(gridPos.x + xOffset, 2, gridPos.z + zOffset);
    }
    public void RotateShip()
    {
        if (clickedGrid == null) return;
        touchGrids.Clear();
        transform.localEulerAngles += new Vector3(0, nextZRotate, 0);
        nextZRotate *= -1;
        float temp = xOffset;
        xOffset = zOffset;
        zOffset = temp;
        SetPosition(clickedGrid.transform.position);
    }

    public void SetPosition(Vector3 newVec)
    {
        ClearGridList();
        transform.localPosition = new Vector3(newVec.x + xOffset, 0, newVec.z + zOffset);
    }

    public void SetClickedGrid(GameObject grid)
    {
        clickedGrid = grid;
    }
    public bool OnGameBoard()
    {
        return touchGrids.Count == shipSize;
    }

    public bool HitCheckSank()
    {
        hitCount++;
        return shipSize <= hitCount;
    }

    public void FlashColor(Color tempColor)
    {
        foreach(Material mate in allMaterials)
        {
            mate.color = tempColor;
        }
        Invoke("ResetColor", 0.5f);
    }

    private void ResetColor()
    {
        int i = 0;
        foreach(Material mate in allMaterials)
        {
            mate.color = allColors[i++];
        }
    }
}
