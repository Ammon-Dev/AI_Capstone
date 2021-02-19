using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Astar))]
public class Unit2 : MonoBehaviour
{

    // Combat
    public float hp;
    public bool redTeam;
    public Weapon weapon;
    public int grenadeCount;
    public int visionRange;
    public LayerMask layerMask;
    public BoxCollider2D col;
    
    //injured, in range, enemies remain
    public bool hurt;
    public bool range;
    public bool enemies;
    public bool alive;

    // Movement
    public int moveSpeed;
    public int curMoveRange { get; set; }
    public int maxMoveRange { get; set; }
    public Vector3Int currentPosition { get; set; }


    // State management
    public bool isActive;
    public int currentEnergy;
    public int maxEnergy { get; set; }
    public state unitState;

    public enum state
    {
        Waiting,
        Moving,
        Attacking,
        Idle
    };

    void Start()
    {
        // Get refernces
        col = GetComponent<BoxCollider2D>();

        // Set default values
        weapon = new Weapon(Weapon.weaponType.Rifle);
        hp = 1000;
        grenadeCount = 3;
        maxEnergy = 100;
        currentEnergy = 0;
        isActive = false;

        // Set current position and make sure tranform.pos is int
        currentPosition = new Vector3Int((int)transform.position.x, (int)transform.position.y, 0);
        transform.position = currentPosition;

        hurt = false;
        range = false;
        enemies = true;

    } // end Start


    public void DamagePlayer(float attackDamage)
    {
        hp -= attackDamage;

        if (hp <= 0)
        {
            Destroy(this.gameObject);
        }
    } // end damagePlayer

    public void TeleportPlayer(Vector3Int newPos, int energyCost)
    {
        transform.position = newPos;
        currentPosition = newPos;

        currentEnergy -= energyCost;
    } // end TeleportPlayer


} // end Unit Class
