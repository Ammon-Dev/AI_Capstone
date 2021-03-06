﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;


public class BTUnitManager : MonoBehaviour
{
    // Unit vars
    public List<Unit> RedTeam;
    public List<Unit> BlueTeam;
    public Unit activeUnit;

    private LayerMask layerMask; // Collision layer that units are on
    private Tilemap tilemap; // Tilemap Grid that units are on
    private Tilemap tilemapFloor; //tilemap with the floor

    // bools
    public bool RedTeamTurn { get; set; }
    public bool BlueTeamTurn { get; set; }
    public bool RedTurnNext { get; set; } // Check who's turn is next


    //lerp interpolation
    int interpolationFramesCount = 1000; // Number of frames to completely interpolate between the 2 positions
    int elapsedFrames = 0;

    //A*
    private Astar astar;
    private List<Node> rangeList;//movement range

    // Text assets
    private Text ActiveUnitText;
    private Text ActiveTeamText;
    private Text UnitStateText;
    private Text EnergyText;
    private Text HPText;

    //BT Vars
    public BTSelector rootAI;
    public BTSequence approachSequence;
    public BTSequence attackSequence;
    public BTSequence attackSafeSequence;
    public BTSequence coverSequence;

 

    // Setup references and default values
    void Start()
    {

        // Get ref to the parent team objects to fill out team lists
        Transform RedTeamT = GameObject.Find("RedTeam").GetComponent<Transform>();
        Transform BlueTeamT = GameObject.Find("BlueTeam").GetComponent<Transform>();

        // Add all units to each team
        foreach (Transform child in RedTeamT)
        {
            RedTeam.Add(child.gameObject.GetComponent<Unit>());
        }
        foreach (Transform child in BlueTeamT)
        {
            BlueTeam.Add(child.gameObject.GetComponent<Unit>());
        }

        // Setting references
        layerMask = LayerMask.GetMask("Units");
        tilemap = GameObject.Find("GridTileMap").GetComponent<Tilemap>();
        tilemapFloor = GameObject.Find("BackroundTileMap").GetComponent<Tilemap>();//tile floor
        astar = new Astar();// access to astar methods
        ActiveUnitText = GameObject.Find("ActiveUnitText").GetComponent<Text>();
        ActiveTeamText = GameObject.Find("ActiveTeamText").GetComponent<Text>();
        UnitStateText = GameObject.Find("UnitStateText").GetComponent<Text>();
        EnergyText = GameObject.Find("EnergyText").GetComponent<Text>();
        HPText = GameObject.Find("HPText").GetComponent<Text>();

        // Set red as first turn
        RedTeamTurn = true;

        approachSequence = new BTSequence(new List<BTNode>
        {
            new MoveToEnemyTask(this),
        });

        attackSequence = new BTSequence(new List<BTNode>
        {
            new CheckForEnemyTask(this),
            new CheckEnemyHPTask(this),
            new AttackTask(this),
        });

        attackSafeSequence = new BTSequence(new List<BTNode>
        {
            new CheckForEnemyTask(this),
            new AttackSafeTask(this),
        });
        coverSequence = new BTSequence(new List<BTNode>
        {
            new MoveToCoverTask(this),
        });

        rootAI = new BTSelector(new List<BTNode>
        {
            approachSequence,
            attackSequence,
            attackSafeSequence,
            coverSequence,
        });

    }

    // Update is called once per frame
    void Update()
    {

        if (RedTeamTurn)// set red team able to go
        {
            foreach (Unit unit in RedTeam)
            {
                if (unit.alive)
                {
                    unit.isActive = true;
                    unit.SetColor();
                }
                
            }
            ActiveTeamText.text = "Red Team";
            RedTeamTurn = false;
            activeUnit = null;
        }
        else if (BlueTeamTurn)// set red team able to go
        {
            foreach (Unit unit in BlueTeam)
            {
                if (unit.alive)
                {
                    unit.isActive = true;
                    unit.SetColor();
                }
            }
            ActiveTeamText.text = "Blue Team";
            BlueTeamTurn = false;
            activeUnit = null;
        }
        else if (activeUnit == null) // No unit yet chosen
        {
            // Set text for null player
            UnitStateText.text = "";
            EnergyText.text = "";
            HPText.text = "";
            ActiveUnitText.text = "No Active Unit";

            /*
            // Camera Lerping for null unit
            Vector3 newCamPos = new Vector3(0, 0, Camera.main.transform.position.z);
            float interpolationRatio = (float)elapsedFrames / interpolationFramesCount;
            Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, newCamPos, interpolationRatio);
            elapsedFrames = (elapsedFrames + 1) % (interpolationFramesCount + 1);  // reset elapsedFrames to zero after it reached (interpolationFramesCount + 1)
            */

            ChooseUnit();

        }
        else
        {
            UpdateUnit();
        }

    }

    private void ChooseUnit()
    {

        if (Input.GetMouseButtonDown(0))
        {

            // Cast a ray though screen
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, Mathf.Infinity, layerMask);

            // Check to hit a collider, and then make sure its a player
            if (hit.collider != null)
            {
                if (hit.transform.name.Contains("Player"))
                {
                    if (hit.collider.gameObject.GetComponent<Unit>().isActive)
                    {
                        activeUnit = hit.transform.gameObject.GetComponent<Unit>();
                        activeUnit.currentEnergy += activeUnit.maxEnergy;
                        activeUnit.unitState = Unit.state.Idle;
                        UnitStateText.text = "Idle"; // Set turn type for UI
                        ActiveUnitText.text = activeUnit.name; // Set name for UI
                        
                        //elapsedFrames = 0;
                        //Camera.main.orthographicSize = Mathf.Lerp(8, 5, 1); // Cammera Zoom out
                    }
                }
            }
        }
    }

    private void UpdateUnit()
    {



        if (activeUnit.isActive)
        {
            /*
            // Camera Lerping
            Vector3 newCamPos = new Vector3(activeUnit.transform.position.x, activeUnit.transform.position.y, Camera.main.transform.position.z);
            float interpolationRatio = (float)elapsedFrames / interpolationFramesCount;
            Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, newCamPos, interpolationRatio);
            elapsedFrames = (elapsedFrames + 1) % (interpolationFramesCount + 1);  // reset elapsedFrames to zero after it reached (interpolationFramesCount + 1)            
            */

            if (activeUnit.currentEnergy > 0)
            {
                EnergyText.text = "Energy: " + activeUnit.currentEnergy;  // Display current energy
                HPText.text = "HP: " + activeUnit.hp;  // Display current HP

                rootAI.Eval();

                /* user Swith
                switch (activeUnit.unitState)
                {
                    case Unit.state.Idle:
                        if (Input.GetKeyDown(KeyCode.Space)) // Space is to back out of current selection
                        {
                            activeUnit.unitState = Unit.state.Waiting;
                            activeUnit.currentEnergy -= activeUnit.maxEnergy;
                            activeUnit = null;
                            Camera.main.orthographicSize = Mathf.Lerp(5, 8, 1); // Cammera Zoom out
                        }
                        else if (Input.GetKeyDown(KeyCode.P)) // P is to Pass the rest of the turn
                        {
                            activeUnit.unitState = Unit.state.Waiting;
                            activeUnit.currentEnergy = 0;
                            Camera.main.orthographicSize = Mathf.Lerp(5, 8, 1); // Cammera Zoom out
                        }
                        else if (Input.GetKeyDown(KeyCode.A)) // A sets phase to attacking
                        {
                            activeUnit.unitState = Unit.state.Attacking;
                            UnitStateText.text = "Attacking"; // Set turn type for UI
                        }
                        else if (Input.GetKeyDown(KeyCode.M)) // M sets phase to movement
                        {
                            activeUnit.unitState = Unit.state.Moving;
                            UnitStateText.text = "Moving"; // Set turn type for UI
                        }
                        break; // End Idle
                    case Unit.state.Moving:
                        // Moving Logic
                        activeUnit.col.enabled = false;
                        if (Input.GetKeyDown(KeyCode.Space))
                        {
                            activeUnit.unitState = Unit.state.Idle;
                            UnitStateText.text = "Idle"; // Set turn type for UI
                        }
                        if (Input.GetMouseButtonDown(0))
                        {
                            // Get mouse pos
                            Vector3Int mousePos = GetMouseWorldPositionInt();

                            // Set default var values
                            int validMove = -1;


                            // AI functions
                            //moveUp();
                            //var cover = findCover();
                            //activeUnit.TeleportPlayer(cover.goal, cover.energy);

                            // Check if valid move
                            validMove = validatePos(mousePos);


                            if (validMove != -1 && validMove <= activeUnit.currentEnergy)
                            {
                                activeUnit.TeleportPlayer(mousePos, validMove); // Moves player if selection is valid

                                activeUnit.unitState = Unit.state.Idle; // Set unit back to idle after action
                                UnitStateText.text = "Idle"; // Set turn type for UI
                            }
                            //activeUnit.unitState = Unit.state.Idle; // Set unit back to idle after action
                            //UnitStateText.text = "Idle"; // Set turn type for UI

                        }
                        activeUnit.col.enabled = true;
                        break; // End Moving
                    case Unit.state.Attacking:
                        activeUnit.col.enabled = false; // Disable collider so raycast won't hit it
                        if (Input.GetKeyDown(KeyCode.Space))
                        {
                            activeUnit.unitState = Unit.state.Idle;
                            UnitStateText.text = "Idle"; // Set turn type for UI
                        }
                        else if (Input.GetMouseButtonDown(0))
                        {
                            Vector3 mousePos = GetMouseWorldPosition();
                            int energyCost = activeUnit.weapon.energyRequired;

                            // Raycast check if Attack is valid
                            Vector3 direction = (activeUnit.transform.position - mousePos).normalized;
                            RaycastHit2D hit = Physics2D.Raycast(activeUnit.transform.position, direction, -activeUnit.weapon.range, layerMask);
                            // Draw Raycast line
                            Debug.DrawLine(activeUnit.transform.position, activeUnit.transform.position + direction * -activeUnit.weapon.range, Color.red, 3);

                            //if we clicked on something
                            if (hit.collider != null)
                            {

                                if (hit.collider.name.Contains("Player"))
                                {
                                    if (hit.collider.gameObject.GetComponent<Unit>().redTeam != activeUnit.redTeam)
                                    {
                                        Unit enemy = hit.collider.gameObject.GetComponent<Unit>();

                                        enemy.DamagePlayer(activeUnit.weapon.damage);
                                        activeUnit.currentEnergy -= energyCost;

                                        activeUnit.unitState = Unit.state.Idle; // Set unit back to idle after action
                                        UnitStateText.text = "Idle"; // Set turn type for UI

                                    }
                                }
                            }
                        }
                        activeUnit.col.enabled = true; // Re-enable collider
                        break; // end Attacking
                    case Unit.state.Waiting:
                        // Currently processing unit choice
                        break; // end Waiting
                    default:
                        break;
                } // end Switch
                */

            } // end Energy
            else
            {
                // When the unit runs out if energy set it to waiting and inactive 
                activeUnit.isActive = false;
                activeUnit.SetColor();
                activeUnit.unitState = Unit.state.Waiting;
                elapsedFrames = 0;

                // Check if any active units left
                List<Unit> Team;
                bool teamActive = false;

                if (activeUnit.redTeam)
                    Team = RedTeam;
                else
                    Team = BlueTeam;
                // Check if the team has any more active members
                foreach (Unit unit in Team)
                {
                    if (unit.isActive)
                    {
                        teamActive = true;
                    }
                }
                // If the team has no more active members, start the next team's turn
                if (!teamActive)
                {
                    if (activeUnit.redTeam)
                        BlueTeamTurn = true;
                    else
                        RedTeamTurn = true;
                }


                // Active unit done with turn, so set ref to null again
                activeUnit = null;

                //Camera.main.orthographicSize = Mathf.Lerp(5, 8, 1); // Cammera Zoom out

            }
        } // end isActive
    } // end CharacterUpdate


    // Get Mouse Position in World with Z = 0f
    public Vector3Int GetMouseWorldPositionInt()
    {
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);//pos where we clicked
        Vector3Int mouseWorldPosRounded = new Vector3Int((int)Mathf.Round(mouseWorldPos.x), (int)Mathf.Round(mouseWorldPos.y), (int)Mathf.Round(mouseWorldPos.z));
        Vector3Int clickPos = tilemap.WorldToCell(mouseWorldPosRounded);//tilespace where we clicked
        return clickPos;
    } // end GetMouseWorldPositionInt

    // Get Mouse Position in World with Z = 0f
    public Vector3 GetMouseWorldPosition()
    {
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);//pos where we clicked
        return mouseWorldPos;
    } // end GetMouseWorldPosition


    //We're gonna rock on down to AI avenue
    //checks if the position you clicked on is a valid tile
    public int validatePos(Vector3Int mousePos)
    {
        //the closed list that holds the valid positions
        int energy = activeUnit.currentEnergy;
        rangeList = astar.CharAlgorithm(activeUnit.currentPosition, mousePos, tilemapFloor, energy);//movement list
        for (int a = 0; a < 2; a++)
        {
            if (RedTeam[a].currentPosition == mousePos || BlueTeam[a].currentPosition == mousePos)
            {
                return -1;
            }
        }

        //if in the list it's valid
        for (int i = 0; i < rangeList.Count; i++)
        {
            if (rangeList[i].pos == mousePos)
            {
                return rangeList[i].G;
            }
        }

        return -1;
    }//end validatePos

    public bool moveUp()//function for moving the unit up and attacking
    {
        bool returnVal = false;
        float pDistance = 9999;//shortest distance to an enemy unit
        Vector3Int goal = new Vector3Int();//current target
        if (activeUnit.redTeam)//if on red
        {
            foreach (Unit blue in BlueTeam)//for each blue
            {
                int x = blue.currentPosition.x - activeUnit.currentPosition.x;
                int y = blue.currentPosition.y - activeUnit.currentPosition.y;
                float distance = Mathf.Sqrt(Mathf.Abs(x * x + y * y));//distance to enemy unit
                if (distance < pDistance)//if the shortest distance
                {
                    pDistance = distance;//the short distance
                    goal = blue.currentPosition;//new goal
                }
            }
            foreach (Unit red in RedTeam)//disables the team collision
            {
                red.col.enabled = false;
            }
            if (CheckAttack(activeUnit.currentPosition, goal))//check if we can hit
            {
                returnVal = false;
            }
            else
            {
                returnVal = attackPos(goal);//gets the move up position
            }
            foreach (Unit red in RedTeam)
            {
                red.col.enabled = true;
            }
            return returnVal;
        }

        else//if on blue
        {
            foreach (Unit red in RedTeam)//for each red
            {
                int x = red.currentPosition.x - activeUnit.currentPosition.x;
                int y = red.currentPosition.y - activeUnit.currentPosition.y;
                float distance = Mathf.Sqrt(Mathf.Abs(x * x + y * y));//distance to the enemy
                if (distance < pDistance)//if shortest distance
                {
                    pDistance = distance;//the short distance
                    goal = red.currentPosition;//new goal
                }
            }
            foreach (Unit blue in BlueTeam)
            {
                blue.col.enabled = false;
            }
            //activeUnit.col.enabled = false;
            if (CheckAttack(activeUnit.currentPosition, goal))//check if we can attack
            {
                returnVal = false;
            }
            else//move up
            {
                returnVal = attackPos(goal);//gets the move up position
            }
            foreach (Unit blue in BlueTeam)
            {
                blue.col.enabled = true;
            }
            return returnVal;
        }
    }

    public bool attackPos(Vector3Int goal)//move til you can hit the enemy
    {
        //path
        Stack<Vector3Int> path = astar.Algorithm(activeUnit.currentPosition, goal, tilemapFloor, activeUnit.currentEnergy);//quickest path to the enemy
        Vector3Int current = activeUnit.currentPosition;//the current step in the path
        int sum = 0;//energy used

        foreach (Vector3Int pos in path)
        {
            int x = current.x - pos.x;//used to check movement type
            int y = current.y - pos.y;
            current = pos;//move the step up

            if (Mathf.Abs(x - y) % 2 == 1)//move side to side/up down
            {
                sum += 10;
            }
            else//diagonal movement
            {
                sum += 14;
            }
            if (activeUnit.currentEnergy - sum >= 0)
            {//if we can still move 
                if (CheckAttack(pos, goal))//if we hit
                {
                    activeUnit.TeleportPlayer(pos, sum);//move the unit
                    activeUnit.canHit = true;
                    return true;
                }
            }
            else
            {
                activeUnit.TeleportPlayer(pos, sum);//moves as far as we can
                return false;
            }
        }
        return false;
    }

    public bool CheckAttack(Vector3 pos, Vector3 goal) //check if an enemy can be attacked
    {
        //Debug.Log(activeUnit.col.enabled);
        Vector3 direction = pos - goal;//the vector direction
        direction = direction.normalized;//unit vector
        RaycastHit2D hit = Physics2D.Raycast(pos, direction, -activeUnit.weapon.range, layerMask);//ray cast to the enemy
        Debug.DrawLine(pos, pos + direction * -activeUnit.weapon.range, Color.blue, 33);//ray drawn
        //Debug.Log(hit.collider);
        //Debug.Log(goal);
        //Debug.Log(activeUnit.transform.position);
        if (hit.collider != GameObject.Find("GridTileMap").GetComponent<TilemapCollider2D>() || hit.collider == null)//if hit the enemy
        {
            //Debug.Log(hit.collider);
            return true;//say hit
        }
        //Debug.Log(hit.collider);
        return false;
    }

    public void AttackAction()
    {
        float pDistance = 999;
        Vector3Int goal = new Vector3Int();
        if (activeUnit.redTeam)
        {
            foreach (Unit blue in BlueTeam)//for each blue
            {
                int x = blue.currentPosition.x - activeUnit.currentPosition.x;
                int y = blue.currentPosition.y - activeUnit.currentPosition.y;
                float distance = Mathf.Sqrt(Mathf.Abs(x * x + y * y));//distance to enemy unit
                if (distance < pDistance)//if the shortest distance
                {
                    pDistance = distance;//the short distance
                    goal = blue.currentPosition;//new goal
                }
            }
        }
        else
        {
            foreach (Unit red in RedTeam)//for each blue
            {
                int x = red.currentPosition.x - activeUnit.currentPosition.x;
                int y = red.currentPosition.y - activeUnit.currentPosition.y;
                float distance = Mathf.Sqrt(Mathf.Abs(x * x + y * y));//distance to enemy unit
                if (distance < pDistance)//if the shortest distance
                {
                    pDistance = distance;//the short distance
                    goal = red.currentPosition;//new goal
                }
            }
        }
        Vector3 pos = activeUnit.currentPosition;
        Vector3 direction = pos - goal;//the vector direction
        direction = direction.normalized;//unit vector
        activeUnit.col.enabled = false;
        RaycastHit2D hit = Physics2D.Raycast(pos, direction, -activeUnit.weapon.range, layerMask);//ray cast to the enemy
        activeUnit.col.enabled = true;
        Debug.DrawLine(pos, pos + direction * -activeUnit.weapon.range, Color.blue, 33);//ray drawn
        if (hit.collider != GameObject.Find("GridTileMap").GetComponent<TilemapCollider2D>() || hit.collider == null)//if hit the enemy
        {
            Unit enemy = hit.collider.gameObject.GetComponent<Unit>();

            enemy.DamagePlayer(activeUnit.weapon.damage);
            activeUnit.currentEnergy -= activeUnit.weapon.energyRequired;
        }
    }

    public (Vector3Int goal, int energy) findCover()//gives a position the unit can find cover
    {
        List<Node> range = astar.CharAlgorithm(activeUnit.currentPosition, activeUnit.currentPosition, tilemapFloor, activeUnit.currentEnergy);//list of range of movement
        foreach (Node node in range)//in the list
        {
            if (!CheckDanger(node.pos))//check if we can be hit
            {
                //Debug.Log("sup");
                Stack<Vector3Int> path = astar.Algorithm(activeUnit.currentPosition, node.pos, tilemapFloor, 0);
                Vector3Int current = activeUnit.currentPosition;
                int sum = 0;
                foreach (Vector3Int pos in path)
                {
                    int x = current.x - pos.x;//used to check movement type
                    int y = current.y - pos.y;
                    current = pos;//move the step up

                    if (Mathf.Abs(x - y) % 2 == 1)//move side to side/up down
                    {
                        sum += 10;
                    }
                    else//diagonal movement
                    {
                        sum += 14;
                    }
                }

                return (node.pos, sum);//if not then move there
            }
        }
        return (activeUnit.currentPosition, 0);
    }

    public bool CheckDanger(Vector3Int active)//checks if they can be hit
    {
        if (activeUnit.redTeam)
        {
            activeUnit.col.enabled = false;
            foreach (Unit blue in BlueTeam)
            {
                blue.col.enabled = true;
                if (CheckAttack(active, blue.currentPosition))
                {
                    Debug.Log(true);

                    activeUnit.col.enabled = true;
                    return true;
                }
                Debug.Log(false);
            }
            activeUnit.col.enabled = true;
            return false;
        }
        else
        {
            activeUnit.col.enabled = false;
            foreach (Unit red in RedTeam)
            {
                red.col.enabled = true;
                if (CheckAttack(active, red.currentPosition))
                {
                    Debug.Log(true);

                    activeUnit.col.enabled = true;
                    return true;
                }
                Debug.Log(false);
            }
            activeUnit.col.enabled = true;
            return false;
        }
    }

    public float CheckEnemyHP()//checks if they can be hit
    {
        if (activeUnit.redTeam)
        {
            activeUnit.col.enabled = false;
            foreach (Unit blue in BlueTeam)
            {
                blue.col.enabled = true;
                if (CheckAttack(activeUnit.currentPosition, blue.currentPosition))
                {
                    Debug.Log(true);

                    activeUnit.col.enabled = true;
                    return blue.hp;
                }
                Debug.Log(false);
            }
            activeUnit.col.enabled = true;
            return -1;
        }
        else
        {
            activeUnit.col.enabled = false;
            foreach (Unit red in RedTeam)
            {
                red.col.enabled = true;
                if (CheckAttack(activeUnit.currentPosition, red.currentPosition))
                {
                    Debug.Log(true);

                    activeUnit.col.enabled = true;
                    return red.hp;
                }
                Debug.Log(false);
            }
            activeUnit.col.enabled = true;
            return -1;
        }
    }


    public bool CheckOutnumbered()//check if the unit is outnumbered
    {
        int enemy = 0;//enemy counter
        int friend = 1;//friend counter including self
        if (activeUnit.redTeam)
        {
            foreach (Unit blue in BlueTeam)
            {
                if (CheckAttack(blue.currentPosition, activeUnit.currentPosition))
                {
                    enemy++;
                }
            }
            foreach (Unit red in RedTeam)
            {
                if (CheckAttack(red.currentPosition, activeUnit.currentPosition) && red.currentPosition != activeUnit.currentPosition)
                {
                    friend++;
                }
            }
            if (enemy > friend)//check if the unit is outnumbered
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            foreach (Unit red in RedTeam)
            {
                if (CheckAttack(red.currentPosition, activeUnit.currentPosition))
                {
                    enemy++;
                }
            }
            foreach (Unit blue in BlueTeam)
            {
                if (CheckAttack(blue.currentPosition, activeUnit.currentPosition) && blue.currentPosition != activeUnit.currentPosition)
                {
                    friend++;
                }
            }
            if (enemy > friend)//check if the unit is outnumbered
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
} // end UnitManager Class
