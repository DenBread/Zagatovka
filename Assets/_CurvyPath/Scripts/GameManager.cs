using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.SceneManagement;

namespace CurvyPath
{
    public enum GameState
    {
        Prepare,
        Playing,
        Paused,
        PreGameOver,
        GameOver
    }

    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        public static event System.Action<GameState, GameState> GameStateChanged;

        private static bool isRestart;

        public GameState GameState
        {
            get
            {
                return _gameState;
            }
            private set
            {
                if (value != _gameState)
                {
                    GameState oldState = _gameState;
                    _gameState = value;

                    if (GameStateChanged != null)
                        GameStateChanged(_gameState, oldState);
                }
            }
        }

        public static int GameCount
        {
            get { return _gameCount; }
            private set { _gameCount = value; }
        }

        private static int _gameCount = 0;

        [Header("Set the target frame rate for this game")]
        [Tooltip("Use 60 for games requiring smooth quick motion, set -1 to use platform default frame rate")]
        public int targetFrameRate = 30;

        [Header("Current game state")]
        [SerializeField]
        private GameState _gameState = GameState.Prepare;

        // List of public variable for gameplay tweaking
        [Header("Gameplay Config")]

        [Range(0f, 1f)]
        public float coinFrequency = 0.1f;

        public float width = 50;

        public float spacing = 12;

        public float speed = 200;

        public float lenght = 25;

        public float timeCreateBall = 0.5f;

        public int limitPlane = 60;

        public float swipeForce = 2000;

        public float increaseSpeedRatio = 0.05f;

        public float limitSpeed = 300;

        public Color planeFirstColor = Color.white;

        public Color planeSecondColor = Color.gray;

        public Color color1 = Color.yellow;

        public Color color2 = Color.green;

        public Color color3 = Color.red;

        // List of public variables referencing other objects
        [Header("Object References")]
        public PlayerController playerController;

        [SerializeField]
        private Material materialObjectColor1;

        [SerializeField]
        private Material materialObjectColor2;

        [SerializeField]
        private Material materialObjectColor3;


        [SerializeField]
        private Material materialChangeColor1;

        [SerializeField]
        private Material materialChangeColor2;

        [SerializeField]
        private Material materialChangeColor3;

        void OnEnable()
        {
            PlayerController.PlayerDied += PlayerController_PlayerDied;
        }

        void OnDisable()
        {
            PlayerController.PlayerDied -= PlayerController_PlayerDied;
        }

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                DestroyImmediate(Instance.gameObject);
                Instance = this;
            }
            materialObjectColor1.SetColor("_Color", color1);
            materialObjectColor2.SetColor("_Color", color2);
            materialObjectColor3.SetColor("_Color", color3);

            materialChangeColor1.SetColor("_Color", color1);
            materialChangeColor2.SetColor("_Color", color2);
            materialChangeColor3.SetColor("_Color", color3);
        }

        void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
            }
        }

        // Use this for initialization
        void Start()
        {
            // Initial setup
            Application.targetFrameRate = targetFrameRate;
            ScoreManager.Instance.Reset();

            PrepareGame();
        }


        // Listens to the event when player dies and call GameOver
        void PlayerController_PlayerDied()
        {
            GameOver();
        }

        // Make initial setup and preparations before the game can be played
        public void PrepareGame()
        {
            GameState = GameState.Prepare;

            // Automatically start the game if this is a restart.
            if (isRestart)
            {
                isRestart = false;
                StartGame();
            }
        }

        // A new game official starts
        public void StartGame()
        {
            GameState = GameState.Playing;
            if (SoundManager.Instance.background != null)
            {
                SoundManager.Instance.PlayMusic(SoundManager.Instance.background);
            }
        }

        // Called when the player died
        public void GameOver()
        {
            if (SoundManager.Instance.background != null)
            {
                SoundManager.Instance.StopMusic();
            }

            SoundManager.Instance.PlaySound(SoundManager.Instance.gameOver);
            GameState = GameState.GameOver;
            GameCount++;

            // Add other game over actions here if necessary
        }

        // Start a new game
        public void RestartGame(float delay = 0)
        {
            isRestart = true;
            StartCoroutine(CRRestartGame(delay));
        }

        IEnumerator CRRestartGame(float delay = 0)
        {
            yield return new WaitForSeconds(delay);
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        public void HidePlayer()
        {
            if (playerController != null)
                playerController.gameObject.SetActive(false);
        }

        public void ShowPlayer()
        {
            if (playerController != null)
                playerController.gameObject.SetActive(true);
        }
    }
}