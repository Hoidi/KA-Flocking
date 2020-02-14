using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// credit to BoardToBitsGames on YouTube

public class OrthographicZoomStrategy : IZoomStrategy {

    public OrthographicZoomStrategy(Camera camera, float startingZoom) {
        camera.orthographicSize = startingZoom;
    }

    public void ZoomIn(Camera camera, float delta, float nearZoomLimit)
    {
        if (camera.orthographicSize != nearZoomLimit) {
            camera.orthographicSize = Mathf.Max(camera.orthographicSize - delta, nearZoomLimit);
        }
    }

    public void ZoomOut(Camera camera, float delta, float farZoomLimit)
    {
        if (camera.orthographicSize != farZoomLimit) {
            camera.orthographicSize = Mathf.Min(camera.orthographicSize + delta, farZoomLimit);
        }
    }
}
