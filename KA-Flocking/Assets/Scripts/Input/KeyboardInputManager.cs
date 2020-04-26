using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


// credit to BoardToBitsGames on YouTube

public class KeyboardInputManager : InputManager {

    // EVENTS
    public static event MoveInputHandler OnMoveInput;
    public static event RotateInputHandler OnRotateInput;
    public static event ZoomInputHandler OnZoomInput;
    public static event SpeedInputHandler OnSpeedInput;


    // Update is called once per frame
    void Update() {
        //movement
        if (Input.GetKey(KeyCode.W)) {
            OnMoveInput?.Invoke(Vector3.forward);
        }
        if (Input.GetKey(KeyCode.S)) {
            OnMoveInput?.Invoke(-Vector3.forward);
        }
        if (Input.GetKey(KeyCode.A)) {
            OnMoveInput?.Invoke(-Vector3.right);
        }
        if (Input.GetKey(KeyCode.D)) {
            OnMoveInput?.Invoke(Vector3.right);
        }

        //rotate
        if (Input.GetKey(KeyCode.E)) {
            OnRotateInput?.Invoke(1.0f);
        }
        if (Input.GetKey(KeyCode.Q)) {
            OnRotateInput?.Invoke(-1.0f);
        }

        //zoom
        if (Input.GetKey(KeyCode.Z)){
            OnZoomInput?.Invoke(-1.0f);
        }
        if (Input.GetKey(KeyCode.X)) {
            OnZoomInput?.Invoke(1.0f);
        }

        //camera speed
        if (Input.GetKey(KeyCode.LeftShift)) {
            OnSpeedInput?.Invoke(1.25f);
        }
        if (Input.GetKey(KeyCode.LeftControl)) {
            OnSpeedInput?.Invoke(0.8f);
        }

        if (SceneManager.GetSceneByName("FlockScene").isLoaded) { //change timescale, but only if in simulation scene
            //simulation speed
            if (Input.GetKey(KeyCode.Alpha1)) {
                Time.timeScale = 1.0f;
            }
            if (Input.GetKey(KeyCode.Alpha2)) {
                Time.timeScale = 2.0f;
            }
            if (Input.GetKey(KeyCode.Alpha3)) {
                Time.timeScale = 3.0f;
            }
            if (Input.GetKey(KeyCode.Alpha4)) {
                Time.timeScale = 4.0f;
            }
            if (Input.GetKey(KeyCode.Alpha5)) {
                Time.timeScale = 5.0f;
            }
        }
        // Can only pause in FlockScene since the game will already be frozen
        if (Input.GetKey(KeyCode.Alpha0)) {
            if (SceneManager.GetActiveScene().name == "FlockScene") {
                Time.timeScale = 0.0f;
            }
        }
    }
}
