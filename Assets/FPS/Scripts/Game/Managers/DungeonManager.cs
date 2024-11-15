using UnityEngine;

namespace Unity.FPS.Game
{
    [System.Serializable]
    public class DungeonManager : MonoBehaviour
    {
        public GameObject startSectionPrefab;       // Prefab for starting section
        public GameObject straightSectionPrefab;  // Prefab for straight section
        public GameObject leftTurnSectionPrefab;        // Prefab for left turn section
        public GameObject rightTurnSectionPrefab;      // Prefab for right turn section
        public GameObject Player;                  // Player transform to track movement
        public int maxGeneratedSections = 1;     // Number of sections to generate ahead

        public Material floorMaterial; // floor texture
        public Material wallMaterial; // floor texture


        private int sectionLength = 18;            // Length of each section
        private Vector3 lastPosition;
        private Vector3 currentDirection = Vector3.forward;
        private int sectionsGenerated = 0; 
        private int currentTurns = 0;               // Current number of turns, used to indicate direction
        private bool startSection = true;

        void Start()
        {
            lastPosition = new Vector3(-35, 0, -10); // Starting point of the dungeon
            GenerateSection();
        }

        void Update()
        {
            // Generate new sections as the player approaches the end of the current section
            // if (Vector3.Distance(Player.position, lastPosition) < sectionLength * 2)
            // {
            //     GenerateSection();
            // }
            GenerateSection();
        }

        void GenerateSection()
        {
            if (sectionsGenerated >= maxGeneratedSections) return;

            // Randomly decide if the next section will be a straight section or a turn
            float randomValue = Random.Range(0f, 1f);
            GameObject newSection;

            if (startSection == true) {
                Debug.Log("Start Position: " + lastPosition);
                newSection = Instantiate(startSectionPrefab, lastPosition, Quaternion.LookRotation(currentDirection));
                lastPosition += currentDirection * sectionLength; // Move to the next section position

                startSection = false;
                sectionsGenerated++;
                return;
            }
            
            if (randomValue < 0.7f) // 55% chance for a straight section
            {
                Debug.Log("Straight Position: " + lastPosition);
                newSection = Instantiate(straightSectionPrefab, lastPosition, Quaternion.LookRotation(currentDirection));
            }
            else                    // 30% chance for a 90-degree turn
            {
                // if (Random.value < 0.5f)
                // {
                //     Debug.Log("Left Position: " + lastPosition);
                //     newSection = Instantiate(leftTurnSectionPrefab, lastPosition, Quaternion.LookRotation(currentDirection));
                //     currentDirection = Quaternion.Euler(0, -90, 0) * currentDirection;  // Left turn
                // }
                // else
                // {
                //     Debug.Log("Right Position: " + lastPosition);
                //     newSection = Instantiate(rightTurnSectionPrefab, lastPosition, Quaternion.LookRotation(currentDirection));
                //     currentDirection = Quaternion.Euler(0, 90, 0) * currentDirection;   // Right turn
                // }

                if (Random.value < 0.5f)
                {
                    if (currentTurns >= 2) 
                    {
                        newSection = Instantiate(rightTurnSectionPrefab, lastPosition, Quaternion.LookRotation(currentDirection));
                        currentDirection = Quaternion.Euler(0, 90, 0) * currentDirection;   // Right turn

                        currentTurns--;
                    }
                    else 
                    {
                        newSection = Instantiate(leftTurnSectionPrefab, lastPosition, Quaternion.LookRotation(currentDirection));
                        currentDirection = Quaternion.Euler(0, -90, 0) * currentDirection;  // Left turn

                        currentTurns++;
                    }
                }
                else
                {
                    if (currentTurns <= -2)
                    {
                        newSection = Instantiate(leftTurnSectionPrefab, lastPosition, Quaternion.LookRotation(currentDirection));
                        currentDirection = Quaternion.Euler(0, -90, 0) * currentDirection;  // Left turn

                        currentTurns++;
                    }
                    else
                    {
                        newSection = Instantiate(rightTurnSectionPrefab, lastPosition, Quaternion.LookRotation(currentDirection));
                        currentDirection = Quaternion.Euler(0, 90, 0) * currentDirection;   // Right turn

                        currentTurns--;
                    }
                    
                }
            }

            //code for rendering the floor and wall textures
            Renderer[] renderers = newSection.GetComponentsInChildren<Renderer>();
            foreach (Renderer renderer in renderers)
            {
                if (renderer.gameObject.name.Contains("Floor")) // Assumes the floor objects are named accordingly
                {
                    renderer.material = floorMaterial;
                }
                else if (renderer.gameObject.name.Contains("Wall")) // Assumes the wall objects are named accordingly
                {
                    renderer.material = wallMaterial;
                }
            }

            lastPosition += currentDirection * sectionLength;

            sectionsGenerated++;
        }
    }
}