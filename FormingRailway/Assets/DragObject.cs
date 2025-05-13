using UnityEngine;

public class DragObject : MonoBehaviour
{
    private bool isDragging = false;
    private Vector3 offset;

    public static bool IsAnyDragging = false;

    void OnMouseDown()
    {
        isDragging = true;
        IsAnyDragging = true;

        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        offset = transform.position - new Vector3(mouseWorldPos.x, mouseWorldPos.y, transform.position.z);
    }

    void OnMouseUp()
    {
        isDragging = false;
        IsAnyDragging = false;
    }

    void Update()
    {
        if (isDragging)
        {
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            transform.position = new Vector3(mouseWorldPos.x, mouseWorldPos.y, transform.position.z) + offset;
        }
    }
}
