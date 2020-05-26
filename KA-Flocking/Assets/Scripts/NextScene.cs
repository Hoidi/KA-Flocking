using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class NextScene : MonoBehaviour
{
    private Settings settings;
    private static bool configuredSetupOne,configuredSetupTwo  = false;
    public void nextScene() {
        int nextScene = SceneManager.GetActiveScene().buildIndex + 1;
        settings = GameObject.Find("SettingsObject").GetComponent<Settings>();

        if (SceneManager.GetActiveScene().name == "PlayerOneSetupScene") {
            if (!validateTroopSetup(GameObject.Find("Team 1 Flock").GetComponent<Flock>())) return;
            // If the turn started with player 2 (modulo 2 == 0) skip directly to flock scene after PlayerOneSetupScene
            if (settings.nTurns % 2 == 0 && settings.nTurns > 1) {
                Time.timeScale = 1.0f;
                nextScene++;
            }
        }
        if (SceneManager.GetActiveScene().name == "PlayerTwoSetupScene")
        {
            if (!validateTroopSetup(GameObject.Find("Team 2 Flock").GetComponent<Flock>())) return;
            // If the turn started with player 2 (modulo 2 == 0) go back to PlayerOneSetupScene instead of going to flockscene
            if (settings.nTurns % 2 == 0 && settings.nTurns > 1) {
                nextScene -= 2;
            } else {
                Time.timeScale = 1.0f;
            }
        }
        if (SceneManager.GetActiveScene().name == "FlockScene"){
            Time.timeScale = 0;
        }

        if (SceneManager.GetActiveScene().name.Equals("Menu")) {
            settings.SaveValues();
        }
        SceneManager.sceneLoaded += ConfigureSettings;
        SceneManager.LoadScene(nextScene);
    }

    private bool validateTroopSetup(Flock flock) {
        int castles = 0;
        foreach (FlockAgent agent in flock.agents)
        {
            if (agent.unit is Castle) castles++;
        }
        ErrorChat errorChat = GameObject.Find("ErrorBoard").GetComponent<ErrorChat>();
        if (castles == 0) {
            errorChat.ShowError("Atleast one castle is required");
            return false;
        } else if (castles == flock.agents.Count) {
            errorChat.ShowError("Atleast one spawned unit is required");
            return false;
        }
        return true;
    }

    public void restartGame() {
        Destroy(GameObject.Find("Team 1 Flock")); //remove gameobjects from finished game
        Destroy(GameObject.Find("Team 2 Flock")); //remove gameobjects from finished game
        Destroy(GameObject.Find("VisualManager")); //remove gameobjects from finished game
        GameObject.Find("SettingsObject").name = "PreviousSettings"; //renames the previous settingsObject
        SceneManager.LoadScene(0);
        configuredSetupOne = false;
        configuredSetupTwo = false;
    }

    private void setActiveFlockAgents(Flock flock, bool activity) {
        foreach (FlockAgent agent in flock.agents)
        {
            agent.gameObject.SetActive(activity);
        }
    }

    // Configures settings depending on the scenes
    void ConfigureSettings(Scene scene, LoadSceneMode mode) {
        if (settings == null) return;
        if (scene.name.Equals("PlayerOneSetupScene")) {
            if (configuredSetupOne == true) return;
            configuredSetupOne = true;

            ChunkManager chunkManager = GameObject.Find("VisualManager").GetComponent<ChunkManager>();
            chunkManager.pointyBreakOff = settings.inputMountains.value;
            chunkManager.chunksX = settings.mapX;
            chunkManager.chunksZ = settings.mapZ;
            chunkManager.seed = settings.seed;

            Flock flock = GameObject.Find("Team 1 Flock").GetComponent<Flock>();
            flock.moneyAmount = settings.startingMoney;
        } else if (scene.name.Equals("PlayerTwoSetupScene")) {
            if (configuredSetupTwo == true) return;
            configuredSetupTwo = true;

            if (settings.hideEnemyFlock) {
                setActiveFlockAgents(GameObject.Find("Team 1 Flock").GetComponent<Flock>(), false);
            }

            Flock flock = GameObject.Find("Team 2 Flock").GetComponent<Flock>();
            flock.moneyAmount = settings.startingMoney;
        } else if (scene.name.Equals("FlockScene")) {
            if (settings.nTurns == 1 && settings.hideEnemyFlock) {
                setActiveFlockAgents(GameObject.Find("Team 1 Flock").GetComponent<Flock>(), true);
            }
        }
    }
}
