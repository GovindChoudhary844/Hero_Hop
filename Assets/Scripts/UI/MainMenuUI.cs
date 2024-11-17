using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuUI : MonoBehaviour
{

    [Header("Main Menu Screens")]
    [SerializeField] private GameObject mainMenuScreen;
    [SerializeField] private GameObject optionScreen;

    private void Awake()
    {
        optionScreen.SetActive(false);
    }


    public void NewGame()
    {
        SceneManager.LoadScene(1);
    }

    public void Option(bool status)
    {
        //If status == true pause | if status == false unpause
        optionScreen.SetActive(status);
        mainMenuScreen.SetActive(!status);
    }

    public void SoundVolume()
    {
        SoundManager.instance.ChangeSoundVolume(0.2f);
    }

    public void MusicVolume()
    {
        SoundManager.instance.ChangeMusicVolume(0.2f);
    }

    public void Quit()
    {
        Application.Quit(); //Quit the game(work only in built)

        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false; //Exits play mode in editor
        #endif
    }
}
