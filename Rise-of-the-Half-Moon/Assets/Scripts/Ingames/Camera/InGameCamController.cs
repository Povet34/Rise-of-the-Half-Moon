using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameCamController : MonoBehaviour
{
    [SerializeField] RectTransform touchPanel;

    [SerializeField] Camera mainCamera;
    [SerializeField] Camera uiCamera;

    [SerializeField] float panSpeed = 0.1f;
    [SerializeField] float zoomSpeed = 0.1f;
    [SerializeField] float minZoom = 5f;
    [SerializeField] float maxZoom = 15f;
    [SerializeField] float scrollSpeed = 10f;

    private Vector2 previousTouchPosition;
    private bool isDragging = false;
    private float initialPinchDistance;

    public void Init(Bounds nodeBounds)
    {
        if (touchPanel == null)
        {
            Debug.LogError("Touch panel not assigned!");
            return;
        }

        if (mainCamera == null)
        {
            Debug.LogError("Main camera not assigned!");
            return;
        }

        // 카메라 초기화
        mainCamera.transform.position = new Vector3(nodeBounds.center.x, nodeBounds.center.y, -10);


        // 카메라의 orthographicSize를 바운드의 크기에 맞게 조절
        float sizeX = nodeBounds.size.x / 2f / mainCamera.aspect;
        float sizeY = nodeBounds.size.y / 2f;
        mainCamera.orthographicSize = Mathf.Clamp(Mathf.Max(sizeX, sizeY) * 1.5f, minZoom, maxZoom);

        uiCamera.transform.position = mainCamera.transform.position;
        uiCamera.orthographicSize = mainCamera.orthographicSize;

    }

    private void Update()
    {
#if UNITY_EDITOR
        HandleEditorInput();
#else
        HandleTouchInput();
#endif
    }

    private void HandleTouchInput()
    {
        if (Card.IsDraging || PUNCard.IsDraging)
            return;

        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                previousTouchPosition = touch.position;
                isDragging = RectTransformUtility.RectangleContainsScreenPoint(touchPanel, touch.position);
            }
            else if (touch.phase == TouchPhase.Moved && isDragging)
            {
                Vector2 touchDelta = touch.position - previousTouchPosition;
                previousTouchPosition = touch.position;

                PanCamera(touchDelta);
            }
            else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
            {
                isDragging = false;
            }
        }
        else if (Input.touchCount == 2)
        {
            Touch touch1 = Input.GetTouch(0);
            Touch touch2 = Input.GetTouch(1);

            if (touch1.phase == TouchPhase.Began || touch2.phase == TouchPhase.Began)
            {
                initialPinchDistance = Vector2.Distance(touch1.position, touch2.position);
            }
            else if (touch1.phase == TouchPhase.Moved || touch2.phase == TouchPhase.Moved)
            {
                float currentPinchDistance = Vector2.Distance(touch1.position, touch2.position);
                float pinchDelta = initialPinchDistance - currentPinchDistance;

                ZoomCamera(pinchDelta);
            }
        }
    }

    private void HandleEditorInput()
    {
        if (Card.IsDraging || PUNCard.IsDraging)
            return;

        if (Input.GetMouseButtonDown(0))
        {
            previousTouchPosition = Input.mousePosition;
            isDragging = RectTransformUtility.RectangleContainsScreenPoint(touchPanel, previousTouchPosition);
        }
        else if (Input.GetMouseButton(0) && isDragging)
        {
            Vector2 touchDelta = (Vector2)Input.mousePosition - previousTouchPosition;
            previousTouchPosition = Input.mousePosition;

            PanCamera(touchDelta);
        }
        else if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
        }

        float scroll = Input.GetAxis("Mouse ScrollWheel");

        if (scroll != 0)
        {
            ZoomCamera(scroll * scrollSpeed);
        }
    }

    private void PanCamera(Vector2 touchDelta)
    {
        Vector3 panMovement = new Vector3(-touchDelta.x * panSpeed, -touchDelta.y * panSpeed, 0);

        mainCamera.transform.position += panMovement;
        uiCamera.transform.position += panMovement;
    }

    private void ZoomCamera(float pinchDelta)
    {
        float newZoom = Mathf.Clamp(mainCamera.orthographicSize + pinchDelta * zoomSpeed, minZoom, maxZoom);

        mainCamera.orthographicSize = newZoom;
        uiCamera.orthographicSize = newZoom;
    }
}