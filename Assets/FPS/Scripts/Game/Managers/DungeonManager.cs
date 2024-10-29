using UnityEngine;

namespace Unity.FPS.Game
{
    public class DungeonManager : MonoBehaviour
    {
        public GameObject straightSectionPrefab;  // Prefab for straight section
        public GameObject turnSectionPrefab;      // Prefab for corner section
        public GameObject Player;                  // Player transform to track movement
        public int sectionLength = 10;            // Length of each section
        public int maxGeneratedSections = 20;     // Number of sections to generate ahead

        private Vector3 lastPosition;
        private Vector3 currentDirection = Vector3.forward;
        private int sectionsGenerated = 0;

        void Start()
        {
            lastPosition = Vector3.zero; // Starting point of the dungeon
            GenerateSection();
        }

        void Update()
        {
            // Generate new sections as the player approaches the end of the current section
            // if (Vector3.Distance(Player.position, lastPosition) < sectionLength * 2)
            // {
            //     GenerateSection();
            // }
            for (int i = 0; i < 5; i++) 
            {
                GenerateSection();
            }
        }

        void GenerateSection()
        {
            if (sectionsGenerated >= maxGeneratedSections) return;

            // Randomly decide if the next section will be a straight section or a turn
            float randomValue = Random.Range(0f, 1f);
            GameObject newSection;

            if (randomValue < 0.7f) // 70% chance for a straight section
            {
                newSection = Instantiate(straightSectionPrefab, lastPosition, Quaternion.LookRotation(currentDirection));
                lastPosition += currentDirection * sectionLength; // Move to the next section position
            }
            else // 30% chance for a 90-degree turn
            {
                newSection = Instantiate(turnSectionPrefab, lastPosition, Quaternion.LookRotation(currentDirection));
                lastPosition += currentDirection * sectionLength;

                // Randomly decide whether to turn left or right
                if (Random.value < 0.5f)
                {
                    currentDirection = Quaternion.Euler(0, -90, 0) * currentDirection; // Left turn
                }
                else
                {
                    currentDirection = Quaternion.Euler(0, 90, 0) * currentDirection; // Right turn
                }
            }

            sectionsGenerated++;
        }
    }
}