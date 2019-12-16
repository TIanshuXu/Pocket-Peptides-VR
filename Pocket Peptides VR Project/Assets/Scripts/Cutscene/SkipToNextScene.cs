using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SkipToNextScene : MonoBehaviour
{
    public float SecondsToSkip = 39f; // after how many sec skip to next scene

    private void Start()
    {
        StartCoroutine(SkipTheScene()); // go to mainscene after sec
    }

    IEnumerator SkipTheScene()
    {
        yield return new WaitForSeconds(SecondsToSkip);
        SceneManager.LoadScene("MainScene");
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // Left Click to skip to the next scene
        {
            SceneManager.LoadScene("MainScene");
        }
    }
}
