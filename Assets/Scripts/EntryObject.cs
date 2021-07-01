using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EntryObject : MonoBehaviour
{
    const string TrailerKey = "watched_trailer";
    void Start()
    {
        if (PlayerPrefs.HasKey(TrailerKey))
        {
            SceneManager.LoadScene("game");
        }
        else
        {
            PlayerPrefs.SetInt(TrailerKey, 1);
            PlayerPrefs.Save();
            SceneManager.LoadScene("trailer");
        }
    }
}