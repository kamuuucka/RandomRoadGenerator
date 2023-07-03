using LevelGeneration;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Editor
{
    [CustomEditor(typeof(RoadGenerator))]
    public class RoadGeneratorEditor : UnityEditor.Editor
    {
        private RoadGenerator _generator;
        private bool _showHiddenFields;
        private int _roadPiecesRegistered;

        private void OnEnable()
        {
            _generator = (RoadGenerator)target;
            _showHiddenFields = false;
        }

        public override void OnInspectorGUI()
        {
       
        
            EditorGUILayout.HelpBox("Generator for the infinite road. Remember to add the road pieces to the list.\n" +
                                    "Keep the generator at 0 Y.",
                MessageType.Info);
            EditorGUILayout.HelpBox("It is important to set up generator!\n" +
                                    "1. Empty straight road - starting piece\n" +
                                    "2. Straights (pipes, obstacles, etc) \n" +
                                    "3. Lefts \n" +
                                    "4. Rights \n" +
                                    "5.CrossRoads", MessageType.Warning);
            base.OnInspectorGUI();

            if (GUILayout.Button("Show Debug"))
            {
                ShowDebugFields();
            }

            if (_showHiddenFields)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.LabelField("Debug", EditorStyles.boldLabel);
                _generator.ShowBorder = GUILayout.Toggle(_generator.ShowBorder, "Show border");

                EditorGUILayout.LabelField("Advanced settings", EditorStyles.boldLabel);
                EditorGUILayout.HelpBox(
                    "Advanced settings used for easier debugging. If you're not sure you should change them, please don't.",
                    MessageType.Error);
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Space from the border (N)");
                EditorGUILayout.Space();
                _generator.NormalRoadBorderSpace =
                    EditorGUILayout.IntField(_generator.NormalRoadBorderSpace, GUILayout.ExpandWidth(false));
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Space from the border (C)");
                EditorGUILayout.Space();
                _generator.CrossRoadBorderSpace =
                    EditorGUILayout.IntField(_generator.CrossRoadBorderSpace, GUILayout.ExpandWidth(false));
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.LabelField("Default Road Rotation");
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("X:", GUILayout.MaxWidth(25.0f));
                _generator.DefaultRotationX =
                    EditorGUILayout.FloatField(_generator.DefaultRotationX, GUILayout.MaxWidth(50.0f));
                EditorGUILayout.LabelField("Y:", GUILayout.MaxWidth(25.0f));
                _generator.DefaultRotationY =
                    EditorGUILayout.FloatField(_generator.DefaultRotationY, GUILayout.MaxWidth(50.0f));
                EditorGUILayout.LabelField("Z:", GUILayout.MaxWidth(25.0f));
                _generator.DefaultRotationZ =
                    EditorGUILayout.FloatField(_generator.DefaultRotationZ, GUILayout.MaxWidth(50.0f));
                EditorGUILayout.EndHorizontal();

                EditorGUI.indentLevel--;
            
                if (GUI.changed)
                {
                    EditorUtility.SetDirty(_generator);
                    EditorSceneManager.MarkSceneDirty(_generator.gameObject.scene);
                }
            }
        
        
        }

        private void OnSceneGUI()
        {
            if (_generator.ShowBorder)
            {
                Handles.color = Color.red;
                Handles.DrawWireCube(_generator.transform.position,
                    new Vector3(_generator.BorderSize, 1, _generator.BorderSize));
            }
        }

        private void ShowDebugFields()
        {
            _showHiddenFields = !_showHiddenFields;
        }
    }
}