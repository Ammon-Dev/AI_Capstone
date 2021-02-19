using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public enum TileType {START, GOAL, WALL, GRASS, PATH, RIFLE}//the tiletypes we have
public class Astar : MonoBehaviour
{
    public TileType tiletype;//the current tiletype for the game


    //serialized block
    [SerializeField]
    public Tilemap tilemap;//the game tilemap

    [SerializeField]
    private Tile[] tiles;//the tiles that go with the tile type

    [SerializeField]
    private Camera camera;//get the main camera

    [SerializeField]
    private LayerMask layerMask;//gets the layer the tilemap is on


    private Vector3Int startPos, goalPos;//starting and ending pos

    

    //A* Node Block
    private Node current;//the node we are on
    private HashSet<Node> openList;//the nodes that are currently at our edge
    private HashSet<Node> closedList;//the nodes that have been visited as current
    private Stack<Vector3Int> path;//the path from start to finish

    private Dictionary<Vector3Int, Node> allNodes = new Dictionary<Vector3Int, Node>();//the nodes we have so far

    private Node currentChar;//the node we are on
    private List<Node> openListChar;//the nodes that are currently at our edge
    private HashSet<Node> closedListChar;//the nodes that have been visited as current
    private Character rifle;//our actors



    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0)) {//if leftmouse down
            RaycastHit2D hit = Physics2D.Raycast(camera.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, Mathf.Infinity, layerMask);//uses a raycast to see if we clicked on a tile or button on the tilemap layer
            if (hit.collider != null)//if we clicked on something
            {
                Vector3 mouseWorldPos = camera.ScreenToWorldPoint(Input.mousePosition);//pos where we clicked
                Vector3Int clickPos = tilemap.WorldToCell(mouseWorldPos);//tilespace where we clicked

                ChangeTile(clickPos);//change the tile at the mouse click
            }
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            //run algorithm
            Algorithm();
            //CharAlgorithm();
        }
    }
    
    private void CharInit()
    {
        
        rifle = new Character(startPos, 5);//makes the rifleman, to be taken out into array
        
        currentChar = new Node(rifle.pos);//makes a node for the first node and puts it in the current


        openListChar = new List<Node>();//makes a new hashset for the open list
        closedListChar = new HashSet<Node>();//makes a new hashset for the closed list

        openListChar.Add(currentChar);//adds the current(start) node to the hashlist
    }

    private void MoveCharTile(Vector3Int startPos, Vector3Int goalPos)//to be worked on
    {
        tilemap.SetTile(startPos, tiles[3]);//lays down the tile type
        tilemap.SetTile(goalPos, tiles[5]);//lays down the tile type
    }

    private void CharAlgorithm()//the actual algorithm
    {
        if (currentChar == null)
        {
            CharInit();//makes the first node
        }
        for (int x=0;x<=rifle.movement;x++)//for as long as the unit can move
        {
            
            List<Node> open = new List<Node>();
            
            for (int n=0; n<openListChar.Count; n++)//for the nodes in the open list
            {
                
                Node node = openListChar[n];
                List<Node> neighbors = FindNeighbors(node.pos);//makes a list of the neighbors
                
                List<Node> temp = CharExamineNeighbors(neighbors, node);//adds new neighbors to the open list and puts old nodes in the closed list
                for (int i=0; i<temp.Count; i++)
                {
                    open.Add(temp[i]);
                }
            }
            for (int j = 0; j < open.Count; j++)
            {
                openListChar.Add(open[j]);
            }
            Debug.Log(openListChar.Count);
        }
        debug.MyInstance.CreateRangeTiles(openListChar, closedListChar);//makes the range tiles
    }
    private List<Node> CharExamineNeighbors(List<Node> neighbors, Node currentNode)//adds neighbors to the open list and adds current to the closed
    {
        List<Node> temp = new List<Node>();
        for (int i = 0; i < neighbors.Count; i++)//for each neighbor
        {
            Node neighbor = neighbors[i];//put the node in a node for cleaner code

            if (!ConnectedDiag(currentNode, neighbor))//skip if there's obstructions diagonally
            {
                continue;
            }

            int gScore = determineGScore(neighbors[i].pos, currentNode.pos);//gets the g score

            if (openListChar.Contains(neighbor))//if neighbor is in the openlist
            {
                if (currentNode.G + gScore < neighbor.G)//if this path costs less than path to get to the neighbor originally
                {
                    CalcValues(currentNode, neighbor, gScore);//gives the neighbor node it's prev and cost
                }
            }
            else if (!closedListChar.Contains(neighbor) && currentNode.G+gScore<=rifle.movement*10)//if neighbor isn't in the closed list and we can still move
            {
                CalcValues(currentNode, neighbor, gScore);//gives the neighbor node it's prev and cost
                temp.Add(neighbor);//adds the neighbor to the temp list
            }
        }
        UpdateCharNode(ref currentNode);//puts the currentNode onto the closed list
        return temp;
    }

    private void UpdateCharNode(ref Node currentNode) //puts current off the open list and into the closed
    {
        openListChar.Remove(currentNode);
        closedListChar.Add(currentNode);
    }

    










    //handles nodes
    private void Initialize()//makes the first node
    {
         
        rifle = new Character(startPos, 5);//makes the rifleman, to be taken out into array
        current = GetNode(rifle.pos);//makes a nodes for the first node and puts it in the current


        openList = new HashSet<Node>();//makes a new hashset for the open list
        closedList = new HashSet<Node>();//makes a new hashset for the closed list
        
        openList.Add(current);//adds the current(start) node to the hashlist
    }
    private Node GetNode(Vector3Int pos) //checks if we already have this node
    {
        if (allNodes.ContainsKey(pos))//if we have this node
        {
            return allNodes[pos];//return the node
        }
        else//else make the node
        {
            Node node = new Node(pos);
            allNodes.Add(pos, node);
            return node;
        }
    }

    //handles neighbors
    private List<Node> FindNeighbors(Vector3Int prevPos)//gets the neighbors of the current Node
    {
        List<Node> neighbors = new List<Node>();//list of the neighbor nodes
        for (int x = -1; x <= 1; x++) //goes through the x coordinates 
        {
            for (int y = -1; y <= 1; y++)//goes through the y coordinates 
            {
                Vector3Int neighborPos = new Vector3Int(prevPos.x - x, prevPos.y - y, prevPos.z);//calcs the neighbor pos
                if (y!=0 || x!=0)//if not at start
                {
                    if (neighborPos != startPos && tilemap.GetTile(neighborPos) && tilemap.GetTile<Tile>(neighborPos).sprite.name!="grass_tile_1")//if not the start, not a wall, and in the tilemap
                    {
                        Node neighbor = GetNode(neighborPos);//makes a node of the neighbor
                        neighbors.Add(neighbor);//adds the neighbor node to the neighbors list
                    }
                }
            }
        }
        return neighbors;//return the list
    }
    private void ExamineNeighbors(List<Node> neighbors, Node current)//adds neighbors to the open list and adds current to the closed
    {
        for (int i = 0; i < neighbors.Count; i++)//for each neighbor
        {
            Node neighbor = neighbors[i];//put the node in a node for cleaner code
            
            if(!ConnectedDiag(current, neighbor))
            {
                continue;
            }
            
            int gScore = determineGScore(neighbors[i].pos, current.pos);//gets the g score

            if (openList.Contains(neighbor))//if neighbor is in the openlist
            {
                if (current.G + gScore < neighbor.G)//if this path costs less than path to get to the neighbor originally
                {
                    CalcValues(current, neighbor, gScore);//gives the neighbor node it's prev and cost
                }
            }
            else if(!closedList.Contains(neighbor))//if neighbor isn't in the closed list
            {
                CalcValues(current, neighbor, gScore);//gives the neighbor node it's prev and cost
                openList.Add(neighbor);//adds the neighbors to the open list
            }
        }
        //UpdateCurrent(ref current);//puts current onto the closed list
    }

    //gets our values
    private void CalcValues(Node prev, Node neighbor, int cost)//set the previous node of the neighbors to current and gives the node a cost
    {
        neighbor.Prev = prev;//set the previous node of the neighbors to current
        neighbor.G = prev.G + cost;//the cost to get to this node
        neighbor.H = ((Math.Abs(neighbor.pos.x - goalPos.x)+Math.Abs(neighbor.pos.y - goalPos.y)) * 10);//the manhatten distance to the goal
        neighbor.F = neighbor.G + neighbor.H;//the total cost of this node
    }
    private int determineGScore(Vector3Int neighbor, Vector3Int current)//determine cost of movement
    {
        int Gscore = 0;//movement cost
        int x = current.x - neighbor.x;//movement on x
        int y = current.y - neighbor.y;//movement on y

        if (Mathf.Abs(x-y)%2 == 1)//move side to side/up down
        {
            Gscore = 10;
        }
        else//diagonal movement
        {
            Gscore = 14;
        }
        return Gscore;
    }

    //messes with the nodes
    private void UpdateCurrent(ref Node current) //puts current off the open list and into the closed
    {
        openList.Remove(current);
        closedList.Add(current);
        if (openList.Count > 0)
        {
            current = openList.OrderBy(x => x.F).First();
        }
    }
    private Stack<Vector3Int> GeneratePath(Node current)//makes the path if we're at the goal
    {
        if (current.pos == goalPos)//if at the end
        {
            Stack<Vector3Int> finalPath = new Stack<Vector3Int>();//make the path list
            while (current.pos!=startPos)//while the pos isnt the start
            {
                finalPath.Push(current.pos);//put the current node's pos on the list
                current = current.Prev;//make current current's prev node
            }
            return finalPath;//return the path
        }
        return null;//not at the end, no path
    }

    private bool ConnectedDiag(Node current, Node neighbor)
    {
        Vector3Int direct = current.pos - neighbor.pos;

        Vector3Int first = new Vector3Int(current.pos.x + (direct.x*-1), current.pos.y, current.pos.z);
        Vector3Int second = new Vector3Int(current.pos.x, current.pos.y + (direct.y * -1), current.pos.z);
        if(tilemap.GetTile<Tile>(first).sprite.name != "grass_tile_1" && tilemap.GetTile<Tile>(second).sprite.name != "grass_tile_1")
        {
            return true;
        }
        return false;
    }

    private void Algorithm()//the actual algorithm
    {
        if(current==null)
        { 
            Initialize();//makes the first node
        }

        while (openList.Count > 0 && path == null)
        {
            List<Node> neighbors = FindNeighbors(current.pos);//makes a list of the neighbors
            ExamineNeighbors(neighbors, current);//adds new neighbors to the open list and puts old nodes in the closed list
            UpdateCurrent(ref current);//updates the current node
            path = GeneratePath(current);
        }
        debug.MyInstance.CreateTiles(openList, closedList, allNodes, startPos, goalPos, path);//makes the debug tiles
    }

    //changes tiletype
    public void ChangeTileType(TileButton button)//changes the game tiletype to the button pressed
    {
        tiletype = button.MyTileType;//changes the game tiletype to the button pressed
    }
    private void ChangeTile(Vector3Int clickPos) //changes tile at pos to the current tiletype
    {
        if (tiletype == TileType.START)//for debugging
        {
            startPos = clickPos;//makes start red
        }
        else if (tiletype == TileType.GOAL)//for debugging
        {
            goalPos = clickPos;//makes goal blue
        }
        tilemap.SetTile(clickPos, tiles[(int)tiletype]);//lays down the tile type
    }
}
