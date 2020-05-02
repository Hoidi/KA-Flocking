using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class Settings : MonoBehaviour
{
    [Range(0,99999)]
    public int startingMoney = 50000;
    [Range(0,999)]
    public int mapX = 10;
    [Range(0,999)]
    public int mapZ = 10;
    [Range(10,2)]
    public int mountains = 6;
    public int seed;
    public int income = 2000;
    public bool hideEnemyFlock = true;

    public InputField inputStartingMoney;
    public InputField inputMapX;
    public InputField inputMapZ;
    public Slider inputMountains;
    public InputField inputSeed;
    public Text prevSeed;
    public Toggle inputHideEnemyFlock;
    public int turnDuration = 60;
    public int nTurns = 1;
    // Start is called before the first frame update
    void Start()
    {
        // If there already exists previos settings (from last game) then copy those settings. The name is changed in "NextScene.cs"
        GameObject previousObject = GameObject.Find("PreviousSettings");
        if (previousObject != null) {
            Settings previousSettings = previousObject.GetComponent<Settings>();
            startingMoney = previousSettings.startingMoney;
            mapX = previousSettings.mapX;
            mapZ = previousSettings.mapZ;
            mountains = previousSettings.mountains;
            seed = previousSettings.seed;
            prevSeed.gameObject.SetActive(true);
            GameObject.Find("PrevSeedNo").GetComponent<Text>().text= seed.ToString();
            hideEnemyFlock = previousSettings.hideEnemyFlock;
            // Remove previous settings after copying
            Destroy(previousObject);
        }
        // Set the values
        inputStartingMoney.text = startingMoney.ToString();
        inputMapX.text = mapX.ToString();
        inputMapZ.text = mapZ.ToString();
        inputMountains.value = mountains;
        RandomizeSeed();
        inputHideEnemyFlock.isOn = inputHideEnemyFlock;

        Time.timeScale = 0.0f; // pauses the game so that the troops stand still
        DontDestroyOnLoad(this.gameObject);
        StartCoroutine(TurnRoutine(turnDuration));
    }

    private IEnumerator TurnRoutine(int turnDuration) {
        SceneManager.sceneLoaded += updateTurnText;

        yield return new WaitForSeconds(turnDuration);
        while (SceneManager.GetActiveScene().name == "FlockScene") {
            nTurns++;

            if (nTurns % 2 == 0) {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
            } else {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 2);
            }

            Time.timeScale = 0.0f;

            Flock flock1 = GameObject.Find("Team 1 Flock").GetComponent<Flock>();
            flock1.moneyAmount += income;
            Flock flock2 = GameObject.Find("Team 2 Flock").GetComponent<Flock>();
            flock2.moneyAmount += income;
            
            yield return new WaitForSeconds(turnDuration);
        }
    }

    private void updateTurnText(Scene scene, LoadSceneMode mode) {
        if (scene.name.Equals("PlayerOneSetupScene") || scene.name.Equals("PlayerTwoSetupScene")) {
            Text turnText = GameObject.Find("TurnText").GetComponent<Text>();
            turnText.text = "Turn number " + nTurns;
            // Reset alpha and fade it out over 15 seconds
            turnText.CrossFadeAlpha(1, 0.0f, true);
            turnText.CrossFadeAlpha(0, 15.0f, true);
        }
    }

    public void SaveValues() {
        startingMoney = int.Parse(inputStartingMoney.text);
        mapX = int.Parse(inputMapX.text);
        mapZ = int.Parse(inputMapZ.text);
        // Adjust such that the amount of chunks is an even number
        mapX += mapX % 2;
        mapZ += mapZ % 2;
        mountains = (int) inputMountains.value;
        seed = int.Parse(inputSeed.text);
        hideEnemyFlock = inputHideEnemyFlock.isOn;
    }

    // Used to randomize the seed
    public void RandomizeSeed() {
        seed = Random.Range(0, 10000);
        inputSeed.text = seed.ToString();
    }
}
