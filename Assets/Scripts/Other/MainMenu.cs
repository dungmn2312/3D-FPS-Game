using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    
    private string gameScene;

    public TMP_Text highScoreUI;

    // Start is called before the first frame update
    void Start()
    {
        gameScene = "SampleScene";
        int highScore = SaveLoadManager.Instance.LoadHighScore();
        highScoreUI.text = $"Best Wave Survived: {highScore}";
    }

    public void StartGame()
    {
        SceneManager.LoadScene(gameScene);
    }

    public void ExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
    Application.Quit();

#endif
    }

}
