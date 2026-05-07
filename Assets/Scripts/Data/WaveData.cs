using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class EnemySpawn
{
    public GameObject Prefab;
    public Vector2Int Tile;
}

[Serializable]
public class ReinforcementSpawn
{
    public GameObject Prefab;
    public int        Column;
}

[Serializable]
public class ReinforcementBatch
{
    public List<ReinforcementSpawn> Spawns = new();
}

[CreateAssetMenu(menuName = "Chess Invaders/Wave Data", fileName = "WaveData")]
public class WaveData : ScriptableObject
{
    // Enemies placed at exact tile positions when the wave starts.
    public List<EnemySpawn> InitialSpawns = new();

    // One batch per step tick. Batch[0] spawns after the first step, Batch[1] after the second, etc.
    // Row is always the back rank — only Column is used from each spawn.
    public List<ReinforcementBatch> Reinforcements = new();
}
