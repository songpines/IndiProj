using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoadManager : MonoBehaviour
{
    private const int MAIN_SCENE = 0;
    private const int PLAY_SCENE = 1;
    public static SceneLoadManager Instance { get; private set; }

    private void Awake()
    {
        if(Instance != null)
        {
            Destroy(gameObject);
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    
    public void LoadPlayScene()
    {
        SceneManager.LoadScene(PLAY_SCENE);
    }

    public void LoadMenuScene()
    {
        SceneManager.LoadScene(MAIN_SCENE);
    }


}
