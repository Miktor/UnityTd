using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable, CreateAssetMenu]
public class LevelData : ScriptableObject
{
    [Serializable]
    public enum TileType : int
    {
        None = 0,
        Occupied,
        Tower
    }

    public TileType[,] Tiles;

    public string MapName;
    public WaveData[] Waves;
}
