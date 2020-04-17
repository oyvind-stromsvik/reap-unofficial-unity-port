using UnityEngine;

public class InputController : MonoBehaviour {

    // The deadzone for the joysticks and analog triggers.
    public float joystick_deadzone = 0.3f;

    // Whether or not the left or right hand buttons are held down.
    [HideInInspector]
    public bool _A;
    [HideInInspector]
    public bool _B;

    // Horizontal and vertical input.
    [HideInInspector]
    public int _h = 0;
    [HideInInspector]
    public int _v = 0;

    void Update() {
        // _A is defined as the X-key, N-key, 
        _A = Input.GetKey(KeyCode.X) || Input.GetKey(KeyCode.N) || Input.GetKey(KeyCode.Joystick1Button0) || (Input.GetAxisRaw("LeftTrigger") > joystick_deadzone);
        _B = Input.GetKey(KeyCode.C) || Input.GetKey(KeyCode.M) || Input.GetKey(KeyCode.Joystick1Button1) || (Input.GetAxisRaw("RightTrigger") > joystick_deadzone);

        // With GetAxisRaw we get either -1, 0 or 1 from keyboards, but analog
        // joysticks still return a float between -1 and 1 depending on how hard
        // you press the stick. This is just converting that to integer values
        // with sort of a deadzone approach.
        if (Input.GetAxisRaw("Horizontal") < -joystick_deadzone) {
            _h = -1;
        }
        else if (Input.GetAxisRaw("Horizontal") > joystick_deadzone) {
            _h = 1;
        }
        else {
            _h = 0;
        }
        if (Input.GetAxisRaw("Vertical") < -joystick_deadzone) {
            _v = -1;
        }
        else if (Input.GetAxisRaw("Vertical") > joystick_deadzone) {
            _v = 1;
        }
        else {
            _v = 0;
        }
    }
}
