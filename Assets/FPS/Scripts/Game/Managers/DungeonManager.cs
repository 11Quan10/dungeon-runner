using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Unity.FPS.Game
{
    [System.Serializable]
    public class DungeonManager : MonoBehaviour
    {
        public GameObject startSectionPrefab;       // Prefab for starting section
        public GameObject straightSectionPrefab;  // Prefab for straight section
        public GameObject leftTurnSectionPrefab;        // Prefab for left turn section
        public GameObject rightTurnSectionPrefab;      // Prefab for right turn section
        public GameObject Player;
        public GameObject Turret;
        public GameObject HoverBot;

        int sectionLength = 18;            // Length of each section
        Vector3 lastPosition;
        Vector3 currentDirection = Vector3.forward;
        int currentTurns = 0;               // Current number of turns, used to indicate direction
        bool startSection = true;
        int straightSections = 0;
        Queue<GameObject> instantiatedSections;
        Queue<GameObject> enemies;

        [Header("Parameters")] [Tooltip("Duration of the fade-to-black at the end of the game")]
        public float EndSceneLoadDelay = 3f;

        [Tooltip("The canvas group of the fade-to-black screen")]
        public CanvasGroup EndGameFadeCanvasGroup;

        [Header("Win")] [Tooltip("This string has to be the name of the scene you want to load when winning")]
        public string WinSceneName = "WinScene";

        [Tooltip("Duration of delay before the fade-to-black, if winning")]
        public float DelayBeforeFadeToBlack = 4f;

        [Tooltip("Win game message")]
        public string WinGameMessage;
        [Tooltip("Duration of delay before the win message")]
        public float DelayBeforeWinMessage = 2f;

        [Tooltip("Sound played on win")] public AudioClip VictorySound;

        [Header("Lose")] [Tooltip("This string has to be the name of the scene you want to load when losing")]
        public string LoseSceneName = "LoseScene";

        public bool GameIsEnding { get; private set; }

        float m_TimeLoadEndGameScene;
        string m_SceneToLoad;

        void Awake() 
        {
            EventManager.AddListener<PlayerDeathEvent>(OnPlayerDeath);
        }

        void Start()
        {
            AudioUtility.SetMasterVolume(1);

            enemies = new Queue<GameObject>();
            instantiatedSections = new Queue<GameObject>();
            lastPosition = Player.transform.position;        // Starting point of the dungeon
            GenerateSection();
        }

        void Update()
        {
            if (GameIsEnding)
            {
                float timeRatio = 1 - (m_TimeLoadEndGameScene - Time.time) / EndSceneLoadDelay;
                EndGameFadeCanvasGroup.alpha = timeRatio;

                AudioUtility.SetMasterVolume(1 - timeRatio);

                // See if it's time to load the end scene (after the delay)
                if (Time.time >= m_TimeLoadEndGameScene)
                {
                    SceneManager.LoadScene(m_SceneToLoad);
                    GameIsEnding = false;
                }
            }

            // GenerateSection();
            if (instantiatedSections.Count < 7)
            {
                GenerateSection();
            }

            if (instantiatedSections.Count > 0)
            {
                GameObject oldSection = instantiatedSections.Peek();
                Vector3 groundedPosition = Player.transform.position; groundedPosition.y = 0;
                if (Vector3.Distance(oldSection.transform.position, groundedPosition) > sectionLength)
                {
                    Destroy(instantiatedSections.Dequeue());
                }
            }

            if (enemies.Count > instantiatedSections.Count)
            {
                Destroy(enemies.Dequeue());
            }
        }

        void OnPlayerDeath(PlayerDeathEvent evt) => EndGame(false);

        void GenerateSection()
        {

            // Randomly decide if the next section will be a straight section or a turn
            float randomValue = Random.Range(0f, 1f);
            GameObject newSection;

            if (startSection == true) {
                newSection = Instantiate(startSectionPrefab, lastPosition, Quaternion.LookRotation(currentDirection));

                startSection = false;
            }
            else if (straightSections < 2 || randomValue < 0.7f)     // 70% chance for a straight section   (and at least 2 straight sections after a turn)
            {
                newSection = Instantiate(straightSectionPrefab, lastPosition, Quaternion.LookRotation(currentDirection));
                straightSections++;
            }
            else                    // 30% chance for a 90-degree turn
            {
                straightSections = 0;
                if (Random.value < 0.5f)
                {
                    newSection = Instantiate(rightTurnSectionPrefab, lastPosition, Quaternion.LookRotation(currentDirection));
                    currentDirection = Quaternion.Euler(0, 90, 0) * currentDirection;                   // Right turn

                    currentTurns--;
                }
                else
                {
                    newSection = Instantiate(leftTurnSectionPrefab, lastPosition, Quaternion.LookRotation(currentDirection));
                    currentDirection = Quaternion.Euler(0, -90, 0) * currentDirection;                  // Left turn

                    currentTurns++;
                }
            }

            instantiatedSections.Enqueue(newSection);
            
            if (!startSection && randomValue < 0.5f) {           // 50% chance to spawn enemy
                Vector3 randomPosition = new Vector3(lastPosition.x + sectionLength * (0.5f - Random.Range(0f, 1f)),
                                                     lastPosition.y,
                                                     lastPosition.z + sectionLength * (0.5f - Random.Range(0f, 1f)));
                GameObject enemy = Instantiate(HoverBot, randomPosition, Quaternion.identity);
                enemies.Enqueue(enemy);
            }

            // if (instantiatedSections.Count > 0)
            // {
            //     GameObject oldSection = instantiatedSections.Peek();
            //     Vector3 groundedPosition = Player.transform.position; groundedPosition.y = 0;
            //     if (Vector3.Distance(oldSection.transform.position, groundedPosition) > 1.5 * sectionLength)
            //     {
            //         Destroy(instantiatedSections.Dequeue());
            //     }
            // }
            
            lastPosition += currentDirection * sectionLength;
        }

        void EndGame(bool win)
        {
            // unlocks the cursor before leaving the scene, to be able to click buttons
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            // Remember that we need to load the appropriate end scene after a delay
            GameIsEnding = true;
            EndGameFadeCanvasGroup.gameObject.SetActive(true);
            if (win)
            {
                m_SceneToLoad = WinSceneName;
                m_TimeLoadEndGameScene = Time.time + EndSceneLoadDelay + DelayBeforeFadeToBlack;

                // play a sound on win
                var audioSource = gameObject.AddComponent<AudioSource>();
                audioSource.clip = VictorySound;
                audioSource.playOnAwake = false;
                audioSource.outputAudioMixerGroup = AudioUtility.GetAudioGroup(AudioUtility.AudioGroups.HUDVictory);
                audioSource.PlayScheduled(AudioSettings.dspTime + DelayBeforeWinMessage);

                // create a game message
                //var message = Instantiate(WinGameMessagePrefab).GetComponent<DisplayMessage>();
                //if (message)
                //{
                //    message.delayBeforeShowing = delayBeforeWinMessage;
                //    message.GetComponent<Transform>().SetAsLastSibling();
                //}

                DisplayMessageEvent displayMessage = Events.DisplayMessageEvent;
                displayMessage.Message = WinGameMessage;
                displayMessage.DelayBeforeDisplay = DelayBeforeWinMessage;
                EventManager.Broadcast(displayMessage);
            }
            else
            {
                m_SceneToLoad = LoseSceneName;
                m_TimeLoadEndGameScene = Time.time + EndSceneLoadDelay;
            }
        }

        void OnDestroy()
        {
            EventManager.RemoveListener<PlayerDeathEvent>(OnPlayerDeath);
        }
    }
}