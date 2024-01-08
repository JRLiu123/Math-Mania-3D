using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyNamespace;
public class Node
{
    public int[] cell;
    public float gScore { get; set; }
    public float hScore { get; set; }
    public float fScore { get; set; } 
    public Node Next { get; set; }
    public Node Previous { get; set; }

    public Node(int[] cell)
    {
        this.cell = cell;
        this.gScore = float.PositiveInfinity;
        this.hScore = float.PositiveInfinity; 
        this.fScore = float.PositiveInfinity; 
        Next = null;
    }

}
