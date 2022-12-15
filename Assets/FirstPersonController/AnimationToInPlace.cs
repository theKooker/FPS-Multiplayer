using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationToInPlace : MonoBehaviour
{
    [SerializeField] private Transform root;
    private Vector3 _position;

    private void Awake() {
        _position = root.localPosition;
    }

    private void LateUpdate() {
        // TODO maybe also override rotation
        Vector3 newPos = _position;
        _position.y = root.localPosition.y;// Only lock x and z axis
        root.localPosition = _position;
    }
}
