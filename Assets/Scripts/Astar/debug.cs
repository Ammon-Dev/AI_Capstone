using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class debug : MonoBehaviour
{
    private static debug instance;//instates debug

    public static debug MyInstance {
        get
        {
            if (instance == null)//if nothing in instance
            {
                instance = FindObjectOfType<debug>();//instances debug
            }
            return instance;
        }
    }


    [SerializeField]//the grid
    private GridLayout grid;

    [SerializeField]//debug tile map
    private Tilemap tileMap;

    [SerializeField]
    private Tile tile;//the blank colored tile

    [SerializeField]//color change
    private Color openColor, closedColor, pathColor, rangeColor, startColor, goalColor;

    [SerializeField]//debug canvas
    private Canvas canvas;

    [SerializeField]//the arrow
    private GameObject debugTextPrefab;

    private List<GameObject> debugObjects = new List<GameObject>();//our list of arrows

    public void CreateTiles(HashSet<Node> openList, HashSet<Node> closedList, Dictionary<Vector3Int, Node> allNodes, Vector3Int start, Vector3Int goal, Stack<Vector3Int> path=null) //makes the colored tiles
    {
        foreach(Node node in openList)//for the nodes in the open list
        {
            ColorTile(node.pos, goalColor);//color them purple
        }

        foreach (Node node in closedList)//for the nodes in the closed list
        {
            ColorTile(node.pos, closedColor);//color them yellow
        }
        
        if (path != null)
        {
            foreach(Vector3Int pos in path)
            {
                if (pos != start && pos != goal)
                {
                    ColorTile(pos, pathColor);
                }
            }
        }

        ColorTile(start, startColor);//color the start red
        ColorTile(goal, goalColor);//color the end blue

        foreach (KeyValuePair<Vector3Int, Node> node in allNodes)//for the nodes we've gone through
        {
            if(node.Value.Prev != null)//if not the start
            {
                GameObject go = Instantiate(debugTextPrefab, canvas.transform);//instantiates an arrow
                go.transform.position = grid.CellToWorld(node.Key);//gives the arrow the position of the node
                debugObjects.Add(go);//adds the arrow to a list
                GenerateDebugText(node.Value, go.GetComponent<DebugText>());
            }
        }
    }

    public void CreateRangeTiles(List<Node> openList, List<Node> closedList)
    {
        for (int i = 0; i < openList.Count; i++)//for the nodes in the open list
        {
            ColorTile(openList[i].pos, rangeColor);//color them light blue
        }
        
        /*foreach (Node node in openList)
        {
            ColorTile(node.pos, rangeColor);
        }*/
        
        foreach (Node node in closedList)//for the nodes in the closed list
        {
            ColorTile(node.pos, closedColor);//color them light blue
        }
    }

    private void GenerateDebugText(Node node, DebugText debugText)
    {
        debugText.F.text = $"F:{node.F}";//displays F cost
        debugText.G.text = $"G:{node.G}";//displays G cost
        debugText.H.text = $"H:{node.H}";//displays H cost
        debugText.P.text = $"P:{node.pos.x},{node.pos.y}";//displays pos

        //rotates the arrow
        if (node.Prev.pos.x < node.pos.x && node.Prev.pos.y == node.pos.y)
        {
            debugText.MyArrow.localRotation = Quaternion.Euler(new Vector3(0, 0, 180));
        }
        else if (node.Prev.pos.x < node.pos.x && node.Prev.pos.y > node.pos.y)
        {
            debugText.MyArrow.localRotation = Quaternion.Euler(new Vector3(0, 0, 135));
        }
        else if (node.Prev.pos.x < node.pos.x && node.Prev.pos.y < node.pos.y)
        {
            debugText.MyArrow.localRotation = Quaternion.Euler(new Vector3(0, 0, 225));
        }
        else if (node.Prev.pos.x > node.pos.x && node.Prev.pos.y == node.pos.y)
        {
            debugText.MyArrow.localRotation = Quaternion.Euler(new Vector3(0, 0, 0));
        }
        else if (node.Prev.pos.x > node.pos.x && node.Prev.pos.y > node.pos.y)
        {
            debugText.MyArrow.localRotation = Quaternion.Euler(new Vector3(0, 0, 45));
        }
        else if (node.Prev.pos.x > node.pos.x && node.Prev.pos.y < node.pos.y)
        {
            debugText.MyArrow.localRotation = Quaternion.Euler(new Vector3(0, 0, -45));
        }
        else if (node.Prev.pos.x == node.pos.x && node.Prev.pos.y > node.pos.y)
        {
            debugText.MyArrow.localRotation = Quaternion.Euler(new Vector3(0, 0, 90));
        }
        else if (node.Prev.pos.x == node.pos.x && node.Prev.pos.y < node.pos.y)
        {
            debugText.MyArrow.localRotation = Quaternion.Euler(new Vector3(0, 0, 270));
        }
    }

    public void ColorTile(Vector3Int pos, Color color) //sets the colored tiles on the debug map
    {
        tileMap.SetTile(pos, tile);//puts a blank tile in the given pos
        tileMap.SetTileFlags(pos, TileFlags.None);//no tile flags
        tileMap.SetColor(pos, color);//colors the tile
    }

}
