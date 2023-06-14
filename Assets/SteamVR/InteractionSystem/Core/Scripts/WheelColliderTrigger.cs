using UnityEngine;

public class WheelColliderTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.CompareTag("Terminalbase"))
            LogicStatusRegister.PickingCartWheels[System.Int16.Parse(transform.parent.name.Substring(1, 1)) - 1] = true;
    }

    private void OnTriggerExit(Collider other) {
        if (other.gameObject.CompareTag("Terminalbase"))
            LogicStatusRegister.PickingCartWheels[System.Int16.Parse(transform.parent.name.Substring(1, 1)) - 1] = false;
    }
}