using UnityEngine;

namespace Valve.VR.InteractionSystem.Sample
{
    public class ARGlasses : MonoBehaviour
    {
        public Collider targetCollider;
        public Rigidbody _rigidbody;
        public GameObject BlinkElement;
        private void Awake() {
            _rigidbody = GetComponent<Rigidbody>();
        }
        private void Start() {}

        private void OnCollisionEnter(Collision collision) {
            if (collision.collider == targetCollider)  {
                _rigidbody.useGravity = false;
                transform.parent = GameObject.Find("FollowHead").transform;
            }
        }
        private void FixedUpdate() {
            if (ControlRegister.PickByVision_IsActive) {
                transform.GetChild(0).gameObject.SetActive(true);
                transform.GetChild(1).gameObject.SetActive(true);
            }
            else {
                transform.GetChild(0).gameObject.SetActive(false);
                transform.GetChild(1).gameObject.SetActive(false);
            } 
        }
    }
}