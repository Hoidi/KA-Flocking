using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// credit to BoardToBitsGames on YouTube

public class MouseInputManager : InputManager
{
    Vector2Int screen;
    float mousePositionOnRotate;

    // EVENTS
    public static event MoveInputHandler OnMoveInput;
    public static event RotateInputHandler OnRotateInput;
    public static event ZoomInputHandler OnZoomInput;

    private void Awake()
    {
        screen = new Vector2Int(Screen.width,Screen.height);
    }

    private void Update()
    {
        // mouse position is valid if no more than 5% outside of screen
        Vector3 mp = Input.mousePosition;
        bool mouseValid = (mp.y <= screen.y * 1.15f && mp.y >= screen.y * -0.15f &&
                           mp.x <= screen.x * 1.15f && mp.x >= screen.x * -0.15f);

        // do nothing if mouse position is not valid (outside of screen)
        if (!mouseValid)
        {
            return;
        }

        //movement
        if(mp.y > screen.y -1)
        {
            OnMoveInput?.Invoke(Vector3.forward);
        } else if (mp.y < 1)
        {
            OnMoveInput?.Invoke(-Vector3.forward);
        }
        if (mp.x > screen.x - 2)
        {
            OnMoveInput?.Invoke(Vector3.right);
        }
        else if (mp.x < 1)
        {
            OnMoveInput?.Invoke(Vector3.left);
        }        

        //rotation
        if (Input.GetMouseButtonDown(2)) // if pressing down scroll button
        {
            mousePositionOnRotate = mp.x;
        } else if (Input.GetMouseButton(2)) // still holding down
        {
            OnRotateInput?.Invoke(-((mousePositionOnRotate - mp.x)*95 )/ screen.x);
            mousePositionOnRotate = mp.x;
        }

        //zoom
        if(!Input.GetMouseButton(1) && Input.mouseScrollDelta.y > 0)
        {
            OnZoomInput?.Invoke(-5f);
        } else if (!Input.GetMouseButton(1) && Input.mouseScrollDelta.y < 0)
        {
            OnZoomInput?.Invoke(5f);
        }
    }
}
