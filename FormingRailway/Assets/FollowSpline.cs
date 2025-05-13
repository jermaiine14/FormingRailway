using System.Collections.Generic;
using UnityEngine;

public class FollowSpline : MonoBehaviour
{
    public DrawSpline drawSpline;
    public float speed = 2f;
    public float rotationSpeed = 360f;

    private List<Vector3> pathPoints;
    private int currentIndex = 0;
    private bool isMoving = false;
    private bool isPausedByBlock = false;
    public bool IsMoving => isMoving;

    void Start()
    {
        drawSpline.OnSplineCompleted += StartFollowingSpline;
    }

    void Update()
    {
        if (isMoving)
        {
            if (!drawSpline.HasSpline)
            {
                isMoving = false;
                return;
            }

            if (IsBlockedByDraggable())
            {
                isPausedByBlock = true;
                return;
            }
            else if (isPausedByBlock)
            {
                isPausedByBlock = false;
            }

            if (currentIndex < pathPoints.Count)
            {
                Vector3 target = pathPoints[currentIndex];
                Vector3 direction = target - transform.position;

                // Smooth Z-axis rotation
                if (direction.sqrMagnitude > 0.001f)
                {
                    float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                    Quaternion targetRotation = Quaternion.Euler(0, 0, targetAngle);
                    transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
                }

                transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);

                if (Vector3.Distance(transform.position, target) < 0.0001f)
                {
                    currentIndex++;
                }

                if (currentIndex >= pathPoints.Count)
                {
                    isMoving = false;
                }
            }
        }
    }

    private void StartFollowingSpline()
    {
        if (DragObject.IsAnyDragging) return; // Don't start while dragging

        pathPoints = drawSpline.GetPoints();
        if (drawSpline.HasSpline && pathPoints.Count > 0)
        {
            currentIndex = 0;
            transform.position = pathPoints[0];
            isMoving = true;
            isPausedByBlock = false;
        }
    }

    bool IsBlockedByDraggable()
    {
        Collider followerCollider = GetComponent<Collider>();
        if (followerCollider == null) return false;

        GameObject[] draggables = GameObject.FindGameObjectsWithTag("Draggable");

        foreach (GameObject obj in draggables)
        {
            Collider objCollider = obj.GetComponent<Collider>();
            if (objCollider == null) continue;

            if (followerCollider.bounds.Intersects(objCollider.bounds))
            {
                return true;
            }
        }

        return false;
    }

    public List<Vector3> GetPath()
{
    return pathPoints;
}
public int GetCurrentIndex()
{
    return currentIndex;
}
}
