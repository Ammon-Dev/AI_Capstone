using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid
{
    private int width;
    private int height;
    private float cellSize;
    private int[,] gridArray;
    public Grid(int width, int height, float cellSize) {
        this.width = width;
        this.height = height;
        this.cellSize = cellSize;
        gridArray = new int[width, height];

        for (int i = 0; i < gridArray.GetLength(0); i++) { 
            for (int j = 0; j < gridArray.GetLength(1); j++){

            }
        }
    }

    private Vector3 GetWorldPostion(int x, int y) { 
        return new Vector3(x,y) * cellSize;
    }
}
