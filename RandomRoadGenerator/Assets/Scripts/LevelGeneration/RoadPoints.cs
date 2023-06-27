using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Renderer), typeof(BoxCollider))]
public class RoadPoints : MonoBehaviour
{
    public enum RoadType
    {
        Straight = 1,
        Right = 2,
        Left = 3,
        Crossroad = 4
    };

    [SerializeField] private RoadType roadType;
    [SerializeField] private int width = 8;
    [SerializeField] private int length = 8;
    [SerializeField] private int curvePoints = 20;
    [SerializeField] private float specialOffset;

    public int Width => width;
    public int Length => length;
    public RoadType TypeOfRoad => roadType;

    [HideInInspector] public List<Vector3> CurvePoints = new List<Vector3>();
    [HideInInspector] public List<Vector3> CurvePointsCross = new List<Vector3>();

    private Vector3 _assetStart;
    private Vector3 _assetEnd;
    private Vector3 _assetLeft;
    private Vector3 _creatureSpawn;

    public Vector3 CreatureSpawn
    {
        get => _creatureSpawn;
        set => _creatureSpawn = value;
    }

    [HideInInspector]public Vector3 RoadCenter;

    private Vector3 _helperVector;
    private Bounds _bounds;
    private bool _curve;
    private bool _doubleCurve;
    private float _roadHeight;
    private float _roadWidth;

    public float RoadHeight
    {
        get => _roadHeight;
        set => _roadHeight = value;
    }

    public float RoadWidth
    {
        get => _roadWidth;
        set => _roadWidth = value;
    }

    //Used in the editor
    public Vector3 AssetStart => _assetStart;

    public Vector3 AssetEnd => _assetEnd;

    public Vector3 AssetLeft => _assetLeft;

    public float SpecialOffset => specialOffset;


    private void Awake()
    {
        _bounds = GetComponent<Renderer>().bounds;
        _roadHeight = _bounds.size.x;
        _roadWidth = _bounds.size.z;
    }

    private void OnEnable()
    {
        GeneratePoints();

        if (_curve)
        {
            GenerateCurve(ref CurvePoints, curvePoints, _assetStart, _helperVector, _assetEnd);
        }

        if (_doubleCurve)
        {
            GenerateCurve(ref CurvePointsCross, curvePoints, _assetStart, _helperVector, _assetLeft);
        }
    }

    /// <summary>
    /// Generates bezier curve to help our player with movement
    /// </summary>
    /// <param name="curves">           List of curves that will get the necessary points (CurvePointsCross if it's a crossroad)</param>
    /// <param name="numberOfPoints">   How many points you want to generate. The more the points, the smoother the curve</param>
    /// <param name="startPoint">       Start point of the curve</param>
    /// <param name="helperPoint">      Helper point needed to generate the curve</param>
    /// <param name="endPoint">         End point of the curve</param>
    private void GenerateCurve(ref List<Vector3> curves, int numberOfPoints, Vector3 startPoint, Vector3 helperPoint,
        Vector3 endPoint)
    {
        for (int i = 0; i <= numberOfPoints; i++)
        {
            float t = i / (float)numberOfPoints;
            curves.Add(CalculateBezierPoint(t, startPoint, helperPoint, endPoint));
        }
    }

    ///<summary>
    ///Destroy after t has passed and then spawn a visual effect
    ///</summary>
    public IEnumerator DestroyMe(float t)
    {
        yield return new WaitForSeconds(t);
        Destroy(gameObject);
        yield break;
    }

    /// <summary>
    /// Method used to generate points on correct positions
    /// </summary>
    /// <param name="xOffsetStart"> Offset of X position of the start point</param>
    /// <param name="zOffsetStart"> Offset of Z position of the start point</param>
    /// <param name="xOffsetEnd">   Offset of X position of the end point</param>
    /// <param name="zOffsetEnd">   Offset of Z position of the end point</param>
    /// <param name="curveOffsetX">  Offset of X position of the left point</param>
    /// <param name="curveOffsetZ">  Offset of Z position of the left point</param>
    /// <param name="spawnOneCurve"></param>
    /// <param name="spawnTwoCurves"></param>
    /// <param name="xOffsetLeft"></param>
    /// <param name="zOffsetLeft"></param>
    private void SpawnPoints(float xOffsetStart, float zOffsetStart, float xOffsetEnd, float zOffsetEnd,
        float curveOffsetX = 0, float curveOffsetZ = 0, bool spawnOneCurve = false,
        bool spawnTwoCurves = false, float xOffsetLeft = 0, float zOffsetLeft = 0)
    {
        float height = transform.position.y;
        _assetStart = new Vector3(_bounds.center.x + xOffsetStart, height,
            _bounds.center.z + zOffsetStart);
        _assetEnd = new Vector3(_bounds.center.x + xOffsetEnd, height,
            _bounds.center.z + zOffsetEnd);
        
        CreatureSpawn = new Vector3(_bounds.center.x + curveOffsetX, height,
            _bounds.center.z + curveOffsetZ);
        RoadCenter = new Vector3(_bounds.center.x, height, _bounds.center.z);

        if (spawnOneCurve)
        {
            _helperVector = new Vector3(_bounds.center.x + curveOffsetX, height,
                _bounds.center.z + curveOffsetZ);
            _curve = true;
        }

        if (spawnTwoCurves)
        {
            _helperVector = new Vector3(_bounds.center.x + curveOffsetX, height,
                _bounds.center.z + curveOffsetZ);
            _assetLeft = new Vector3(_bounds.center.x + xOffsetLeft, height,
                _bounds.center.z + zOffsetLeft);
            _doubleCurve = true;
            _curve = true;
        }
    }

    /// <summary>
    /// Method used to calculate all the points' positions
    /// </summary>
    private void GeneratePoints()
    {
        int roadRotation = ((int)transform.localRotation.eulerAngles.y + 360) % 360;

        float simpleX = _bounds.size.x / 2;
        float simpleZ = _bounds.size.z / 2;
        float lengthZ = _bounds.size.z / (2 * length);
        float widthX = _bounds.size.x / (2 * width);
        float xMinusWidthX = simpleX - widthX;
        float xPlusWidthX = -simpleX + widthX;
        float zMinusLengthZ = simpleZ - lengthZ;
        float zPlusLengthZ = -simpleZ + lengthZ;
        switch (roadType)
        {
            case RoadType.Straight when roadRotation == 0:
                SpawnPoints(-simpleX, 0, simpleX, 0);
                break;
            case RoadType.Straight when roadRotation == 90:
                SpawnPoints(0, simpleZ, 0, -simpleZ);
                break;
            case RoadType.Straight when roadRotation == 180:
                SpawnPoints(simpleX, 0, -simpleX, 0);
                break;
            case RoadType.Straight when roadRotation == 270:
                SpawnPoints(0, -simpleZ, 0, simpleZ);
                break;
            //RIGHT-------------------------------------------------------------------------------------------------
            case RoadType.Right when roadRotation == 0:
                SpawnPoints(-simpleX, zMinusLengthZ - specialOffset, xMinusWidthX - specialOffset, -simpleZ,
                     xMinusWidthX - specialOffset, zMinusLengthZ - specialOffset, true);
                break;
            case RoadType.Right when roadRotation == 90:
                SpawnPoints(xMinusWidthX - specialOffset, simpleZ, -simpleX, zPlusLengthZ + specialOffset,
                     xMinusWidthX - specialOffset, zPlusLengthZ + specialOffset, true);
                break;
            case RoadType.Right when roadRotation == 180:
                SpawnPoints(simpleX, zPlusLengthZ + specialOffset, xPlusWidthX + specialOffset, simpleZ,
                     xPlusWidthX + specialOffset, zPlusLengthZ + specialOffset, true);
                break;
            case RoadType.Right when roadRotation == 270:
                SpawnPoints(xPlusWidthX + specialOffset, -simpleZ, simpleX, zMinusLengthZ - specialOffset,
                     xPlusWidthX + specialOffset, zMinusLengthZ - specialOffset, true);
                break;
            //LEFT--------------------------------------------------------------------------------------------------
            case RoadType.Left when roadRotation == 0:
                SpawnPoints(-simpleX, zPlusLengthZ + specialOffset, xMinusWidthX - specialOffset, simpleZ,
                     xMinusWidthX - specialOffset, zPlusLengthZ + specialOffset, true);
                break;
            case RoadType.Left when roadRotation == 90:
                SpawnPoints(xPlusWidthX + specialOffset, simpleZ, simpleX, zPlusLengthZ + specialOffset,
                     xPlusWidthX + specialOffset, zPlusLengthZ + specialOffset, true);
                break;
            case RoadType.Left when roadRotation == 180:
                SpawnPoints(simpleX, zMinusLengthZ - specialOffset, xPlusWidthX + specialOffset, -simpleZ,
                     xPlusWidthX + specialOffset, zMinusLengthZ - specialOffset, true);
                break;
            case RoadType.Left when roadRotation == 270:
                SpawnPoints(xMinusWidthX - specialOffset, -simpleZ, -simpleX, zMinusLengthZ - specialOffset,
                     xMinusWidthX - specialOffset, zMinusLengthZ - specialOffset, true);
                break;
            //CROSSROAD---------------------------------------------------------------------------------------------
            case RoadType.Crossroad when roadRotation == 0:
                SpawnPoints(-simpleX, 0, xMinusWidthX, -simpleZ,
                     xMinusWidthX, 0, false, true,
                    xMinusWidthX,simpleZ);
                break;
            case RoadType.Crossroad when roadRotation == 90:
                SpawnPoints(0, simpleZ, -simpleX, zPlusLengthZ,
                     0, zPlusLengthZ, false, true,
                    simpleX, zPlusLengthZ);
                break;
            case RoadType.Crossroad when roadRotation == 180:
                SpawnPoints(simpleX, 0, xPlusWidthX, simpleZ,
                     xPlusWidthX, 0, false, true,
                    xPlusWidthX,-simpleX);
                break;
            case RoadType.Crossroad when roadRotation == 270:
                SpawnPoints(0, -simpleZ, simpleX, zMinusLengthZ,
                     0, zMinusLengthZ, false, true,
                    -simpleX, zMinusLengthZ);
                break;
        }
    }

    private Vector3 CalculateBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2)
    {
        float u = 1 - t;
        float tt = t * t;
        float uu = u * u;
        Vector3 point = uu * p0 + 2 * u * t * p1 + tt * p2;
        return point;
    }
}