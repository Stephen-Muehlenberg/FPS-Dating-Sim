using System.Collections;
using UnityEngine;

public static class CameraExtensions {
  /**
   * Zoom to the target field of view over the specified time.
   * The zoom accelerates proportionally to the current field of view, so each doubling of the FOV takes the same amount of time.
   */
  public static IEnumerator zoomProportional(this Camera camera, float targetFOV, float zoomTime) {
    float time = 0;
    float initialFOV = camera.fieldOfView;
    bool zoomingOut = initialFOV < targetFOV;

    // The number of times the FOV will double in value over the course of the zoom. E.g. going from 10 to 40 is two doublings.
    float doublings = Mathf.Log((targetFOV / initialFOV), 2);
    // How long it will take to double the FOV.
    float doublingTime = zoomTime / doublings;


    while ((zoomingOut && camera.fieldOfView < targetFOV) || (!zoomingOut && camera.fieldOfView > targetFOV)) {
      if (TimeUtils.dialogPaused) yield return null;
      else {
        time += TimeUtils.dialogDeltaTime;
        camera.fieldOfView = initialFOV * Mathf.Pow(2, (time / doublingTime));
        yield return null;
      }
    }
  }

  /**
   * Zoom to the target field of view over (approximately) the specified time.
   * Dampening is applied to the start and end.
   */
  public static IEnumerator zoomDamp(this Camera camera, float targetFOV, float zoomTime) {
    float velocity = 0;
    while (camera.fieldOfView != targetFOV) {
      if (TimeUtils.dialogDeltaTime == 0) yield return null;
      camera.fieldOfView = Mathf.SmoothDamp(camera.fieldOfView, targetFOV, ref velocity, zoomTime, float.MaxValue, TimeUtils.dialogDeltaTime);
      yield return null;
    }
  }
}
