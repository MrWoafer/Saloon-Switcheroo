using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Logo : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        PlayerPrefs.SetInt("EnableSwitcheroo", 1);
    }

    public void ToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
