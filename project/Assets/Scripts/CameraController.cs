using UnityEngine;

public class CameraController : MonoBehaviour {

    private const float moveSpeed = 10f;

    private void Update() {
        HandleMovement();
    }

    private void HandleMovement() {
        Vector2 panVector = GameInput.GetPanVectorNormalized();
        float zoomValue = GameInput.GetZoomValue();
        Vector3 direction = new Vector3(panVector.x, zoomValue, panVector.y);
        float distance = moveSpeed * Time.deltaTime;
        Vector3 targetPosition = transform.position + direction;

        if (GameInput.IsRotateToggled()) {
            Vector3 currentRotation = transform.rotation.eulerAngles;
            if (panVector.x != 0) {
                transform.rotation = Quaternion.Euler(currentRotation.x, panVector.x + currentRotation.y, currentRotation.z);
                
            } else {
                transform.rotation = Quaternion.Euler(currentRotation.x, panVector.y + currentRotation.y, currentRotation.z);
            }
            
        } else {
            transform.position = Vector3.Slerp(transform.position, targetPosition, distance);
        }
    }
}
