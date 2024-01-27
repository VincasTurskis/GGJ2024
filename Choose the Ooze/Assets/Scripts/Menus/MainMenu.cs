using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public GameObject aboutMenu;

    void Awake(){   
        gameObject.SetActive(true);
        aboutMenu.SetActive(false);
        Debug.Log("Start main menu");
    }

    public void LaunchGame(){
        SceneManager.LoadScene(2);
    }
 
    public void QuitGame(){
        Debug.Log("Quit game");
        Application.Quit();
    }
}
