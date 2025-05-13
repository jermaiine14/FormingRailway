using System.Collections.Generic;
using UnityEngine;

public class FollowBehind : MonoBehaviour
{
    public FollowSpline followSpline;
    public float followDistance = 2f;           // Distance behind in units
    public float teleportThreshold = 10f;       // Distance before teleporting
    public float moveSpeed = 3f;                // Movement speed
    public float rotationSpeed = 360f;          // Z rotation smoothing

    private List<Vector3> pathPoints => followSpline.GetPath(); // Get current spline path
    private Transform leader => followSpline.transform;

    private int targetIndex = 0;

    void Update()
    {
        if (!followSpline.IsMoving || pathPoints == null || pathPoints.Count < 2)
            return;

        // Find the point on the spline closest to followSpline but offset behind by distance
        int leaderIndex = followSpline.GetCurrentIndex();

        // Estimate how many points correspond to the desired followDistance
        float accumulatedDistance = 0f;
        int behindIndex = leaderIndex;

        for (int i = leaderIndex - 1; i >= 0; i--)
        {
            accumulatedDistance += Vector3.Distance(pathPoints[i + 1], pathPoints[i]);
            if (accumulatedDistance >= followDistance)
            {
                behindIndex = i;
                break;
            }
        }

        // If too far, teleport to the desired position
        if (Vector3.Distance(transform.position, pathPoints[behindIndex]) > teleportThreshold)
        {
            transform.position = pathPoints[behindIndex];
        }
        else
        {
            // Move smoothly toward that point
            transform.position = Vector3.MoveTowards(transform.position, pathPoints[behindIndex], moveSpeed * Time.deltaTime);
        }

        // Rotate Z-axis smoothly to match direction along spline
        Vector3 dir = pathPoints[behindIndex + 1] - pathPoints[behindIndex];
        if (dir.sqrMagnitude > 0.001f)
        {
            float targetAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            Quaternion targetRot = Quaternion.Euler(0, 0, targetAngle);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, rotationSpeed * Time.deltaTime);
        }
    }
}
