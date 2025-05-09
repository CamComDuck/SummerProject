using System;
using UnityEngine;
using UnityEngine.InputSystem.Composites;

public class GameInput : MonoBehaviour {
    
    // public static event EventHandler OnCameraPanPerformed;
    // public static event EventHandler OnCameraZoomPerformed;
    // public static event EventHandler OnCameraRotateTogglePerformed;
    public static event EventHandler OnCameraRotatePlacedPerformed;
    public static event EventHandler OnClickPerformed;

    private static InputSystem inputSystem;

    private static bool isRotateToggled = false;

    private void Awake() {
        inputSystem = new InputSystem();
        inputSystem.Camera.Enable();

        // inputSystem.Camera.Pan.performed += Pan_performed;
        // inputSystem.Camera.Zoom.performed += Zoom_performed;
        inputSystem.Camera.RotateToggle.performed += RotateToggle_performed;
        inputSystem.Camera.RotateToggle.canceled += RotateToggle_canceled;
        inputSystem.Camera.RotatePlace.performed += RotatePlaced_performed;
        inputSystem.Camera.Click.performed += Click_performed;
    }

    public static Vector2 GetPanVectorNormalized() {
        Vector2 panVector = inputSystem.Camera.Pan.ReadValue<Vector2>();
        return panVector.normalized;
    }

    public static float GetZoomValue() {
        float zoomValue = inputSystem.Camera.Zoom.ReadValue<float>();
        return zoomValue;
    }

    public static bool IsRotateToggled() {
        return isRotateToggled;
    }

    // private void Pan_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj) {
    //     OnCameraPanPerformed?.Invoke(this, EventArgs.Empty);
    // }

    private void Zoom_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj) {
        // OnCameraZoomPerformed?.Invoke(this, EventArgs.Empty);
    }

    private void RotateToggle_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj) {
        isRotateToggled = true;
        // OnCameraRotateTogglePerformed?.Invoke(this, EventArgs.Empty);
    }

    private void RotateToggle_canceled(UnityEngine.InputSystem.InputAction.CallbackContext obj) {
        isRotateToggled = false;
        // OnCameraRotateTogglePerformed?.Invoke(this, EventArgs.Empty);
    }

    private void RotatePlaced_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj) {
        OnCameraRotatePlacedPerformed?.Invoke(this, EventArgs.Empty);
    }

    private void Click_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj) {
        OnClickPerformed?.Invoke(this, EventArgs.Empty);
    }
}
