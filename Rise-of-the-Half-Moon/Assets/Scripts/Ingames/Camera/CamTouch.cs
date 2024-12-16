using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CamTouch : MonoBehaviour
{
    [SerializeField] CinemachineFreeLook freeLookCam;
    [SerializeField] RectTransform touchPanel;

    private Vector2 previousTouchPosition;
    private bool isDragging = false;

    private void Start()
    {
        if (freeLookCam == null)
        {
            Debug.LogError("CinemachineFreeLook camera not assigned!");
            return;
        }

        if (touchPanel == null)
        {
            Debug.LogError("Touch panel not assigned!");
            return;
        }

        AdjustRigRadius();
    }

    private void Update()
    {
        HandleTouchInput();
    }

    private void AdjustRigRadius()
    {
        // 현재 해상도와 가로세로 비율을 가져옵니다.
        float screenWidth = Screen.width;
        float screenHeight = Screen.height;
        float aspectRatio = screenWidth / screenHeight;

        // 해상도와 가로세로 비율에 따라 리그의 반경을 설정합니다.
        float topRigRadius = CalculateRadius(aspectRatio, 50f);
        float middleRigRadius = CalculateRadius(aspectRatio, 50f);
        float bottomRigRadius = CalculateRadius(aspectRatio, 50f);

        // CinemachineFreeLook 카메라의 각 리그의 반경을 설정합니다.
        freeLookCam.m_Orbits[0].m_Radius = topRigRadius;
        freeLookCam.m_Orbits[1].m_Radius = middleRigRadius;
        freeLookCam.m_Orbits[2].m_Radius = bottomRigRadius;
    }

    private float CalculateRadius(float aspectRatio, float baseRadius)
    {
        // 가로세로 비율에 따라 반경을 조정하는 로직을 구현합니다.
        // 예시로, 가로세로 비율에 비례하여 반경을 조정합니다.
        return baseRadius * aspectRatio;
    }

    private void HandleTouchInput()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (RectTransformUtility.RectangleContainsScreenPoint(touchPanel, touch.position))
            {
                if (touch.phase == TouchPhase.Began)
                {
                    isDragging = true;
                    previousTouchPosition = touch.position;
                }
                else if (touch.phase == TouchPhase.Moved && isDragging)
                {
                    Vector2 touchDelta = touch.position - previousTouchPosition;
                    touchDelta.y = 0;

                    RotateCamera(touchDelta);
                    previousTouchPosition = touch.position;
                }
                else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
                {
                    isDragging = false;
                }
            }
        }
    }

    private void RotateCamera(Vector2 touchDelta)
    {
        float rotationSpeed = 0.1f; // 회전 속도 조정
        freeLookCam.m_XAxis.Value += touchDelta.x * rotationSpeed;
        freeLookCam.m_YAxis.Value -= touchDelta.y * rotationSpeed;
    }
}