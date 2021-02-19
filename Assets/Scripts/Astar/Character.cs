using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

class Character
{
    public Vector3Int pos { get; set; }//it's place
    public int movement { get; set; }//how far it can move
    public Character(Vector3Int pos, int movement)
    {
        this.pos = pos;
        this.movement = movement;
    }
}
