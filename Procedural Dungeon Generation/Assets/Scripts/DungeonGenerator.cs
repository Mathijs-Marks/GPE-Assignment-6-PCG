using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonGenerator : MonoBehaviour
{
    private const int MIN_ROOM_DELTA = 2;
    [SerializeField] private int dungeonSize;

    [Range(1, 6)] [SerializeField] private int numberOfIterations;
    [Range(1, 4)] [SerializeField] private int corridorThickness;

    [SerializeField] private bool shouldDebugDrawBSP;

    private BSPTree tree;

    public void GenerateDungeon()
    {

    }
}
