using UnityEngine;
using UnityEngine.SceneManagement;

public class Transition : MonoBehaviour
{
    public Animator animator; // reference gameObject's animator

    public void FadeToNextLevel()
    {
        animator.SetTrigger("FadeOut");
        // an animation event call OnFadeComplete() after FadeOut
    }

    public void OnFadeComplete()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
