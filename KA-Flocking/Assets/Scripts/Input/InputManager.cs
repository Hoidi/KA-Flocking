using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// credit to BoardToBitsGames on YouTube

public abstract class InputManager : MonoBehaviour {

    public delegate void MoveInputHandler(Vector3 moveVector);
    public delegate void RotateInputHandler(float rotateAmount);
    public delegate void ZoomInputHandler(float zoomAmount);
    public delegate void SpeedInputHandler(float speed);

}
