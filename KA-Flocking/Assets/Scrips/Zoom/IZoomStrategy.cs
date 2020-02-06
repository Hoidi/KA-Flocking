using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IZoomStrategy {

    void ZoomIn(Camera camera, float delta, float nearZoomLimit);
    void ZoomOut(Camera camera, float delta, float farZoomLimit);

}
