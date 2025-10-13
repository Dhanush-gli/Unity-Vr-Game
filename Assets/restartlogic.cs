using UnityEngine;
using UnityEngine.SceneManagement;

public class GliderCrashHandler : MonoBehaviour
{
    [Header("Crash Settings")]
    public bool useGameOverUI = true;       // enable/disable Game Over UI behavior
    public GameObject gameOverUI;           // assign your Game Over UI prefab or panel
    public string deadlyTag = "Deadly";     // tag for objects that cause death
    public bool useAllCollisions = true;    // crash on any collision if true

    private bool isDead = false;

    void OnCollisionEnter(Collision collision)
    {
        if (isDead) return;

        if (useAllCollisions || collision.gameObject.CompareTag(deadlyTag))
        {
            isDead = true;

            if (useGameOverUI && gameOverUI != null)
            {
                Debug.Log("💀 You Crashed! Showing Game Over UI...");
                Time.timeScale = 0f;            // pause the game
                gameOverUI.SetActive(true);     // show UI
            }
            else
            {
                // default instant restart
                Debug.Log("💀 You Crashed! Restarting...");
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
        }
    }

    void Update()
    {
        if (isDead && useGameOverUI)
        {
            // restart on R key press
            if (Input.GetKeyDown(KeyCode.R))
            {
                Time.timeScale = 1f;  // unpause
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
        }
    }
}
