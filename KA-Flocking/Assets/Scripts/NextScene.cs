using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class NextScene : MonoBehaviour
{
    public void nextScene() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void restartGame() {
        Destroy(GameObject.Find("Team 1 Flock")); //remove gameobjects from finished game
        Destroy(GameObject.Find("Team 2 Flock")); //remove gameobjects from finished game
        Destroy(GameObject.Find("VisualManager")); //remove gameobjects from finished game
        SceneManager.LoadScene(0);
    }
}
