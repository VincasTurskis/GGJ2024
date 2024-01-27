using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialMenu : MonoBehaviour
{
    public GameObject mainMenu;

    public GameObject tutorialMenu;

    public void loadTutorialMenu(){
        mainMenu.SetActive(false);
        tutorialMenu.SetActive(true);
    }

    public void exitTutorialMenu(){
        mainMenu.SetActive(true);
        tutorialMenu.SetActive(false);
    }
}
