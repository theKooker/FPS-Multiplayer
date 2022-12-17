using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleGate : MonoBehaviour, IGate
{
    [SerializeField, Range(0f,1f)] private float doorPosition = 0;// 0: closed, 1: open

    [SerializeField] private Transform openPosition;
    [SerializeField] private Transform closedPosition;

    [SerializeField] private bool opening = false;//Not opening means closing
    [SerializeField] private float speed;
    
    public void SetOpening(bool opening) {
        this.opening = opening;
    }

    private void Update() {
        if (opening)
            doorPosition += speed * Time.deltaTime;
        else
            doorPosition -= speed * Time.deltaTime;
        doorPosition = Mathf.Clamp(doorPosition, 0f, 1f);

        UpdateDoorPosition();
    }

    public bool IsClosed() {
        return doorPosition == 0f;
    }
    public bool IsOpen() {
        return doorPosition == 1f;
    }

    private void UpdateDoorPosition() {
        transform.position = Vector3.Lerp(closedPosition.position, openPosition.position, doorPosition);
    }

    private void OnValidate() {
        if (openPosition != null && closedPosition != null)
            UpdateDoorPosition();
    }
}
