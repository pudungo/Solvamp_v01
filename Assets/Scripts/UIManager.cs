using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public GameObject GameOverScreen;
    private Health playerHealth;

    private void Awake()
    {
        GameOverScreen.SetActive(false); // turns off when awake on start
        playerHealth = GameObject.FindGameObjectWithTag("Player").GetComponent<Health>(); // calls player's health

    }

    // Update is called once per frame
    void Update()
    {
        if (playerHealth != null && playerHealth.IsDead)
        {
            GameOverScreen.SetActive(true);
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = true;
        }
    }

    public void StartGame()
    {
        Debug.Log("Start pressed!");

        SceneManager.LoadScene(sceneBuildIndex: 1);

    }

    public void ExitGame()
    {
        Debug.Log("Exit pressed!");

        // exit game

    }

    public void RestartGame()
    {
        // Reload scene
        Debug.Log("Restart pressed!");
        //    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); // resets to start of level

        SceneManager.LoadScene(sceneBuildIndex: 0);
    }
}
