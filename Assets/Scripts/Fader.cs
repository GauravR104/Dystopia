using UnityEngine;
using UnityEngine.SceneManagement;

public class Fader : MonoBehaviour
{
    // Start is called before the first frame update
    Animator animator;
    public int sceneIndex;

    private string loadSceneName;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }


    public void FadeToLevel(string sceneName = "")
    {
        animator.SetTrigger("FadeOut");

        if (!string.IsNullOrEmpty(sceneName))
        {
            sceneIndex = -1;
            loadSceneName = sceneName;
        }
    }

    public void FadeComplete()
    {

        if (sceneIndex == -1)
        {
            SceneManager.LoadScene(loadSceneName);

        }
        else
        {
            SceneManager.LoadScene(sceneIndex);
        }

    }
}
