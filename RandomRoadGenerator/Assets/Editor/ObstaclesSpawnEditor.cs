using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

[CustomEditor(typeof(ObstaclesSpawn))]
public class ObstaclesSpawnEditor : Editor
{
    private ObstaclesSpawn _obstacles;

    private void OnEnable()
    {
        _obstacles = (ObstaclesSpawn)target;
    }

    public override void OnInspectorGUI()
    {
        EditorGUILayout.HelpBox("Obstacle spawner.\n" +
                                "Remember to check if all the measurements are correct and that your obstacles have assigned type!\n" +
                                "Areas are spawning from the middle. Take it into the account while modifying the spacing.",
            MessageType.Info);
        base.OnInspectorGUI();

        if (GUILayout.Button("Control Spawning"))
        {
            ControlledSpawn();
        }

        if (_obstacles.ControlledSpawn)
        {
            EditorGUILayout.HelpBox("If you see this message, you are controlling the spawn!\n" +
                                    "If you want to go back to randomised one, click the button",
                MessageType.Warning);
            EditorGUILayout.HelpBox("To use a specific area, mark it active in the inspector.\n" +
                                    "If you specify, what exact object should spawn, click on the Area [number] object to reveal field to put your specific object in.\n" +
                                    "Area 1 is the closest one to the start of the path!",
                MessageType.Info);
            EditorGUI.indentLevel++;
            EditorGUILayout.BeginHorizontal();
            _obstacles.IsAreaThree = EditorGUILayout.Toggle("Use Area1", _obstacles.IsAreaThree);
            if (GUILayout.Button("Area 1 object")) ChooseAreaObjects(1);
            EditorGUILayout.EndHorizontal();
            if (_obstacles.ControlAreaThree && _obstacles.IsAreaThree)
            {
                _obstacles.Area3Object =
                    (GameObject)EditorGUILayout.ObjectField("Obstacle", _obstacles.Area3Object, typeof(GameObject),
                        true);
            }
            EditorGUILayout.BeginHorizontal();
            _obstacles.IsAreaOne = EditorGUILayout.Toggle("Use Area2", _obstacles.IsAreaOne);
            if (GUILayout.Button("Area 2 object")) ChooseAreaObjects(3);
            EditorGUILayout.EndHorizontal();
            if (_obstacles.ControlAreaOne && _obstacles.IsAreaOne)
            {
                _obstacles.Area1Object =
                    (GameObject)EditorGUILayout.ObjectField("Obstacle", _obstacles.Area1Object, typeof(GameObject),
                        true);
            }
            EditorGUILayout.BeginHorizontal();
            _obstacles.IsAreaTwo = EditorGUILayout.Toggle("Use Area3", _obstacles.IsAreaTwo);
            if (GUILayout.Button("Area 3 object")) ChooseAreaObjects(2);
            EditorGUILayout.EndHorizontal();
            if (_obstacles.ControlAreaTwo && _obstacles.IsAreaTwo)
            {
                _obstacles.Area2Object =
                    (GameObject)EditorGUILayout.ObjectField("Obstacle", _obstacles.Area2Object, typeof(GameObject),
                        true);
            }
            EditorGUI.indentLevel--;
        }
        else
        {
            _obstacles.IsAreaOne = false;
            _obstacles.IsAreaTwo = false;
            _obstacles.IsAreaThree = false;
            _obstacles.ControlAreaOne = false;
            _obstacles.ControlAreaTwo = false;
            _obstacles.ControlAreaThree = false;
        }

        if (GUI.changed)
        {
            EditorUtility.SetDirty(_obstacles);
            EditorSceneManager.MarkSceneDirty(_obstacles.gameObject.scene);
        }
    }

    private void OnSceneGUI()
    {
        Handles.color = Color.red;
        foreach (Vector3 point in _obstacles.AreasPoints)
        {
            Handles.DrawSolidDisc(point, Vector3.up, 0.05f);
        }
    }

    private void ControlledSpawn()
    {
        _obstacles.ControlledSpawn = !_obstacles.ControlledSpawn;
    }

    private void ChooseAreaObjects(int areaNumber)
    {
        switch (areaNumber)
        {
            case 1:
                _obstacles.ControlAreaOne = !_obstacles.ControlAreaOne;
                break;
            case 2:
                _obstacles.ControlAreaTwo = !_obstacles.ControlAreaTwo;
                break;
            case 3:
                _obstacles.ControlAreaThree = !_obstacles.ControlAreaThree;
                break;
        }
    }
}