using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockMoving : MonoBehaviour {
    [SerializeField]
    private readonly float distance = 5f;
    private Vector3 destination;
    private Vector3 startPosition;
    private Vector3 direction;
    private readonly float stepDistance = 0.1f;

    private void Start() {
        startPosition = transform.position;
        destination = transform.position;
        destination.x += distance;
        direction = (destination - startPosition).normalized;
    }
    void FixedUpdate() {
        transform.Translate(direction * stepDistance);
        if (Vector3.Distance(transform.position, destination) <= stepDistance) {
            transform.position = destination;
            destination = startPosition;
            startPosition = transform.position;
            direction = (destination - startPosition).normalized;
        }
    }
}
