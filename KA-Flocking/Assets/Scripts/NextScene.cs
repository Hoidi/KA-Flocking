using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class NextScene : MonoBehaviour
{
    private Settings settings;
    public void nextScene() {
        if (SceneManager.GetActiveScene().name.Equals("Menu")) {
            settings = GameObject.Find("SettingsObject").GetComponent<Settings>();
            settings.SaveValues();
        }
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        SceneManager.sceneLoaded += ConfigureSettings;
    }

    public void restartGame() {
        Destroy(GameObject.Find("Team 1 Flock")); //remove gameobjects from finished game
        Destroy(GameObject.Find("Team 2 Flock")); //remove gameobjects from finished game
        Destroy(GameObject.Find("VisualManager")); //remove gameobjects from finished game
        GameObject.Find("SettingsObject").name = "PreviousSettings"; //renames the previous settingsObject
        SceneManager.LoadScene(0);
    }

    // Configures settings depending on the scenes
    void ConfigureSettings(Scene scene, LoadSceneMode mode) {
        if (settings == null) return;
        if (scene.name.Equals("PlayerOneSetupScene")) {
            ChunkManager chunkManager = GameObject.Find("VisualManager").GetComponent<ChunkManager>();
            chunkManager.pointyBreakOff = settings.inputMountains.value;
            chunkManager.chunksX = settings.mapX;
            chunkManager.chunksZ = settings.mapZ;
            chunkManager.seed = settings.seed;

            Flock flock = GameObject.Find("Team 1 Flock").GetComponent<Flock>();
            flock.moneyAmount = settings.startingMoney;
        } else if (scene.name.Equals("PlayerTwoSetupScene")) {
            Flock flock = GameObject.Find("Team 2 Flock").GetComponent<Flock>();
            flock.moneyAmount = settings.startingMoney;
        }
    }
}
