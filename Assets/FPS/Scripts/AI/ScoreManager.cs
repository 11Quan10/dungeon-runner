using UnityEngine;
using UnityEngine.UI;

namespace Unity.FPS.AI
{
    public class ScoreManager : MonoBehaviour
    {
        public Text ScoreText;

        private int currentScore;

        void Start()
        {
            currentScore = 0;
            UpdateScoreUI();
        }

        public void AddScore(int scoreToAdd)
        {
            currentScore += scoreToAdd;
            UpdateScoreUI();
        }

        void UpdateScoreUI()
        {
            if (ScoreText)
            {
                ScoreText.text = "Score: " + currentScore;
            }
        }
    }
}
