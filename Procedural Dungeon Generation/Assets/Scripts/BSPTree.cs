using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The Binary Space Partitioning algorithm makes use of a Binary Tree to cut a 64x64/128x128 size room into smaller rooms.
/// Depth First Search is used to navigate the tree.
/// </summary>
public class BSPTree
{
    public RectInt container; // A 2D rectangle representing the cut areas by the BSP algorithm.
    public RectInt room; // A 2D rectangle painted with tiles to represent the dungeon rooms.
    public BSPTree left; // Define the binary tree recursively to the left.
    public BSPTree right; // Define the binary tree recursively to the right.

    /// <summary>
    /// Construct the 2D rectangle containers using the binary tree.
    /// </summary>
    /// <param name="container"></param>
    public BSPTree(RectInt container)
    {
        this.container = container;
    }

    /// <summary>
    /// If no neighbours are present, then we are on a leaf node.
    /// </summary>
    /// <returns>Boolean indicating a leaf node</returns>
    public bool IsLeaf()
    {
        return left == null && right == null;
    }

    /// <summary>
    /// If any neighbour is present, then we are on an internal node.
    /// </summary>
    /// <returns>Boolean indicating an internal node</returns>
    public bool IsInternal()
    {
        return left != null || right != null;
    }

    /// <summary>
    /// Until the numberOfIterations reaches 0, delegate (recursively) the splitting of each container to the SplitContainer() method.
    /// </summary>
    /// <param name="numberOfIterations"></param>
    /// <param name="container"></param>
    /// <returns>Tree node after splitting is finished</returns>
    internal static BSPTree Split(int numberOfIterations, RectInt container)
    {
        var node = new BSPTree(container);
        if (numberOfIterations == 0) return node;

        var splittedContainers = SplitContainer(container);
        node.left = Split(numberOfIterations - 1, splittedContainers[0]);
        node.right = Split(numberOfIterations - 1, splittedContainers[1]);

        return node;
    }

    /// <summary>
    /// Randomly choose a cut direction and split a container into two containers accordingly.
    /// </summary>
    /// <param name="container"></param>
    /// <returns>2 split containers</returns>
    private static RectInt[] SplitContainer(RectInt container)
    {
        RectInt c1, c2;

        if (UnityEngine.Random.Range(0f, 1f) > 0.5f)
        {
            // Vertical
            c1 = new RectInt(container.x, container.y,
                container.width, (int) UnityEngine.Random.Range(container.height * 0.3f, container.height * 0.5f));
            c2 = new RectInt(container.x, container.y + c1.height,
                container.width, container.height - c1.height);
        }
        else
        {
            // Horizontal
            c1 = new RectInt(container.x, container.y,
                (int) UnityEngine.Random.Range(container.width * 0.3f, container.width * 0.5f), container.height);
            c2 = new RectInt(container.x + c1.width, container.y,
                container.width - c1.width, container.height);
        }

        return new RectInt[] {c1, c2};
    }

    /// <summary>
    /// Navigate the tree using DFS and generate rooms in each leaf node.
    /// </summary>
    /// <param name="node"></param>
    public static void GenerateRoomsInsideContainersNode(BSPTree node)
    {
        // Should create rooms for leafs
        if (node.left == null && node.right == null)
        {
            var randomX = UnityEngine.Random.Range(DungeonGenerator.MIN_ROOM_DELTA, node.container.width / 4);
            var randomY = UnityEngine.Random.Range(DungeonGenerator.MIN_ROOM_DELTA, node.container.height / 4);
            int roomX = node.container.x + randomX;
            int roomY = node.container.y + randomY;
            int roomW = (node.container.width - (int)(randomX * UnityEngine.Random.Range(1f, 2f)));
            int roomH = node.container.height - (int) (randomY * UnityEngine.Random.Range(1f, 2f));
            node.room = new RectInt(roomX, roomY, roomW, roomH);
        }
        else
        {
            if(node.left != null) GenerateRoomsInsideContainersNode(node.left);
            if(node.right != null) GenerateRoomsInsideContainersNode(node.right);
        }
    }
}
