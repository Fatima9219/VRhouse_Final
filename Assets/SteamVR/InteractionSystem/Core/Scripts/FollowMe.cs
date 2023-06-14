using UnityEngine;

public class FollowMe : MonoBehaviour
{
    private Camera VRCamera;
    private Vector3 offset;
    void Start() {
        VRCamera = GameObject.Find("VRCamera").GetComponent<Camera>();
        offset = transform.position - VRCamera.transform.position;
    }

    void FixedUpdate() {
        transform.position = VRCamera.transform.position + offset;
    }
}