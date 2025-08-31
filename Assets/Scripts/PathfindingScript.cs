using UnityEngine;
using Scripts;
using UnityEditor.Experimental.GraphView;
using System.Collections.Generic;

//Comment of shame
//I did this mostly with Tuts and AI cause I couldnt figure it out myself.
public class PathfindingScript : MonoBehaviour {

    private GameController gameController;
    public Nodes[,] nodes;

    void Awake() {
        gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
    }

    private void Start() {
        GenerateNodesArray();
    }

    // Update is called once per frame
    void Update() {

    }

    private void GenerateNodesArray() {
        Vector2Int mapSize = gameController.getMapSize();
        nodes = new Nodes[mapSize.x, mapSize.y];

        for (int x = 0; x < mapSize.x; x++) {
            for (int y = 0; y < mapSize.y; y++) {
                bool walkable = gameController.GetTileValue(x, y) == TileValue.Empty;
                nodes[x, y] = new Nodes(x, y, walkable);
            }
        }
        //Debug.Log("Generated Node Array");
    }

    private List<Nodes> GetNeighbors(Nodes node) {
        List<Nodes> neighbors = new List<Nodes>();
        int[,] dirs = new int[,] { { 0, 1 }, { 1, 0 }, { 0, -1 }, { -1, 0 } };

        for (int i = 0; i < dirs.GetLength(0); i++) {
            int nx = node.x + dirs[i, 0];
            int ny = node.y + dirs[i, 1];

            // TODO: please invert the statement to reduce depth
            if (nx >= 0 && nx < nodes.GetLength(0) && ny >= 0 && ny < nodes.GetLength(1)) {
                neighbors.Add(nodes[nx, ny]);
            }
        }

        return neighbors;
    }

    public List<Nodes> FindPath(Nodes startNode, Nodes targetNode) {
        List<Nodes> openSet = new List<Nodes> { startNode };
        HashSet<Nodes> closedSet = new HashSet<Nodes>();

        // TODO: please invert it all. I can't take it anymore
        while (openSet.Count > 0) {
            Nodes current = openSet[0];
            for (int i = 1; i < openSet.Count; i++) {
                if (openSet[i].fCost < current.fCost ||
                   ( openSet[i].fCost == current.fCost && openSet[i].hCost < current.hCost )) {
                    current = openSet[i];
                }
            }

            if (current == targetNode) {
                return RetracePath(startNode, targetNode);
            }

            openSet.Remove(current);
            closedSet.Add(current);

            foreach (Nodes neighbor in GetNeighbors(current)) {
                if (!neighbor.walkable || closedSet.Contains(neighbor)) {
                    continue;
                }
                int newCost = current.gCost + gameController.GetDistanceToTile(current.x, current.y, neighbor.x, neighbor.y);
                if (newCost < neighbor.gCost || !openSet.Contains(neighbor)) {
                    neighbor.gCost = newCost;
                    neighbor.hCost = gameController.GetDistanceToTile(neighbor.x, neighbor.y, targetNode.x, targetNode.y);
                    neighbor.parent = current;

                    if (!openSet.Contains(neighbor)) {
                        openSet.Add(neighbor);
                    }
                }
            }
        }

        return null;
    }

    private List<Nodes> RetracePath(Nodes startNode, Nodes endNode) {
        List<Nodes> path = new List<Nodes>();
        Nodes current = endNode;

        while (current != startNode) {
            path.Add(current);
            current = current.parent;
        }

        path.Reverse();
        return path;
    }
}
