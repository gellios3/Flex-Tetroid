using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Game : MonoBehaviour
{
    public int CurrentLevel;

    public int LinesCleaned { get; set; }

    public static bool IsStartDefault;
    public static int StartingLevel;

    private float _fallSpeed = 1;

    private AudioSource _audioSource;

    private GameObject _previewTetromino;
    private GameObject _nextTetromino;

    private bool _gameStarted;

    private readonly Vector2 _previewPosition = new Vector2(-6.5f, 16);
    private readonly Vector2 _startPosition = new Vector2(5.0f, 20.0f);

    public AudioClip MecsikanetsSound;

    private int _key;

    private readonly KeyCode[] _cheatButtons =
    {
        KeyCode.UpArrow, KeyCode.UpArrow, KeyCode.DownArrow, KeyCode.DownArrow, KeyCode.LeftArrow, KeyCode.RightArrow,
        KeyCode.LeftArrow, KeyCode.RightArrow, KeyCode.B, KeyCode.A
    };

    private readonly KeyCode[] _pushButtons = new KeyCode[10];

    private readonly KeyCode[] _keyCodes =
    {
        KeyCode.UpArrow, KeyCode.DownArrow, KeyCode.LeftArrow, KeyCode.RightArrow, KeyCode.A, KeyCode.B
    };

    private const int LinesToNextLevel = 10;

    public float FallSpeed
    {
        get { return _fallSpeed; }
    }

    private void Start()
    {
        CurrentLevel = StartingLevel;
        _audioSource = GetComponent<AudioSource>();
        SpawnNextTetromino();
    }

    private void Update()
    {
        UpdateSpeed();
        UpdateLevel();

        if (IsKonamiCode())
        {
            CurrentLevel = 15;
            FindObjectOfType<ScoreManager>().TotalScore = 9999;
            FindObjectOfType<ScoreManager>().RowsCount = 9999;
            PlaySound(MecsikanetsSound);
            _pushButtons[0] = KeyCode.A;
        }

        if (Input.GetKey("escape"))
        {
            Application.Quit();
        }
    }

    private bool IsKonamiCode()
    {
        foreach (var keyKode in _keyCodes)
        {
            if (!Input.GetKeyDown(keyKode)) continue;

            _pushButtons[_key] = keyKode;

            _key = _key == 9 ? 0 : _key + 1;
        }

        return CompArr(_cheatButtons, _pushButtons);
    }

    private bool CompArr<T, S>(T[] arrayA, S[] arrayB)
    {
        if (arrayA.Length != arrayB.Length) return false;

        return !arrayA.Where((t, i) => !t.Equals(arrayB[i])).Any();
    }

    /// <summary>
    /// Sapan Next Tetromino
    /// </summary>
    public void SpawnNextTetromino()
    {
        if (!_gameStarted)
        {
            _gameStarted = true;

            _nextTetromino = (GameObject) Instantiate(
                Resources.Load(GetRandomTetroninoName(), typeof(GameObject)),
                _startPosition, Quaternion.identity
            );
            _nextTetromino.transform.parent = gameObject.transform;
            SpawnPreviewTetromino();
        }
        else
        {
            _previewTetromino.transform.localPosition = _startPosition;
            _nextTetromino = _previewTetromino;
            _nextTetromino.GetComponent<TetrominoManager>().enabled = true;
            SpawnPreviewTetromino();
        }
    }

    /// <summary>
    /// Check is inside grid
    /// </summary>
    /// <param name="pos"></param>
    /// <returns></returns>
    public static bool CheckIsInsideGrid(Vector2 pos)
    {
        return (int) pos.x >= 0 && (int) pos.x < GridManager.GridWidth && (int) pos.y >= 0;
    }

    /// <summary>
    /// Round position 
    /// </summary>
    /// <param name="pos"></param>
    /// <returns></returns>
    public static Vector2 RoundPosition(Vector2 pos)
    {
        return new Vector2(Mathf.Round(pos.x), Mathf.Round(pos.y));
    }

    /// <summary>
    /// Game over
    /// </summary>
    public static void GameOver()
    {
        SceneManager.LoadScene("GameOver");
    }

    /// <summary>
    /// Play sound
    /// </summary>
    /// <param name="sound"></param>
    public void PlaySound(AudioClip sound)
    {
        _audioSource.PlayOneShot(sound);
    }

    /// <summary>
    /// Update level
    /// </summary>
    private void UpdateLevel()
    {
        var tempLevel = StartingLevel + LinesCleaned / LinesToNextLevel;
        if (IsStartDefault || !IsStartDefault && tempLevel > StartingLevel)
        {
            CurrentLevel = tempLevel;
        }
    }

    /// <summary>
    /// Update Speed
    /// </summary>
    private void UpdateSpeed()
    {
        if (!(_fallSpeed > 0.05f)) return;
        if (CurrentLevel < 10)
        {
            _fallSpeed = 1.0f - CurrentLevel * 0.1f;
        }
        else
        {
            _fallSpeed = 0.1f - (CurrentLevel - 10) * 0.01f;
        }
    }

    /// <summary>
    /// Spawn preview teromino
    /// </summary>
    private void SpawnPreviewTetromino()
    {
        _previewTetromino = (GameObject) Instantiate(
            Resources.Load(GetRandomTetroninoName(), typeof(GameObject)),
            _previewPosition, Quaternion.identity
        );
        _previewTetromino.transform.parent = gameObject.transform;
        _previewTetromino.GetComponent<TetrominoManager>().enabled = false;
    }

    /// <summary>
    /// Get random teromino
    /// </summary>
    /// <returns></returns>
    private static string GetRandomTetroninoName()
    {
        var randomPos = Random.Range(0, 7);
        string[] randomTetromonoNames =
        {
            "Tetromino_j", "Tetromino_L", "Tetromino_Long",
            "Tetromino_S", "Tetromino_Square", "Tetromino_T", "Tetromino_Z"
        };

        return "Prefabs/" + randomTetromonoNames[randomPos];
    }
}