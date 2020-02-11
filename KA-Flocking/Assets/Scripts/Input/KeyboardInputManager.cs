using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyboardInputManager : InputManager {

    // EVENTS
    public static event MoveInpuptHandler OnMoveInput;
    public static event RotateInpuptHandler OnRotateInput;
    public static event ZoomInpuptHandler OnZoomInput;


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
            OnRotateInput?.Invoke(-1.0f);
        }
        if (Input.GetKey(KeyCode.Q)) {
            OnRotateInput?.Invoke(1.0f);
        }

        //zoom
        if (Input.GetKey(KeyCode.Z)){
            OnZoomInput?.Invoke(-1.0f);
        }
        if (Input.GetKey(KeyCode.X)) {
            OnZoomInput?.Invoke(1.0f);
        }
    }
}
