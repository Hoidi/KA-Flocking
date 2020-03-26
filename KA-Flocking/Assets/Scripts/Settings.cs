using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    public InputField inputStartingMoney;
    public InputField inputMapX;
    public InputField inputMapZ;
    public Slider inputMountains;
    public InputField inputSeed;

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
            // Remove previous settings after copying
            Destroy(previousObject);
        }
        // Set the values
        inputStartingMoney.text = startingMoney.ToString();
        inputMapX.text = mapX.ToString();
        inputMapZ.text = mapZ.ToString();
        inputMountains.value = mountains;
        inputSeed.text = Random.Range(0, 10000).ToString();
        DontDestroyOnLoad(this.gameObject);
    }

    public void SaveValues() {
        startingMoney = int.Parse(inputStartingMoney.text);
        mapX = int.Parse(inputMapX.text);
        mapZ = int.Parse(inputMapZ.text);
        mountains = (int) inputMountains.value;
        seed = int.Parse(inputSeed.text);
    }
}
