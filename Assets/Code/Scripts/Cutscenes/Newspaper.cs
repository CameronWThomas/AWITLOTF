using System;
using System.Collections;
using UnityEngine;

public class Newspaper : MonoBehaviour
{
    public Camera NewspaperCamera;
    public Transform NewsPaperModel;
    [Range(0f, 10f)] public float CameraDistance = .5f;
    [Range(0, 10)] public int Rotations = 3;

    private void Start()
    {
        SetPositionAndRotation(0f);
    }

    public void Hide()
    {
        NewsPaperModel.gameObject.SetActive(false);
    }

    public void Show()
    {
        NewsPaperModel.gameObject.SetActive(true);
    }

    public void SetPositionAndRotation(float t)
    {
        var position = GetPosition(t);
        var rotation = GetRotation(t);

        NewsPaperModel.SetPositionAndRotation(position, rotation);
    }

    public IEnumerator WaitForInputRoutine()
    {
        while (true)
        {
            if (Input.GetKeyDown(KeyCode.Space))
                break;

            yield return null;
        }
    }

    private Vector3 GetPosition(float t)
    {
        var cameraTransform = NewspaperCamera.transform;

        var stopPointDistance = 10f;
        var stopPointT = .5f;

        var startPosition = transform.position;
        var stopPoint = cameraTransform.position + cameraTransform.forward * stopPointDistance;
        var endPosition = cameraTransform.position + cameraTransform.forward * CameraDistance;

        t = Mathf.Clamp01(t);
        t = Mathf.SmoothStep(0f, 1f, t); // Make t smooth

        if (t < stopPointT)
        {
            t = t / stopPointT;
            return Vector3.Lerp(transform.position, stopPoint, t);
        }

        t = (t - stopPointT) / (1f - stopPointT);
        return Vector3.Lerp(stopPoint, endPosition, t);
    }

    private Quaternion GetRotation(float t)
    {
        var angle = Mathf.Lerp(0f, Rotations * 360f, t);
        return Quaternion.AngleAxis(angle, transform.forward);
    }
}
