using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour {

    [Header("Camera Positioning")]
    public Vector2 cameraOffset = new Vector2(10.0f,14.0f);
    public float lookAtOffset = 2.0f;

    [Header("Move Controls")]
    public float inOutSpeed = 5.0f;
    public float lateralSpeed = 5.0f;
    public float rotateSpeed = 5.0f;

    [Header("Move Bounds")]
    public Vector2 minBounds, maxBounds;

    [Header("Zoom Controls")]
    public float zoomSpeed = 4.0f;
    public float nearZoomLimit = 2.0f;
    public float farZoomLimit = 16.0f;
    public float startingZoom = 5.0f;

    IZoomStrategy zoomStrategy;
    Vector3 frameMove;
    float frameRotate;
    float frameZoom;
    Camera camera;

    private void Awake() {
        camera = GetComponentsInChildren<Camera>()[0];
        camera.transform.localPosition = new Vector3(0f, Mathf.Abs(cameraOffset.y), -Mathf.Abs(cameraOffset.x));

        zoomStrategy = new OrthographicZoomStrategy(camera, startingZoom);
        camera.transform.LookAt(transform.position + Vector3.up * lookAtOffset);
    }

    private void OnEnable() {
        KeyboardInputManager.OnMoveInput += UpdateFrameMove;
        KeyboardInputManager.OnRotateInput += UpdateFrameRotate;
        KeyboardInputManager.OnZoomInput += UpdateFrameZoom;
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
    }

    private void LateUpdate() {
        if (frameMove != Vector3.zero) {
            Vector3 speedModFrameMove = new Vector3(frameMove.x * lateralSpeed, frameMove.y, frameMove.z * inOutSpeed);
            transform.position += transform.TransformDirection(speedModFrameMove) * Time.deltaTime;
            LookPositionInBouds();
            frameMove = Vector3.zero;
            
        }

        if (frameRotate != 0f) {
            transform.Rotate(Vector3.up, frameRotate * Time.deltaTime * rotateSpeed);
            frameRotate = 0f;
        }

        if (frameZoom < 0f) {
            zoomStrategy.ZoomIn(camera, Time.deltaTime * Mathf.Abs(frameZoom) * zoomSpeed, nearZoomLimit);
            frameZoom = 0f;

        }

        if (frameZoom > 0f) {
            zoomStrategy.ZoomOut(camera, Time.deltaTime * frameZoom * zoomSpeed, farZoomLimit);
            frameZoom = 0f;
        }
    }

    private void LookPositionInBouds() {
        transform.position = new Vector3(
            Mathf.Clamp(transform.position.x, minBounds.x, maxBounds.x),
            transform.position.y,
            Mathf.Clamp(transform.position.z, minBounds.y, maxBounds.y));
    }
}
