
/* Camera Movement Variations
Linear Traveling + Zoom In

Description: The camera moves in a straight line from point A to point B while simultaneously zooming in.
Movement: A combination of linear traveling and zooming in on a point of interest.
Orbiting + Panorama

Description: The camera orbits around a central point while executing a horizontal panoramic movement.
Movement: Circular rotation around an object with simultaneous horizontal panning.
Zoom Out + Camera Rotation (Yaw)

Description: The camera zooms out while rotating around its Y-axis (Yaw).
Movement: A combination of zooming out and horizontal rotation.
Curved Traveling + Zoom In

Description: The camera follows an S-shaped curve while zooming in.
Movement: Curved trajectory with a zoom towards a central point.
Linear Traveling + Camera Rotation (Pitch)

Description: The camera moves in a straight line while adjusting its vertical viewing angle (Pitch).
Movement: Linear movement combined with vertical rotation.
Zoom In + Camera Rotation (Roll)

Description: The camera zooms in while rotating around its Z-axis (Roll).
Movement: Zoom in with lateral rotation.
Possible Combinations
The six variations above are basic examples. By combining traveling, rotation, panning, and zooming concepts in different sequences and with varying parameters, you can create an almost infinite number of variations.

For example:

Curved traveling with Yaw rotation.
Horizontal panning with zoom out and Roll rotation.
Orbiting with linear traveling and zoom in.
Each combination offers a new way to utilize cameras in a scene, creating dynamic and immersive visual effects.
*/


private IEnumerator OrbitCamera(Camera camera, Transform centerPoint, float radius, float duration)
{
    float elapsedTime = 0f;

    while (elapsedTime < duration)
    {
        float angle = Mathf.Lerp(0, 360, elapsedTime / duration);
        float radian = angle * Mathf.Deg2Rad;

        Vector3 offset = new Vector3(Mathf.Sin(radian), 0, Mathf.Cos(radian)) * radius;
        camera.transform.position = centerPoint.position + offset;
        camera.transform.LookAt(centerPoint);

        elapsedTime += Time.deltaTime;
        yield return null;
    }
}

private IEnumerator OrbitAndPan(Camera camera, Transform centerPoint, float radius, Vector3 panStartPos, Vector3 panEndPos, float duration)
{
    float elapsedTime = 0f;

    while (elapsedTime < duration)
    {
        float angle = Mathf.Lerp(0, 360, elapsedTime / duration);
        float radian = angle * Mathf.Deg2Rad;

        Vector3 offset = new Vector3(Mathf.Sin(radian), 0, Mathf.Cos(radian)) * radius;
        camera.transform.position = Vector3.Lerp(panStartPos + offset, panEndPos + offset, elapsedTime / duration);
        camera.transform.LookAt(centerPoint);

        elapsedTime += Time.deltaTime;
        yield return null;
    }
}

private IEnumerator PanCamera(Camera camera, Vector3 startPos, Vector3 endPos, float duration)
{
    float elapsedTime = 0f;

    while (elapsedTime < duration)
    {
        camera.transform.position = Vector3.Lerp(startPos, endPos, elapsedTime / duration);
        elapsedTime += Time.deltaTime;
        yield return null;
    }

    camera.transform.position = endPos;
}

private IEnumerator ZoomOutAndRotate(Camera camera, float startFOV, float endFOV, Quaternion startRot, Quaternion endRot, float duration)
{
    float elapsedTime = 0f;

    while (elapsedTime < duration)
    {
        camera.fieldOfView = Mathf.Lerp(startFOV, endFOV, elapsedTime / duration);
        camera.transform.rotation = Quaternion.Lerp(startRot, endRot, elapsedTime / duration);
        elapsedTime += Time.deltaTime;
        yield return null;
    }

    camera.fieldOfView = endFOV;
    camera.transform.rotation = endRot;
}

private IEnumerator RotateCamera(Camera camera, Quaternion startRot, Quaternion endRot, float duration)
{
    float elapsedTime = 0f;

    while (elapsedTime < duration)
    {
        camera.transform.rotation = Quaternion.Lerp(startRot, endRot, elapsedTime / duration);
        elapsedTime += Time.deltaTime;
        yield return null;
    }

    camera.transform.rotation = endRot;
}

private IEnumerator CurveTravelWithZoom(Camera camera, Vector3[] controlPoints, float startFOV, float endFOV, float duration)
{
    float elapsedTime = 0f;

    while (elapsedTime < duration)
    {
        float t = elapsedTime / duration;
        Vector3 position = Mathf.Pow(1 - t, 2) * controlPoints[0] + 2 * (1 - t) * t * controlPoints[1] + Mathf.Pow(t, 2) * controlPoints[2];
        camera.transform.position = position;
        camera.fieldOfView = Mathf.Lerp(startFOV, endFOV, t);
        elapsedTime += Time.deltaTime;
        yield return null;
    }

    camera.transform.position = controlPoints[2];
    camera.fieldOfView = endFOV;
}