﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrthographicZoomStrategy : IZoomStrategy {

    public OrthographicZoomStrategy(Camera camera, float startingZoom) {
        camera.orthographicSize = startingZoom;
    }

    public void ZoomIn(Camera camera, float delta, float nearZoomLimit)
    {
        if (camera.orthographicSize != nearZoomLimit) {
            camera.orthographicSize = Mathf.Max(camera.orthographicSize - delta, nearZoomLimit);
        }

        //if (camera.orthographicSize == nearZoomLimit) return;
        //camera.orthographicSize = Mathf.Max(camera.orthographicSize - delta, nearZoomLimit);
    }

    public void ZoomOut(Camera camera, float delta, float farZoomLimit)
    {
        if (camera.orthographicSize != farZoomLimit) {
            camera.orthographicSize = Mathf.Min(camera.orthographicSize + delta, farZoomLimit);
        }

        //if (camera.orthographicSize == farZoomLimit) return;
        //camera.orthographicSize = Mathf.Min(camera.orthographicSize + delta, farZoomLimit);
    }
}
