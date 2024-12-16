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
        // ���� �ػ󵵿� ���μ��� ������ �����ɴϴ�.
        float screenWidth = Screen.width;
        float screenHeight = Screen.height;
        float aspectRatio = screenWidth / screenHeight;

        // �ػ󵵿� ���μ��� ������ ���� ������ �ݰ��� �����մϴ�.
        float topRigRadius = CalculateRadius(aspectRatio, 50f);
        float middleRigRadius = CalculateRadius(aspectRatio, 50f);
        float bottomRigRadius = CalculateRadius(aspectRatio, 50f);

        // CinemachineFreeLook ī�޶��� �� ������ �ݰ��� �����մϴ�.
        freeLookCam.m_Orbits[0].m_Radius = topRigRadius;
        freeLookCam.m_Orbits[1].m_Radius = middleRigRadius;
        freeLookCam.m_Orbits[2].m_Radius = bottomRigRadius;
    }

    private float CalculateRadius(float aspectRatio, float baseRadius)
    {
        // ���μ��� ������ ���� �ݰ��� �����ϴ� ������ �����մϴ�.
        // ���÷�, ���μ��� ������ ����Ͽ� �ݰ��� �����մϴ�.
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
        float rotationSpeed = 0.1f; // ȸ�� �ӵ� ����
        freeLookCam.m_XAxis.Value += touchDelta.x * rotationSpeed;
        freeLookCam.m_YAxis.Value -= touchDelta.y * rotationSpeed;
    }
}