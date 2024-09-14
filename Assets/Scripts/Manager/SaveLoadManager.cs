using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class SaveLoadManager : MonoBehaviour
{
    public static SaveLoadManager Instance { get; set; }

    public string highScoreKey = "BestWaveSaveValue";


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }

        DontDestroyOnLoad(this);
    }

    public void SaveHighScore(int score)
    {
        // Lưu giá trị dưới dạng Key - Value
        
        
        PlayerPrefs.SetInt(highScoreKey, score);
        
    }

    public int LoadHighScore()
    {
        // Kiểm tra key tồn tại ko
        if (PlayerPrefs.HasKey(highScoreKey))
        {
            
            return PlayerPrefs.GetInt(highScoreKey);
        }
        else
        {
            
            PlayerPrefs.SetInt(highScoreKey, 0);
            
            return 0;
        }
    }
}
