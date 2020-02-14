using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// credit to BoardToBitsGames on YouTube

public class PerspectiveZoomStrategy : IZoomStrategy {

    Vector3 normalizedCameraPosition;
    float currentZoomLevel;


    public PerspectiveZoomStrategy(Camera camera, Vector3 offset, float startingZoom) {
        normalizedCameraPosition = new Vector3(0f, Mathf.Abs(offset.y), -Mathf.Abs(offset.x)).normalized;
        currentZoomLevel = startingZoom;
        PositionCamera(camera);
    }

    private void PositionCamera(Camera camera)
    {
        camera.transform.localPosition = normalizedCameraPosition * currentZoomLevel;
    }

    public void ZoomIn(Camera camera, float delta, float nearZoomLimit) {
        if (currentZoomLevel > nearZoomLimit) {
            currentZoomLevel = Mathf.Max(currentZoomLevel - delta, nearZoomLimit);
            PositionCamera(camera);
        }
    }

    public void ZoomOut(Camera camera, float delta, float farZoomLimit) {
        if (currentZoomLevel < farZoomLimit) {
            currentZoomLevel = Mathf.Min(currentZoomLevel + delta, farZoomLimit);
            PositionCamera(camera);
        }
    }
}
