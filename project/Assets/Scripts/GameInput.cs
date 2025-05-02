using System;
using UnityEngine;
using UnityEngine.InputSystem.Composites;

public class GameInput : MonoBehaviour {
    
    // public static event EventHandler OnCameraPanPerformed;
    // public static event EventHandler OnCameraZoomPerformed;
    public static event EventHandler OnCameraRotateTogglePerformed;

    private static InputSystem inputSystem;

    private static bool isRotateToggled = false;

    private void Awake() {
        inputSystem = new InputSystem();
        inputSystem.Camera.Enable();

        // inputSystem.Camera.Pan.performed += Pan_performed;
        // inputSystem.Camera.Zoom.performed += Zoom_performed;
        inputSystem.Camera.RotateToggle.performed += RotateToggle_performed;
        inputSystem.Camera.RotateToggle.canceled += RotateToggle_canceled;
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
        Debug.Log(isRotateToggled);
        // OnCameraRotateTogglePerformed?.Invoke(this, EventArgs.Empty);
    }

    private void RotateToggle_canceled(UnityEngine.InputSystem.InputAction.CallbackContext obj) {
        isRotateToggled = false;
        Debug.Log(isRotateToggled);
        // OnCameraRotateTogglePerformed?.Invoke(this, EventArgs.Empty);
    }
}
