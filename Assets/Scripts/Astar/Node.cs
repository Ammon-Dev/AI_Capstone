using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class Node
{
    public int G { get; set; }
    public int H { get; set; }
    public int F { get; set; }
    public Node Prev { get; set; }
    public Vector3Int pos { get; set; }

    public Node(Vector3Int pos) 
    {
        this.pos = pos;
    }
}
