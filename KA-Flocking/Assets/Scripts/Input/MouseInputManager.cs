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

    private void Update()
    {
        screen = new Vector2Int(Screen.width, Screen.height);
        Vector3 mp = Input.mousePosition;

        // do nothing if mouse position is not valid (outside of screen)
        if (!MouseValid(mp))
        {
            return;
        }

        Move(mp);
        Rotate(mp);
        Zoom();
    }

    private bool MouseValid(Vector3 mp)
    {
        // mouse position is valid if no more than 15% outside of screen
        return mp.y <= screen.y * 1.15f && mp.y >= screen.y * -0.15f &&
               mp.x <= screen.x * 1.15f && mp.x >= screen.x * -0.15f;
    }

    private void Move(Vector3 mp)
    {
        if (mp.y > screen.y - 1)
        {
            OnMoveInput?.Invoke(Vector3.forward);
        }
        else if (mp.y < 1)
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
    }

    private void Rotate(Vector3 mp)
    {
        if (Input.GetMouseButtonDown(2)) // if pressing down scroll button
        {
            mousePositionOnRotate = mp.x;
        }
        else if (Input.GetMouseButton(2)) // still holding down
        {
            OnRotateInput?.Invoke(-((mousePositionOnRotate - mp.x) * 95) / screen.x);
            mousePositionOnRotate = mp.x;
        }
    }

    private void Zoom()
    {
        if (!Input.GetMouseButton(1) && Input.mouseScrollDelta.y > 0)
        {
            OnZoomInput?.Invoke(-5f);
        }
        else if (!Input.GetMouseButton(1) && Input.mouseScrollDelta.y < 0)
        {
            OnZoomInput?.Invoke(5f);
        }
    }
}
