using UnityEngine;
using UnityEngine.EventSystems;

public class Pointer : MonoBehaviour
{
    private float m_DefaultLength = 20.0f;
    private GameObject laserDot;
    private GameObject laserBeam;
    public VRInputModule m_InputModule;

    private void Start() {
        laserBeam = GameObject.Find("LaserBeam");
        laserDot = GameObject.Find("LaserDot");
    }

    void Update() {
        UpdateLaserBeam();
    }

    private void UpdateLaserBeam() {
        //Use default or distance
        PointerEventData data = m_InputModule.GetData();
        float targetLength = data.pointerCurrentRaycast.distance == 0 ? m_DefaultLength : data.pointerCurrentRaycast.distance;

        // Raycast
        RaycastHit hit = CreateRaycast();

        // Default
        Vector3 endPosition = transform.position + (transform.forward * targetLength);

        // Or based on hit
        if (hit.collider != null)
            endPosition = hit.point;

        // Set position of the dot
        laserDot.transform.position = endPosition;

        float dist = Vector3.Distance(endPosition, transform.position);

        laserBeam.transform.localScale = new Vector3(0.4f, 0.4f, dist*1000);
    }

    private RaycastHit CreateRaycast() {
        Ray ray = new Ray(transform.position, transform.forward);
        Physics.Raycast(ray, out RaycastHit hit, m_DefaultLength);
        return hit;
    }
}