using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Astar))]
public class Unit : MonoBehaviour
{

    // Combat
    public float hp;
    public bool redTeam;
    public Weapon weapon;
    public int grenadeCount;
    public int visionRange;
    public LayerMask layerMask;
    public BoxCollider2D col;

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

    //injured, in range, enemies remain
    public bool hurt;
    public bool canHit;
    public bool enemies;
    public bool alive;

    //Cosmetic
    public SpriteRenderer spriteRend;

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
        spriteRend = GetComponent<SpriteRenderer>();

        // Set default values
        weapon = new Weapon(Weapon.weaponType.Rifle);
        hp = 250;
        grenadeCount = 3;
        maxEnergy = 100;
        currentEnergy = 0;
        isActive = false;

        hurt = false;
        canHit = false;
        enemies = true;
        alive = true;

        // Set current position and make sure tranform.pos is int
        currentPosition = new Vector3Int((int)transform.position.x, (int)transform.position.y, 0);
        transform.position = currentPosition;
        
    } // end Start

    // Sets the color of a unit depending on what team they are on and if they are avtive or not
    public void SetColor()
    {
        if (spriteRend) // Stops the function from happening if something happens to the sprite renderer
        {
            if (isActive)
            {
                if (redTeam)
                    spriteRend.color = Color.red;
                else
                    spriteRend.color = Color.blue;
            }
            else
            {
                if (redTeam)
                    spriteRend.color = new Color(0.5f, 0, 0);
                else
                    spriteRend.color = new Color(0.5f, 0, 150);
            }
        }
        
    }

    public void DamagePlayer(float attackDamage)
    {
        hp -= attackDamage;

        if(hp <= 0)
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
