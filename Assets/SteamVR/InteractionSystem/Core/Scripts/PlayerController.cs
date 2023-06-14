using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;

public class PlayerController : MonoBehaviour
{

    public SteamVR_Action_Vector2 touchpadInput;

    public Transform cameraTransform;
    private CapsuleCollider capsuleCollider;

    void Start()
    {
        capsuleCollider = GetComponent<CapsuleCollider>();
    }

    void FixedUpdate()
    {
        Vector3 movementDir = Player.instance.hmdTransform.TransformDirection(new Vector3(touchpadInput.axis.x, 0, touchpadInput.axis.y));
        transform.position += (Vector3.ProjectOnPlane(Time.deltaTime * movementDir * 3.0f, Vector3.up));

        float distanceFromFlor = Vector3.Dot(cameraTransform.localPosition, Vector3.up);
        capsuleCollider.height = Mathf.Max(capsuleCollider.radius, distanceFromFlor);

        capsuleCollider.center = cameraTransform.localPosition - 0.5f * distanceFromFlor * Vector3.up;
    }
}