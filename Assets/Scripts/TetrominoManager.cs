using UnityEngine;
using UnityEngine.Tilemaps;

public class TetrominoManager : MonoBehaviour
{
    public Tile TetominoTile;
    public bool AllowRotation = true;
    public bool LimitRotation;

    public AudioClip MoveSound;
    public AudioClip RotateSound;
    public AudioClip LandSound;

    private const float ContinuosVerticalSpeed = 0.05f;
    private const float ContinuosHorozontalSpeed = 0.1f;
    private const float DownButtonMaxWait = 0.2f;

    private int _fallBonusScore = 20;

    private float _verticalTimer;
    private float _horizontalTimer;

    private float _horizontalDownButtonWaitTimer;
    private float _verticalDownButtonWaitTimer;

    private bool _movedImmedialeHorozontal;
    private bool _movedImmedialeVertical;

    private float _fallTime;
    private float _individualScoreTime;

    /// <summary>
    ///  Update is called once per frame
    /// </summary>
    private void Update()
    {
        CheckUserInput();

        UpdateIndividualScore();
    }

    /// <summary>
    /// Update individual score
    /// </summary>
    private void UpdateIndividualScore()
    {
        if (_individualScoreTime < 1)
        {
            _individualScoreTime += Time.deltaTime;
        }
        else
        {
            _individualScoreTime = 0;
            _fallBonusScore = Mathf.Max(_fallBonusScore - _fallBonusScore / 10, 0);
        }
    }

    /// <summary>
    /// Check User input
    /// </summary>
    private void CheckUserInput()
    {
        var gameInstance = FindObjectOfType<Game>();

        if (Input.GetKeyUp(KeyCode.RightArrow) || Input.GetKeyUp(KeyCode.LeftArrow))
        {
            _movedImmedialeHorozontal = false;
            _horizontalTimer = 0;
            _horizontalDownButtonWaitTimer = 0;
        }

        if (Input.GetKeyUp(KeyCode.DownArrow))
        {
            _movedImmedialeVertical = false;
            _verticalTimer = 0;
            _verticalDownButtonWaitTimer = 0;
        }

        if (Input.GetKey(KeyCode.RightArrow))
        {
            MoveRight();
        }

        if (Input.GetKey(KeyCode.LeftArrow))
        {
            MoveLeft();
        }

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            Rotate();
        }

        if (Input.GetKey(KeyCode.DownArrow) || Time.time - _fallTime >= gameInstance.FallSpeed)
        {
            MoveDown();
        }
    }

    /// <summary>
    /// Move left
    /// </summary>
    private void MoveLeft()
    {
        if (_movedImmedialeHorozontal)
        {
            if (_horizontalDownButtonWaitTimer < DownButtonMaxWait)
            {
                _horizontalDownButtonWaitTimer += Time.deltaTime;
                return;
            }

            if (_horizontalTimer < ContinuosHorozontalSpeed)
            {
                _horizontalTimer += Time.deltaTime;
                return;
            }
        }
        else
        {
            _movedImmedialeHorozontal = true;
        }

        _horizontalTimer = 0;

        transform.position += new Vector3(-1, 0, 0);
        if (CheckIsValidPosition())
        {
            FindObjectOfType<Game>().PlaySound(MoveSound);
        }
        else
        {
            transform.position += new Vector3(1, 0, 0);
        }
    }

    /// <summary>
    /// Move right
    /// </summary>
    private void MoveRight()
    {
        if (_movedImmedialeHorozontal)
        {
            if (_horizontalDownButtonWaitTimer < DownButtonMaxWait)
            {
                _horizontalDownButtonWaitTimer += Time.deltaTime;
                return;
            }

            if (_horizontalTimer < ContinuosHorozontalSpeed)
            {
                _horizontalTimer += Time.deltaTime;
                return;
            }
        }
        else
        {
            _movedImmedialeHorozontal = true;
        }

        _horizontalTimer = 0;

        transform.position += new Vector3(1, 0, 0);
        if (CheckIsValidPosition())
        {
            FindObjectOfType<Game>().PlaySound(MoveSound);
        }
        else
        {
            transform.position += new Vector3(-1, 0, 0);
        }
    }

    /// <summary>
    /// Move down
    /// </summary>
    private void MoveDown()
    {
        var gameInstance = FindObjectOfType<Game>();
        if (_movedImmedialeVertical)
        {
            if (_verticalDownButtonWaitTimer < DownButtonMaxWait)
            {
                _verticalDownButtonWaitTimer += Time.deltaTime;
                return;
            }

            if (_verticalTimer < ContinuosVerticalSpeed)
            {
                _verticalTimer += Time.deltaTime;
                return;
            }
        }
        else
        {
            _movedImmedialeVertical = true;
        }

        _verticalTimer = 0;

        transform.position += new Vector3(0, -1, 0);
        if (CheckIsValidPosition())
        {
            if (Input.GetKey(KeyCode.DownArrow))
            {
                gameInstance.PlaySound(MoveSound);
            }
        }
        else
        {
            transform.position += new Vector3(0, 1, 0);

            if (GridManager.CheckIsAboveGrid(this))
            {
                Game.GameOver();
            }

            gameInstance.PlaySound(LandSound);
            gameInstance.SpawnNextTetromino();

            FindObjectOfType<ScoreManager>().TotalScore += _fallBonusScore;

            FindObjectOfType<GridManager>().UpdateTileMap(gameObject.transform, TetominoTile);
            FindObjectOfType<GridManager>().DeleteRow();

            Destroy(gameObject);
        }

        _fallTime = Time.time;
    }

    /// <summary>
    /// Rotate
    /// </summary>
    private void Rotate()
    {
        if (!AllowRotation) return;

        if (LimitRotation)
        {
            if (transform.rotation.eulerAngles.z >= 90)
            {
                transform.Rotate(0, 0, -90);
            }
            else
            {
                transform.Rotate(0, 0, 90);
            }
        }
        else
        {
            transform.Rotate(0, 0, 90);
        }

        if (CheckIsValidPosition())
        {
            FindObjectOfType<Game>().PlaySound(RotateSound);
        }
        else
        {
            if (LimitRotation)
            {
                if (transform.rotation.eulerAngles.z >= 90)
                {
                    transform.Rotate(0, 0, -90);
                }
                else
                {
                    transform.Rotate(0, 0, 90);
                }
            }
            else
            {
                transform.Rotate(0, 0, -90);
            }
        }
    }

    /// <summary>
    /// Check is valid position
    /// </summary>
    /// <returns></returns>
    private bool CheckIsValidPosition()
    {
        foreach (Transform mino in transform)
        {
            var pos = Game.RoundPosition(mino.position);
            if (!Game.CheckIsInsideGrid(pos))
            {
                return false;
            }


            var gridTile = FindObjectOfType<GridManager>().GetTransformAtGridPosition(pos);
            if (null != gridTile)
            {
                return false;
            }
        }

        return true;
    }
}