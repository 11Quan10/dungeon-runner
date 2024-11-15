using System.Collections.Generic;
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
        public GameObject Player;
        public int maxGeneratedSections = 1;     // Number of sections to generate ahead

        int sectionLength = 18;            // Length of each section
        Vector3 lastPosition;
        Vector3 currentDirection = Vector3.forward;
        int sectionsGenerated = 0; 
        int currentTurns = 0;               // Current number of turns, used to indicate direction
        bool startSection = true;
        int straightSections = 0;
        Queue<GameObject> instantiatedSections;

        void Start()
        {
            instantiatedSections = new Queue<GameObject>();
            lastPosition = Player.transform.position;        // Starting point of the dungeon
            GenerateSection();
        }

        void Update()
        {
            // GenerateSection();
            if (Vector3.Distance(Player.transform.position, lastPosition) < sectionLength * 3)
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

            if (startSection == true) {
                newSection = Instantiate(startSectionPrefab, lastPosition, Quaternion.LookRotation(currentDirection));

                startSection = false;
                sectionsGenerated++;
                goto UpdateSection;
            }
            
            if (straightSections < 2 || randomValue < 0.7f)     // 70% chance for a straight section
            {
                newSection = Instantiate(straightSectionPrefab, lastPosition, Quaternion.LookRotation(currentDirection));
                straightSections++;
            }
            else                    // 30% chance for a 90-degree turn
            {
                straightSections = 0;
                if (currentTurns > 1)                       
                {
                    newSection = Instantiate(rightTurnSectionPrefab, lastPosition, Quaternion.LookRotation(currentDirection));
                    currentDirection = Quaternion.Euler(0, 90, 0) * currentDirection;                   // Right turn

                    currentTurns = -1;
                }
                else if (currentTurns < -1)
                {
                    newSection = Instantiate(leftTurnSectionPrefab, lastPosition, Quaternion.LookRotation(currentDirection));
                    currentDirection = Quaternion.Euler(0, -90, 0) * currentDirection;                  // Left turn

                    currentTurns = 1;
                }
                else
                {
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
            }

        UpdateSection:
            instantiatedSections.Enqueue(newSection);
            if (instantiatedSections.Count >= 10)
            {
                Destroy(instantiatedSections.Dequeue());
            }
            
            lastPosition += currentDirection * sectionLength;

            sectionsGenerated++;
        }
    }
}