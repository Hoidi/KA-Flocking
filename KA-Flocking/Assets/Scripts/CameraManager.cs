using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// credit to BoardToBitsGames on YouTube

public class CameraManager : MonoBehaviour {

    [Header("Camera Positioning")]
    public Vector2 cameraOffset = new Vector2(10.0f,14.0f);
    public float lookAtOffset = 2.0f;

    [Header("Move Controls")]
    public float originalInOutSpeed = 20.0f;
    public float originalLateralSpeed = 20.0f;
    float inOutSpeed;
    float lateralSpeed;
    public float slowSpeed = 12.0f;
    public float fastSpeed = 40.0f;
    public float rotateSpeed = 100.0f;

    [Header("Move Bounds")]
    public Vector2 minBounds, maxBounds;

    [Header("Zoom Controls")]
    public float zoomSpeed = 20.0f;
    public float nearZoomLimit = 2.0f;
    public float farZoomLimit = 100.0f;
    public float startingZoom = 30.0f;

    IZoomStrategy zoomStrategy;
    Vector3 frameMove;
    float frameRotate;
    float frameZoom;
    new Camera camera;
    float lastFrameTime;

    private void Awake() {
        inOutSpeed = originalInOutSpeed * (frameZoom / startingZoom);
        lateralSpeed = originalLateralSpeed;

        camera = GetComponentInChildren<Camera>();
        camera.transform.localPosition = new Vector3(0f, Mathf.Abs(cameraOffset.y), -Mathf.Abs(cameraOffset.x));

        zoomStrategy = camera.orthographic ? (IZoomStrategy) new OrthographicZoomStrategy(camera, startingZoom) : new PerspectiveZoomStrategy(camera, cameraOffset, startingZoom); ;
        camera.transform.LookAt(transform.position + Vector3.up * lookAtOffset);

        lastFrameTime = Time.realtimeSinceStartup;

        GameObject settingsObject = GameObject.Find("SettingsObject");
        if (settingsObject != null) {
            Settings settings = settingsObject.GetComponent<Settings>();
            minBounds.x = -(settings.mapX*13);
            minBounds.y = -(settings.mapZ*13);
            maxBounds.x = settings.mapX*13;
            maxBounds.y = settings.mapZ*13;
        }
    }

    private void OnEnable() {
        KeyboardInputManager.OnMoveInput += UpdateFrameMove;
        KeyboardInputManager.OnRotateInput += UpdateFrameRotate;
        KeyboardInputManager.OnZoomInput += UpdateFrameZoom;
        KeyboardInputManager.OnSpeedInput += UpdateFrameSpeed;
        MouseInputManager.OnMoveInput += UpdateFrameMove;
        MouseInputManager.OnRotateInput += UpdateFrameRotate;
        MouseInputManager.OnZoomInput += UpdateFrameZoom;
    }

    private void UpdateFrameSpeed(float speed) {
        // this is pretty ugly but it works
        if (speed > 1) {
            lateralSpeed = fastSpeed * (zoomStrategy.getCurrentZoomLevel() / startingZoom);
            inOutSpeed = fastSpeed * (zoomStrategy.getCurrentZoomLevel() / startingZoom);
        } else {
            lateralSpeed = slowSpeed * (zoomStrategy.getCurrentZoomLevel() / startingZoom);
            inOutSpeed = slowSpeed * (zoomStrategy.getCurrentZoomLevel() / startingZoom);
        }
    }

    private void UpdateFrameMove(Vector3 moveVector) {
        frameMove += moveVector;
    }

    private void UpdateFrameRotate(float rotateAmount) {
        frameRotate += rotateAmount;
    }
    private void UpdateFrameZoom(float zoomAmount) {
        frameZoom += zoomAmount;
    }

    private void OnDisable() {
        KeyboardInputManager.OnMoveInput -= UpdateFrameMove;
        KeyboardInputManager.OnRotateInput -= UpdateFrameRotate;
        KeyboardInputManager.OnZoomInput -= UpdateFrameZoom;
        KeyboardInputManager.OnSpeedInput -= UpdateFrameSpeed;
        MouseInputManager.OnMoveInput -= UpdateFrameMove;
        MouseInputManager.OnRotateInput -= UpdateFrameRotate;
        MouseInputManager.OnZoomInput -= UpdateFrameZoom;
    }

    private void LateUpdate() {
        float myDeltaTime = Time.realtimeSinceStartup - lastFrameTime;
        lastFrameTime = Time.realtimeSinceStartup;
        if (frameMove != Vector3.zero) {
            Vector3 speedModFrameMove = new Vector3(frameMove.x * lateralSpeed, frameMove.y, frameMove.z * inOutSpeed);
            transform.position += transform.TransformDirection(speedModFrameMove) * myDeltaTime;
            LookPositionInBouds();
            frameMove = Vector3.zero;
            
        }

        if (frameRotate != 0f) {
            transform.Rotate(Vector3.up, frameRotate * myDeltaTime * rotateSpeed);
            frameRotate = 0f;
        }

        if (frameZoom < 0f) {
            zoomStrategy.ZoomIn(camera, myDeltaTime * Mathf.Abs(frameZoom) * zoomSpeed * (zoomStrategy.getCurrentZoomLevel()/startingZoom), nearZoomLimit);
            frameZoom = 0f;
        }

        if (frameZoom > 0f) {
            zoomStrategy.ZoomOut(camera, myDeltaTime * frameZoom * zoomSpeed * (zoomStrategy.getCurrentZoomLevel() / startingZoom), farZoomLimit);
            frameZoom = 0f;
        }

        lateralSpeed = originalLateralSpeed * (zoomStrategy.getCurrentZoomLevel() / startingZoom);
        inOutSpeed = originalInOutSpeed * (zoomStrategy.getCurrentZoomLevel() / startingZoom);
    }

    private void LookPositionInBouds() {
        transform.position = new Vector3(
            Mathf.Clamp(transform.position.x, minBounds.x, maxBounds.x),
            transform.position.y,
            Mathf.Clamp(transform.position.z, minBounds.y, maxBounds.y));
    }
}
