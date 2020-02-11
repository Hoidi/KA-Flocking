using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InputManager : MonoBehaviour {

    public delegate void MoveInpuptHandler(Vector3 moveVector);
    public delegate void RotateInpuptHandler(float rotateAmount);
    public delegate void ZoomInpuptHandler(float zoomAmount);

}
