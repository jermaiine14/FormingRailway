using System.Collections.Generic;
using UnityEngine;
using System;

public class DrawSpline : MonoBehaviour
{
    public LineRenderer lineRenderer;
    public float pointSpacing = 0.1f;
    public LayerMask drawingLayer;

    private List<Vector3> points = new List<Vector3>();
    private Camera cam;
    private bool isDrawing = false;

    public Action OnSplineCompleted;

    public bool HasSpline => points.Count > 1;

    void Start()
    {
        cam = Camera.main;
        ClearSpline();
    }

    void Update()
    {
        // Disable drawing if dragging
        if (DragObject.IsAnyDragging)
            return;

        if (Input.GetMouseButtonDown(0))
        {
            ClearSpline();
            isDrawing = true;
        }

        if (isDrawing && Input.GetMouseButton(0))
        {
            Vector3 mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 0;

            if (points.Count == 0 || Vector3.Distance(points[points.Count - 1], mousePos) > pointSpacing)
            {
                points.Add(mousePos);
                lineRenderer.positionCount = points.Count;
                lineRenderer.SetPositions(points.ToArray());
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            if (isDrawing && points.Count > 1 && !DragObject.IsAnyDragging)
            {
                OnSplineCompleted?.Invoke();
            }

            isDrawing = false;
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            ClearSpline();
        }
    }

    public void ClearSpline()
    {
        points.Clear();
        lineRenderer.positionCount = 0;
    }

    public List<Vector3> GetPoints()
    {
        return new List<Vector3>(points);
    }
}
