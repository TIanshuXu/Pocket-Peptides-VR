using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [HideInInspector]
    public static bool IsCutscenePlayed; // static makes the bool pass through scenes

    private void Start()
    {
        if (SceneManager.GetActiveScene().name == "MainScene" && !IsCutscenePlayed)
        {
            IsCutscenePlayed = true; // flip the flag
            SceneManager.LoadScene("Cutscene");
        }
    }

    public void PlayGame()
    {
        // SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        FindObjectOfType<Transition>().FadeToNextLevel(); // use transition code
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("QUIT!");
    }
}