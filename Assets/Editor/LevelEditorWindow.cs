using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System;

class LevelEditorWindow : EditorWindow
{
    [MenuItem("Window/TD Level Editor")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(LevelEditorWindow));
    }

    private GameObject _activeObject;
    private LevelData _levelData;
    private Vector2Int _dimentions = new Vector2Int(32, 32);

    private const float CellSize = 15f;

    private GUIStyle _grassVoxel;
    private GUIStyle _dirtVoxel;
    private GUIStyle _towerVoxel;
    private bool _leftMouseButtonDown = false;
    private HashSet<Vector2Int> _visitedCells = new HashSet<Vector2Int>();
    private bool _isDirty;
    private Vector2 _scrollPosition;
    private GameManager _gameManager;

    private GUIStyle GrassVoxel
    {
        get
        {
            if (_grassVoxel == null)
            {
                _grassVoxel = new GUIStyle("GUI.skin.button");
                _grassVoxel.normal.background = MakeTex(2, 2, Color.green);
            }

            return _grassVoxel;
        }
    }

    private GUIStyle DirtVoxel
    {
        get
        {
            if (_dirtVoxel == null)
            {
                _dirtVoxel = new GUIStyle("GUI.skin.button");
                _dirtVoxel.normal.background = MakeTex(2, 2, Color.cyan);
            }

            return _dirtVoxel;
        }
    }

    private GUIStyle TowerVoxel
    {
        get
        {
            if (_towerVoxel == null)
            {
                _towerVoxel = new GUIStyle("GUI.skin.button");
                _towerVoxel.normal.background = MakeTex(2, 2, Color.gray);
            }

            return _towerVoxel;
        }
    }

    private LevelData GetLevelData(GameObject activeObject)
    {
        _gameManager = activeObject.GetComponent<GameManager>();
        if (_gameManager == null)
            return null;

        return _gameManager.currentLevel;
    }

    void OnGUI()
    {
        var activeObject = Selection.activeGameObject;
        if (activeObject == null)
            return;

        _levelData = GetLevelData(activeObject);
        if (_levelData == null)
            return;

        if (_levelData.Tiles == null)
        {
            CreateTiles();
        }

        EditorGUILayout.TextField("Level Name: ", _levelData.MapName);

        EditorGUI.BeginChangeCheck();
        _dimentions = EditorGUILayout.Vector2IntField("Dimensions", _dimentions);
        if (EditorGUI.EndChangeCheck())
        {
            CreateTiles();
        }

        var mouseMove = false;
        var mouseReleased = false;
        if (Event.current.button == 0)
        {
            switch (Event.current.type)
            {
                case EventType.MouseDown:
                    _leftMouseButtonDown = true;
                    break;
                case EventType.MouseUp:
                    _visitedCells.Clear();
                    _leftMouseButtonDown = false;
                    mouseReleased = true;
                    break;
                case EventType.MouseDrag:
                    mouseMove = true;
                    break;
            }
        }

        EditorGUI.BeginChangeCheck();
        EditorGUILayout.BeginScrollView(_scrollPosition);
        for (var x = 0; x < _dimentions.x; x++)
        {
            for (var y = 0; y < _dimentions.y; y++)
            {
                var tileType = _levelData.Tiles[x, y];
                GUIStyle style = null;
                switch (tileType)
                {
                    case LevelData.TileType.None:
                        style = DirtVoxel;
                        break;
                    case LevelData.TileType.Occupied:
                        style = GrassVoxel;
                        break;
                    case LevelData.TileType.Tower:
                        style = TowerVoxel;
                        break;
                }

                var currentRect = new Rect(x * CellSize + x, y * CellSize + y, CellSize, CellSize);
                var pos = new Vector2Int(x, y);
                GUI.Button(currentRect, GUIContent.none, style);

                if (!_visitedCells.Contains(pos))
                    if (currentRect.Contains(Event.current.mousePosition)
                            && _leftMouseButtonDown
                            && mouseMove)
                    {
                        _levelData.Tiles[x, y] = tileType.Next();
                        _visitedCells.Add(pos);
                        _isDirty = true;
                    }
            }
        }

        EditorGUILayout.EndScrollView();
        EditorGUI.EndChangeCheck();

        if (GUILayout.Button("Reset map"))
        {
            ResetTiles();
            _isDirty = true;
        }

        if (mouseReleased && _isDirty)
        {
            SaveAssets();
        }
    }

    private void SaveAssets()
    {
        _gameManager.GenerateMap();
        EditorUtility.SetDirty(_levelData);
        AssetDatabase.SaveAssets();
        _isDirty = false;
        Debug.Log($"Saved!");
    }

    private void CreateTiles()
    {
        _levelData.Tiles = new LevelData.TileType[_dimentions.x, _dimentions.y];
        ResetTiles();
    }
    private void ResetTiles()
    {
        for (int i = 0; i < _levelData.Tiles.GetLength(0); ++i)
            for (int j = 0; j < _levelData.Tiles.GetLength(1); ++j)
                _levelData.Tiles[i, j] = LevelData.TileType.Occupied;

        SaveAssets();
    }

    private Texture2D MakeTex(int width, int height, Color col)
    {
        Color[] pix = new Color[width * height];
        for (int i = 0; i < pix.Length; ++i)
        {
            pix[i] = col;
        }

        var result = new Texture2D(width, height);
        result.SetPixels(pix);
        result.Apply();
        return result;
    }
}
