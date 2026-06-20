using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    string newGameScene = "SampleScene";
    public TextMeshProUGUI highScoreUI;

    public AudioClip bgMusic;
    public AudioSource mainChannel;
   
    private void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        int highScore = SaveLoadManager.Instance.LoadHighScore();
        highScoreUI.text = "Top Wave Survived: " + highScore;

        mainChannel.clip = bgMusic;
        mainChannel.Play();
    }

    public void StartNewGame()
    {
        mainChannel.Stop();
        SceneManager.LoadScene(newGameScene);
    }

    public void ExitApplication()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}