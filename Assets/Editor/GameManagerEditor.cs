using UnityEngine;
using UnityEditor;
using System;

[CustomEditor(typeof(GameManager))]
public class GameManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        GameManager manager = (GameManager)target;
        if (manager.TilePrefabs == null)
            manager.TilePrefabs = new Transform[Enum.GetValues(typeof(LevelData.TileType)).GetLength(0)];

        foreach (int enumValue in Enum.GetValues(typeof(LevelData.TileType)))
        {
            manager.TilePrefabs[enumValue] = EditorGUILayout.ObjectField(
                Enum.GetName(typeof(LevelData.TileType), enumValue),
                manager.TilePrefabs[enumValue],
                typeof(Transform), false) as Transform;
        }
    }
}