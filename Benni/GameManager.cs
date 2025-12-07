using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("UI")]
    public GameObject gameOverPanel;     // Assign in Inspector
    public GameObject pausePanel;     // Assign in Inspector
    public TextMeshProUGUI finalScoreText;
    

    private bool isGameOver = false;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!isGameOver) // only allow pause/resume if the game isn't over
            {
                if (Time.timeScale == 1f) {
                    pausePanel.SetActive(true);
                    PauseGame();
                }

                else
                {
                    pausePanel.SetActive(false);
                    ResumeGame();
                }
            }
        }
    }
    void Awake()
    {
        if (Instance == null) Instance = this;
    }

    public void GameOver()
    {
        if (isGameOver) return;
        isGameOver = true;

        // Stop time
        Time.timeScale = 0f;

        // Save best score
        ScoreManager sm = FindObjectOfType<ScoreManager>();
        sm.SaveBestScore();

        // Show panel
        gameOverPanel.SetActive(true);

        // Show final + best score
        int finalScore = Mathf.FloorToInt(sm.score);
        int bestScore = sm.GetBestScore();
        finalScoreText.text = $"Final Score: {finalScore}\nBest Score: {bestScore}";
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        AudioManager.Instance.PlayMusic();
    }
    public void PauseGame()
    {
        Time.timeScale = 0f;  // stops all physics, Update() with deltaTime, animations, etc.
        Debug.Log("Game Paused!");
        AudioManager.Instance.PauseMusic();
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f;  // normal speed again
        pausePanel.SetActive(false);
        Debug.Log("Game Resumed!");
        AudioManager.Instance.PlayMusic();
    }


}
