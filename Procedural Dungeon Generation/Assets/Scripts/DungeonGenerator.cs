using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = System.Random;

/// <summary>
/// When the BSP algorithm has finished calculating the rooms, paint the rooms using a tilemap.
/// </summary>
public class DungeonGenerator : MonoBehaviour
{
    public const int MIN_ROOM_DELTA = 2;

    [SerializeField] private int dungeonSize;
    [SerializeField] private Tile debugTile;

    [Range(1, 6)] [SerializeField] private int numberOfIterations;
    [Range(1, 4)] [SerializeField] private int corridorThickness;

    [SerializeField] private bool shouldDebugDrawBSP; // Used to visualize the containers.

    // A reference to the tiles used in the Custom Editor.
    [HideInInspector]
    public Tile tlTile;
    [HideInInspector]
    public Tile tmTile;
    [HideInInspector]
    public Tile trTile;
    [HideInInspector]
    public Tile mlTile;
    [HideInInspector]
    public Tile mmTile;
    [HideInInspector]
    public Tile mrTile;
    [HideInInspector]
    public Tile blTile;
    [HideInInspector]
    public Tile bmTile;
    [HideInInspector]
    public Tile brTile;

    private Tilemap map;
    private BSPTree tree;

    /// <summary>
    /// Request to draw gizmos of the containers.
    /// </summary>
    private void OnDrawGizmos()
    {
        AttemptDebugDrawBSP();
    }

    /// <summary>
    /// Request to draw gizmos of the containers.
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        AttemptDebugDrawBSP();
    }

    /// <summary>
    /// Only draw gizmos if shouldDebugDrawBSP is true.
    /// </summary>
    void AttemptDebugDrawBSP()
    {
        if (shouldDebugDrawBSP)
        {
            DebugDrawBSP();
        }
    }

    /// <summary>
    /// Draw gizmos of the containers, if a Binary Tree is present.
    /// </summary>
    public void DebugDrawBSP()
    {
        if (tree == null) return; // Hasn't been generated yet.

        DebugDrawBSPNode(tree); // Recursive call.
    }

    /// <summary>
    /// Draw lines around each of the containers and its children.
    /// </summary>
    /// <param name="node"></param>
    public void DebugDrawBSPNode(BSPTree node)
    {
        // Container
        Gizmos.color = Color.green;
        // Top
        Gizmos.DrawLine(new Vector3(node.container.x, node.container.y, 0), new Vector3Int(node.container.xMax, node.container.y, 0));
        // Right
        Gizmos.DrawLine(new Vector3(node.container.xMax, node.container.y, 0), new Vector3Int(node.container.xMax, node.container.yMax, 0));
        // Bottom
        Gizmos.DrawLine(new Vector3(node.container.x, node.container.yMax, 0), new Vector3Int(node.container.xMax, node.container.yMax, 0));
        // Left
        Gizmos.DrawLine(new Vector3(node.container.x, node.container.y, 0), new Vector3Int(node.container.x, node.container.yMax, 0));

        // Children
        if (node.left != null) DebugDrawBSPNode(node.left);
        if (node.right != null) DebugDrawBSPNode(node.right);
    }

    /// <summary>
    /// Generate the dungeon.
    /// </summary>
    public void GenerateDungeon()
    {
        InitReferences();
        GenerateContainersUsingBSP();
        GenerateRoomsInsideContainers();
        GenerateCorridors();
        FillRoomsOnTilemap();
        PaintTilesAccordingToTheirNeighbours();
    }

    /// <summary>
    /// Reset references and clear the map before generating a dungeon.
    /// </summary>
    private void InitReferences()
    {
        map = GetComponentInChildren<Tilemap>();
        map.ClearAllTiles();
    }
    
    /// <summary>
    /// Using BSP, split the container into two, keep going until all iterations are finished.
    /// </summary>
    private void GenerateContainersUsingBSP()
    {
        tree = BSPTree.Split(numberOfIterations,
            new RectInt(0, 0, dungeonSize, dungeonSize));
    }

    /// <summary>
    /// For each container node, generate a room.
    /// </summary>
    private void GenerateRoomsInsideContainers()
    {
        BSPTree.GenerateRoomsInsideContainersNode(tree);
    }

    /// <summary>
    /// For each parent room,
    /// Find their center,
    /// find a direction and connect these centers
    /// </summary>
    private void GenerateCorridors()
    {
        GenerateCorridorsNode(tree);
    }

    /// <summary>
    /// Find the center of each non-leaf container and connect them to each other..
    /// </summary>
    /// <param name="node"></param>
    private void GenerateCorridorsNode(BSPTree node)
    {
        if (node.IsInternal())
        {
            RectInt leftContainer = node.left.container;
            RectInt rightContainer = node.right.container;
            Vector2 leftCenter = leftContainer.center;
            Vector2 rightCenter = rightContainer.center;
            Vector2 direction = (rightCenter - leftCenter).normalized; // Arbitrary choosing right as the target point.

            while (Vector2.Distance(leftCenter, rightCenter) > 1)
            {
                if (direction.Equals(Vector2.right))
                {
                    for (int i = 0; i < corridorThickness; i++)
                    {
                        map.SetTile(new Vector3Int((int)leftCenter.x, (int)leftCenter.y + i, 0), mmTile);
                    }
                }
                else if (direction.Equals(Vector2.up))
                {
                    for (int i = 0; i < corridorThickness; i++)
                    {
                        map.SetTile(new Vector3Int((int)leftCenter.x + i, (int)leftCenter.y, 0), mmTile);
                    }
                }

                leftCenter.x += direction.x;
                leftCenter.y += direction.y;
            }

            if (node.left != null) GenerateCorridorsNode(node.left);
            if (node.right != null) GenerateCorridorsNode(node.right);
        }
    }

    /// <summary>
    /// Call for a DFS walk through the tree to paint the tiles in each room.
    /// </summary>
    private void FillRoomsOnTilemap()
    {
        UpdateTilemapUsingTreeNode(tree);
    }

    /// <summary>
    /// Use DFS to walk through the tree and paint tiles in each leaf node.
    /// </summary>
    /// <param name="node"></param>
    private void UpdateTilemapUsingTreeNode(BSPTree node)
    {
        if (node.IsLeaf())
        {
            for (int i = node.room.x; i < node.room.xMax; i++)
            {
                for (int j = node.room.y; j < node.room.yMax; j++)
                {
                    map.SetTile(new Vector3Int(i, j, 0), mmTile);
                }
            }
        }
        else
        {
            if (node.left != null) UpdateTilemapUsingTreeNode(node.left);
            if (node.right != null) UpdateTilemapUsingTreeNode(node.right);
        }
    }

    /// <summary>
    /// For each tile, check each neighbour surrounding the tile.
    /// Decide, based on the position of each neighbour, which kind of tile this should be.
    /// </summary>
    /// <param name="i"></param>
    /// <param name="j"></param>
    /// <returns>Default mmTile, otherwise specified tile according to the rules</returns>
    private Tile GetTileByNeighbours(int i, int j)
    {
        var mmGridTile = map.GetTile(new Vector3Int(i, j, 0));

        if (mmGridTile == null) return null; // You shouldn't repaint a null

        var blGridTile = map.GetTile(new Vector3Int(i - 1, j - 1, 0));
        var bmGridTile = map.GetTile(new Vector3Int(i, j - 1, 0));
        var brGridTile = map.GetTile(new Vector3Int(i + 1, j - 1, 0));

        var mlGridTile = map.GetTile(new Vector3Int(i - 1, j, 0));
        var mrGridTile = map.GetTile(new Vector3Int(i + 1, j, 0));

        var tlGridTile = map.GetTile(new Vector3Int(i - 1, j + 1, 0));
        var tmGridTile = map.GetTile(new Vector3Int(i, j + 1, 0));
        var trGridTile = map.GetTile(new Vector3Int(i + 1, j + 1, 0));

        // we have 8 + 1 cases

        // left
        if (mlGridTile == null && tmGridTile == null) return tlTile;
        if (mlGridTile == null && tmGridTile != null && bmGridTile != null) return mlTile;
        if (mlGridTile == null && bmGridTile == null && tmGridTile != null) return blTile;

        // middle
        if (mlGridTile != null && tmGridTile == null && mrGridTile != null) return tmTile;
        if (mlGridTile != null && bmGridTile == null && mrGridTile != null) return bmTile;

        // right
        if (mlGridTile != null && tmGridTile == null && mrGridTile == null) return trTile;
        if (tmGridTile != null && bmGridTile != null && mrGridTile == null) return mrTile;
        if (tmGridTile != null && bmGridTile == null && mrGridTile == null) return brTile;

        return mmTile; // default case
    }

    /// <summary>
    /// Loop through all the rooms and paint each tile according to the ruleset.
    /// </summary>
    private void PaintTilesAccordingToTheirNeighbours()
    {
        for (int i = MIN_ROOM_DELTA; i < dungeonSize; i++)
        {
            for (int j = MIN_ROOM_DELTA; j < dungeonSize; j++)
            {
                var tile = GetTileByNeighbours(i, j);

                if (tile != null)
                {
                    map.SetTile(new Vector3Int(i, j, 0), tile);
                }
            }
        }
    }
}
