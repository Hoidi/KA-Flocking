using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Settings : MonoBehaviour
{
    [Range(0,9999999)]
    public int startingMoney = 50000;
    [Range(0,999)]
    public int mapX = 10;
    [Range(0,999)]
    public int mapZ = 10;
    [Range(10,2)]
    public int mountains = 6;
    private int seed;

    public InputField inputStartingMoney;
    public InputField inputMapX;
    public InputField inputMapZ;
    public Slider inputMountains;
    public InputField inputSeed;

    // Start is called before the first frame update
    void Start()
    {
        inputStartingMoney.text = startingMoney.ToString();
        inputMapX.text = mapX.ToString();
        inputMapZ.text = mapZ.ToString();
        inputMountains.value = mountains;
        inputSeed.text = Random.Range(0, 10000).ToString();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
