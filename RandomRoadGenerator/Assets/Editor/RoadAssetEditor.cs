using UnityEditor;
using UnityEngine;


[CustomEditor(typeof(RoadPoints))]
public class RoadAssetEditor : UnityEditor.Editor
{
    private RoadPoints _road;
    [SerializeField] private Vector3 _assetStartEditor;


    private Vector3 _assetEnd;
    private Vector3 _assetLeft;
    private Vector3 _assetRight;

    private Bounds _bounds;

    private void OnEnable()
    {
        _road = (RoadPoints)target;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        EditorGUILayout.HelpBox("Green point - start\n" +
                                "Red point - end\n" +
                                "Orange point - left\n" +
                                "To check if the points are generated correctly, click on the path in the editor",
            MessageType.Info);
    }

    private void OnSceneGUI()
    {
        _bounds = _road.GetComponent<Renderer>().bounds;
        int roadRotation = ((int)_road.transform.localRotation.eulerAngles.y + 360) % 360;

        float simpleX = _bounds.size.x / 2;
        float simpleZ = _bounds.size.z / 2;
        float lengthZ = _bounds.size.z / (2 * _road.Length);
        float widthX = _bounds.size.x / (2 * _road.Width);
        float xMinusWidthX = simpleX - widthX;
        float xPlusWidthX = -simpleX + widthX;
        float zMinusLengthZ = simpleZ - lengthZ;
        float zPlusLengthZ = -simpleZ + lengthZ;
        switch (_road.TypeOfRoad)
        {
            case RoadPoints.RoadType.Straight when roadRotation == 0:
                SpawnPoints(-simpleX, 0, simpleX, 0);
                break;
            case RoadPoints.RoadType.Straight when roadRotation == 90:
                SpawnPoints(0, simpleZ, 0, -simpleZ);
                break;
            case RoadPoints.RoadType.Straight when roadRotation == 180:
                SpawnPoints(simpleX, 0, -simpleX, 0);
                break;
            case RoadPoints.RoadType.Straight when roadRotation == 270:
                SpawnPoints(0, -simpleZ, 0, simpleZ);
                break;
            //RIGHT-------------------------------------------------------------------------------------------------
            case RoadPoints.RoadType.Right when roadRotation == 0:
                SpawnPoints(-simpleX, zMinusLengthZ - _road.SpecialOffset, xMinusWidthX - _road.SpecialOffset, -simpleZ,
                    true, xMinusWidthX - _road.SpecialOffset, zMinusLengthZ - _road.SpecialOffset);
                break;
            case RoadPoints.RoadType.Right when roadRotation == 90:
                SpawnPoints(xMinusWidthX - _road.SpecialOffset, simpleZ, -simpleX, zPlusLengthZ + _road.SpecialOffset,
                    true, xMinusWidthX - _road.SpecialOffset, zPlusLengthZ + _road.SpecialOffset);
                break;
            case RoadPoints.RoadType.Right when roadRotation == 180:
                SpawnPoints(simpleX, zPlusLengthZ + _road.SpecialOffset, xPlusWidthX + _road.SpecialOffset, simpleZ,
                    true, xPlusWidthX + _road.SpecialOffset, zPlusLengthZ + _road.SpecialOffset);
                break;
            case RoadPoints.RoadType.Right when roadRotation == 270:
                SpawnPoints(xPlusWidthX + _road.SpecialOffset, -simpleZ, simpleX, zMinusLengthZ - _road.SpecialOffset,
                    true, xPlusWidthX + _road.SpecialOffset, zMinusLengthZ - _road.SpecialOffset);
                break;
            //LEFT--------------------------------------------------------------------------------------------------
            case RoadPoints.RoadType.Left when roadRotation == 0:
                SpawnPoints(-simpleX, zPlusLengthZ + _road.SpecialOffset, xMinusWidthX - _road.SpecialOffset, simpleZ,
                    true, xMinusWidthX - _road.SpecialOffset, zPlusLengthZ + _road.SpecialOffset);
                break;
            case RoadPoints.RoadType.Left when roadRotation == 90:
                SpawnPoints(xPlusWidthX + _road.SpecialOffset, simpleZ, simpleX, zPlusLengthZ + _road.SpecialOffset,
                    true, xPlusWidthX + _road.SpecialOffset, zPlusLengthZ + _road.SpecialOffset);
                break;
            case RoadPoints.RoadType.Left when roadRotation == 180:
                SpawnPoints(simpleX, zMinusLengthZ - _road.SpecialOffset, xPlusWidthX + _road.SpecialOffset, -simpleZ,
                    true, xPlusWidthX + _road.SpecialOffset, zMinusLengthZ - _road.SpecialOffset);
                break;
            case RoadPoints.RoadType.Left when roadRotation == 270:
                SpawnPoints(xMinusWidthX - _road.SpecialOffset, -simpleZ, -simpleX, zMinusLengthZ - _road.SpecialOffset,
                    true, xMinusWidthX - _road.SpecialOffset, zMinusLengthZ - _road.SpecialOffset);
                break;
            //CROSSROAD---------------------------------------------------------------------------------------------
            case RoadPoints.RoadType.Crossroad when roadRotation == 0:
                SpawnPoints(-simpleX, 0, xMinusWidthX, -simpleZ,
                    true, xMinusWidthX, simpleZ);
                break;
            case RoadPoints.RoadType.Crossroad when roadRotation == 90:
                SpawnPoints(0, simpleZ, -simpleX, zPlusLengthZ,
                    true, simpleX, zPlusLengthZ);
                break;
            case RoadPoints.RoadType.Crossroad when roadRotation == 180:
                SpawnPoints(simpleX, 0, xPlusWidthX, simpleZ,
                    true, xPlusWidthX, -simpleZ);
                break;
            case RoadPoints.RoadType.Crossroad when roadRotation == 270:
                SpawnPoints(0, -simpleZ, simpleX, zMinusLengthZ,
                    true, -simpleX, zMinusLengthZ);
                break;
            //PORTAL---------------------------------------------------------------------------------------------
            case  RoadPoints.RoadType.Portal when roadRotation == 0:
                SpawnPoints(-simpleX, 0, simpleX, 0);
                break;
            case  RoadPoints.RoadType.Portal when roadRotation == 90:
                SpawnPoints(0, simpleZ, 0, -simpleZ);
                break;
            case  RoadPoints.RoadType.Portal when roadRotation == 180:
                SpawnPoints(simpleX, 0, -simpleX, 0);
                break;
            case  RoadPoints.RoadType.Portal when roadRotation == 270:
                SpawnPoints(0, -simpleZ, 0, simpleZ);
                break;
        }

        foreach (var curve in _road.CurvePoints)
        {
            Handles.color = Color.cyan;
            Handles.DrawSolidDisc(curve, Vector3.up, 0.2f);
        }

        foreach (var curve in _road.CurvePointsCross)
        {
            Handles.color = Color.yellow;
            Handles.DrawSolidDisc(curve, Vector3.up, 0.2f);
        }
    }

    private void SpawnPoints(float xOffsetStart, float zOffsetStart, float xOffsetEnd, float zOffsetEnd,
        bool isCrossroad = false, float xOffsetLeft = 0, float zOffsetLeft = 0)
    {
        _assetStartEditor = new Vector3(_bounds.center.x + xOffsetStart, _bounds.center.y - _bounds.size.y / 2,
            _bounds.center.z + zOffsetStart);
        _assetEnd = new Vector3(_bounds.center.x + xOffsetEnd, _bounds.center.y - _bounds.size.y / 2,
            _bounds.center.z + zOffsetEnd);

        if (isCrossroad)
        {
            _assetLeft = new Vector3(_bounds.center.x + xOffsetLeft, _bounds.center.y - _bounds.size.y / 2,
                _bounds.center.z + zOffsetLeft);
            Handles.color = new Color(1, 0.6f, 0);
            Handles.DrawSolidDisc(_assetLeft, Vector3.up, 0.2f);
        }

        Handles.color = Color.green;
        Handles.DrawSolidDisc(_assetStartEditor, Vector3.up, 0.2f);
        Handles.color = Color.red;
        Handles.DrawSolidDisc(_assetEnd, Vector3.up, 0.2f);
    }
}