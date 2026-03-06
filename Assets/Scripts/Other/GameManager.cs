using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public GameObject WinPanel;
    public GameObject LosePanel;
    public GameObject StopOption;

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            OnPressESC();
        }
    }

    public void OnPressESC()
    {
        Time.timeScale = StopOption.activeSelf ? 1 : 0;
        StopOption.SetActive(!StopOption.activeSelf);
        
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void Win()
    {
        Time.timeScale = 0f;
        WinPanel.SetActive(true);
    }

    public void Lost()
    {
        Time.timeScale = 0f;
        LosePanel.SetActive(true);
    }

    public void BackToMainMenu()
    {
        Time.timeScale = 1f;
        SceneLoadManager.Instance.LoadMenuScene();
    }
}
