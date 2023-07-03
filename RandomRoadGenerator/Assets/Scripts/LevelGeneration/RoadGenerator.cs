using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace LevelGeneration
{
    /// <summary>
    /// Random road generator.
    /// </summary>
    public class RoadGenerator : MonoBehaviour
    {
        //SETTINGS-------------------------------------------------------------------------------------------------------
        [Header("----SETTINGS----")]
        [Tooltip(
             "Time before a roadPiece actually gets destroyed then the player leaves itself," +
             "it should be high enough that the player doesn't see it but low enough that you dont get pieces colliding with each other"),
         SerializeField]
        private float destructionTimer;

        [Tooltip(
            "How many pieces the generator will keep generated at once.\n" +
            "I recommend number smaller than 4 to prevent overlapping.")]
        [SerializeField]
        private int piecesAtOnce = 3;

        [Tooltip("After how many road pieces the crossroad should be generated")] [SerializeField]
        private int whenToSpawnCross = 6;

        [Tooltip("The size of the whole generator area")] [SerializeField]
        private int borderSize = 100;

        [Tooltip("Player necessary to show the working generator")] [SerializeField]
        private Player player;

        //ROADS---------------------------------------------------------------------------------------------------------
        [Header("----ROADS----")]
        [Tooltip("Pieces of roads specific to the generator / area.\n" +
                 "Remember to follow the template of how to add them to the scene.")]
        [SerializeField]
        private List<RoadPoints> roadPieces;


        //NOT SHOWN IN THE INSPECTOR
        private readonly List<RoadPoints> _activePieces = new List<RoadPoints>();

        private Vector3 _startPosition;
        private RoadPoints _activePiece;
        private RoadPoints _rightSpawn;
        private RoadPoints _leftSpawn;
        private RoadPoints _activePoints;
        private GameObject _portalRoad;
        private int _generation;
        private int _dangerZone;
        private int _straightMark;
        private int _leftMark;
        private int _rightMark;
        private bool _closeToEdge;
        private bool _crossRoadGenerated;
        private bool _generateNewPiece;
        private bool _isActive;
        private bool _clear;
        private bool _clockwise;
        private int _straightRoads;
        private int _rightRoads;
        private int _leftRoads;
        private int _crossRoads;

        //DEBUG -----------------------
        public bool ShowBorder { get; set; }
        public int BorderSize => borderSize;
        public int NormalRoadBorderSpace { get; set; } = 3;
        public int CrossRoadBorderSpace { get; set; } = 5;

        public List<RoadPoints> RoadPieces => roadPieces;

        [HideInInspector] public float DefaultRotationX;
        [HideInInspector] public float DefaultRotationY;
        [HideInInspector] public float DefaultRotationZ;
        [HideInInspector] public float StartRotationY;


        private void Start()
        {
            _isActive = true;
            _activePiece = CreateNewActivePiece(NewPieceRotation(),
                transform.position);
            _activePoints = _activePiece;
        
            foreach (RoadPoints piece in roadPieces)
            {
                switch (piece.TypeOfRoad)
                {
                    case RoadPoints.RoadType.Straight:
                        _straightRoads++;
                        break;
                    case RoadPoints.RoadType.Left:
                        _leftRoads++;
                        break;
                    case RoadPoints.RoadType.Right:
                        _rightRoads++;
                        break;
                    case RoadPoints.RoadType.Crossroad:
                        _crossRoads++;
                        break;
                    default:
                        break;
                }
            }
        
            _straightMark = _straightRoads-1;
            _leftMark = _straightMark + _leftRoads;
            _rightMark = _leftMark + _rightRoads;
        }

        private void Update()
        {
            if (_isActive)
            {
                if (_generateNewPiece)
                {
                    GenerateStartRoads();
                    _generateNewPiece = false;
                }

                if (_activePieces.Count <= piecesAtOnce && !_crossRoadGenerated)
                {
                    GenerateRoad();
                }

                if (_crossRoadGenerated)
                {
                    GenerateRoadsAfterCrossRoad();
                }

                RemoveRoad();
            }

            if (_clear && !_isActive)
            {
                ClearGenerator();
            }
        }

        private void ClearGenerator()
        {
            _clear = false;
            foreach (RoadPoints piece in _activePieces)
            {
                Destroy(piece.gameObject);
            }

            _activePieces.Clear();
            _generateNewPiece = true;
            _startPosition = transform.localPosition;
            _activePiece = CreateNewActivePiece(NewPieceRotation(),
                transform.position);
            _activePoints = _activePiece;
        }


        /// <summary>
        /// Makes sure to generate roads correctly after the crossroads.
        /// Checks which road got chosen by the player and acts accordingly.
        /// </summary>
        private void GenerateRoadsAfterCrossRoad()
        {
            if (player.CurrentRoad == _leftSpawn.gameObject)
            {
                ChooseRoad(_leftSpawn, _rightSpawn);
                _clockwise = false;
            }
            else if (player.CurrentRoad == _rightSpawn.gameObject)
            {
                ChooseRoad(_rightSpawn, _leftSpawn);
                _clockwise = true;
            }
        }

        /// <summary>
        /// Checks which direction player chose, sets this direction as the latest road and removes roads that are on the opposite side
        /// </summary>
        /// <param name="roadToLeave">Road that player chose and is gonna leave soon</param>
        /// <param name="roadToRemove">Road that player hasn't chosen and is gonna be removed</param>
        private void ChooseRoad(RoadPoints roadToLeave, RoadPoints roadToRemove)
        {
            _activePiece = roadToLeave;
            _activePoints = roadToLeave;
            Destroy(roadToRemove.gameObject);
            _activePieces.Remove(roadToRemove);
            _crossRoadGenerated = false;
        }

        /// <summary>
        /// Generates the set of start roads
        /// </summary>
        private void GenerateStartRoads()
        {
            for (int i = 0; i < piecesAtOnce - 1; i++)
            {
                GenerateRoad();
            }
        }

        /// <summary>
        /// Removes the oldest road from the scene and from the list
        /// </summary>
        private void RemoveRoad()
        {
            if (player.CurrentRoad == _activePieces[1].gameObject)
            {
                StartCoroutine(_activePieces[0].DestroyMe(destructionTimer));
                _activePieces.RemoveAt(0);
            }
        }

        /// <summary>
        /// Generates road piece depends on the previous ones rotation.
        /// </summary>
        private void GenerateRoad()
        {
            _generation++;
            _startPosition = _activePoints.AssetEnd;
            int randomRoad = Random.Range(0, roadPieces.Count - _crossRoads);
            bool isACrossroad = false;
            float yDirection = 0;
            float pieceYRotation = _activePiece.transform.eulerAngles.y;

            switch (_activePoints.TypeOfRoad)
            {
                case RoadPoints.RoadType.Right:
                    yDirection = 90 + pieceYRotation;
                    CheckBorders(pieceYRotation);
                    break;
                case RoadPoints.RoadType.Left:
                    yDirection = -90 + pieceYRotation;
                    CheckBorders(pieceYRotation);
                    break;
                case RoadPoints.RoadType.Crossroad:
                    isACrossroad = true;
                    break;
                case RoadPoints.RoadType.Straight:
                    yDirection = 0 + pieceYRotation;
                    CheckBorders(pieceYRotation);
                    break;
            }

            if (isACrossroad)
            {
                GenerateCrossRoad();
            }
            else if (_closeToEdge)
            {
                randomRoad = !_clockwise ? 
                    Random.Range(_straightMark + 1, _leftMark + 1) :
                    Random.Range(_leftMark + 1, _rightMark + 1);
            
                _activePiece = CreateNewActivePiece(NewPieceRotation(yDirection), _startPosition, randomRoad);
                _activePoints = _activePieces[^1];
                _closeToEdge = false;
            }
            else
            {
                if (_generation % whenToSpawnCross == 0 && _crossRoads != 0)
                {
                    if (BordersCondition(((int)pieceYRotation + 360) % 360, CrossRoadBorderSpace))
                    {
                        _generation--;
                    }
                    else
                    {
                        randomRoad = Random.Range(roadPieces.Count - _crossRoads, roadPieces.Count);
                    }
                }
                else if (_generation % 2 == 0)
                {
                    randomRoad = Random.Range(0, _straightMark + 1);
                }
            
                _activePiece = CreateNewActivePiece(NewPieceRotation(yDirection), _startPosition, randomRoad);
                _activePoints = _activePieces[^1];
            }
        }

        /// <summary>
        /// Checks if the piece fits in the borders.
        /// Makes it possible to specify how far it should stop generating from the border line.
        /// </summary>
        /// <param name="roadRotationNonNegativeInt"> Road rotation passed as a non-negative int between 0 and 359</param>
        /// <param name="spaceFromTheBorders">  How big should be the space between the checked piece and border</param>
        /// <returns> True if at least one condition is met, false if road is not close to the border</returns>
        private bool BordersCondition(int roadRotationNonNegativeInt, int spaceFromTheBorders = 3)
        {
            //For our project I calculated that the space needs to be 3
            Vector3 activePiecePosition = _activePiece.transform.position;
            Vector3 generatorPosition = transform.position;
            float pieceHeight = _activePoints.RoadHeight;
            float pieceWidth = _activePoints.RoadWidth;
            float borderCorrected = borderSize / 2.0f;

            return (activePiecePosition.x + pieceHeight * spaceFromTheBorders >=
                       generatorPosition.x + borderCorrected && roadRotationNonNegativeInt == 0)
                   || (activePiecePosition.x - pieceHeight * spaceFromTheBorders <=
                       generatorPosition.x - borderCorrected && roadRotationNonNegativeInt == 180)
                   || (activePiecePosition.z + pieceWidth * spaceFromTheBorders >=
                       generatorPosition.z + borderCorrected && roadRotationNonNegativeInt == 270)
                   || (activePiecePosition.z - pieceWidth * spaceFromTheBorders <=
                       generatorPosition.z - borderCorrected && roadRotationNonNegativeInt == 90);
        }

        /// <summary>
        /// If the piece wouldn't fit the borders, marks it as close to the edge
        /// </summary>
        /// <param name="pieceYRotation"> Rotation of the current piece</param>
        private void CheckBorders(float pieceYRotation)
        {
            if (BordersCondition(((int)pieceYRotation + 360) % 360, NormalRoadBorderSpace))
            {
                _closeToEdge = true;
                // Debug.DrawLine(_activePiece.transform.position, 
                //     new Vector3(_activePiece.transform.position.x + _activePoints.Height * 3, _activePiece.transform.position.y, _activePiece.transform.position.z), 
                //     Color.red, 20.0f);
                // Debug.DrawLine(_activePiece.transform.position, 
                //     new Vector3(_activePiece.transform.position.x - _activePoints.Height * 3, _activePiece.transform.position.y, _activePiece.transform.position.z), 
                //     Color.red, 20.0f);
                // Debug.DrawLine(_activePiece.transform.position, 
                //     new Vector3(_activePiece.transform.position.x, _activePiece.transform.position.y, _activePiece.transform.position.z + _activePoints.RoadWidth * 3), 
                //     Color.red, 20.0f);
                // Debug.DrawLine(_activePiece.transform.position, 
                //     new Vector3(_activePiece.transform.position.x, _activePiece.transform.position.y, _activePiece.transform.position.z - _activePoints.RoadWidth * 3), 
                //     Color.red, 20.0f);
            }
        }

        /// <summary>
        /// Generated roads on the both ends of the crossroad. The roads are ALWAYS straight
        /// </summary>
        private void GenerateCrossRoad()
        {
            _crossRoadGenerated = true;
            float pieceYRotation = _activePiece.transform.eulerAngles.y;

            float yDirection = 90 + pieceYRotation;
            float yDirectionT = -90 + pieceYRotation;
        
            Vector3 rightPosition = _activePoints.AssetEnd;
            Vector3 leftPosition = _activePoints.AssetLeft;

            _leftSpawn = CreateNewActivePiece(NewPieceRotation(yDirectionT), leftPosition);
            _rightSpawn = CreateNewActivePiece(NewPieceRotation(yDirection), rightPosition);
        }

        /// <summary>
        /// Creates new road piece and adds it to the list of active (present in the scene) pieces
        /// </summary>
        /// <param name="rotation">Rotation of the piece, important to make sure that the road is going correctly</param>
        /// <param name="startPosition">Start position of the piece</param>
        /// <param name="roadPieceNumber">Number deciding which piece of road should be generated</param>
        /// <returns> New GameObject roadPiece (used to assign it to activePiece or left/right Spawn)</returns>
        private RoadPoints CreateNewActivePiece(Quaternion rotation, Vector3 startPosition, int roadPieceNumber = 0)
        {
            GameObject newPiece = Instantiate(roadPieces[roadPieceNumber].gameObject, startPosition, rotation);
            RoadPoints newRoadPoints = null;
            if (newPiece.TryGetComponent(out RoadPoints roadPoints))
            {
                newRoadPoints = roadPoints;
            }
            if (roadPieceNumber > _straightMark && roadPieceNumber <= _leftMark) _clockwise = false;
            if (roadPieceNumber > _leftMark && roadPieceNumber <= _rightMark) _clockwise = true;
            newPiece.transform.parent = transform;
            _activePieces.Add(newRoadPoints);
            return newRoadPoints;
        }

        private Quaternion NewPieceRotation(float rotationY=0)
        {
            Quaternion rotation = Quaternion.Euler(DefaultRotationX,DefaultRotationY + rotationY,DefaultRotationZ);
            return rotation;
        }
    }
}