using UnityEngine;

public class MainMenuManager : MonoBehaviour
{
    public GameObject settingMenu;




    //play 버튼
    public void PlayButton()
    {
        SceneLoadManager.Instance.LoadPlayScene();
    }

    public void MainMenu()
    {
        SceneLoadManager.Instance.LoadMenuScene();
    }

    public void SettingButton()
    {
        settingMenu.SetActive(!settingMenu.activeSelf);
    }

    //exit 버튼
    public void ExitButton()
    {
        Application.Quit();
    }


}
