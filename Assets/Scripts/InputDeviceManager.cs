using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputDeviceManager : MonoBehaviour
{
    public bool m_isKeyboardAndMouse = true;

    // Start is called before the first frame update
    void Start()
    {
        InputSystem.onActionChange += InputActionChangeCallback;
    }

    // Update is called once per frame
    private void InputActionChangeCallback(object obj, InputActionChange change)
    {
        if (change == InputActionChange.ActionPerformed)
        {
            InputAction receivedInputAction = (InputAction) obj;
            InputDevice lastDevice = receivedInputAction.activeControl.device;
            m_isKeyboardAndMouse = lastDevice.name.Equals("Keyboard") || lastDevice.name.Equals("Mouse");
            //If needed we could check for "XInputControllerWindows" or "DualShock4GamepadHID"
            //Maybe if it Contains "controller" could be xbox layout and "gamepad" sony? More investigation needed
        }
    }
}
