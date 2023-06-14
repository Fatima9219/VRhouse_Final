using UnityEngine;

namespace Valve.VR.InteractionSystem.Sample
{
    public class LockToPoint : MonoBehaviour
    {
        public Transform snapTo;
        private Rigidbody body;
        public float snapTime = 2;
        public float snapDistance = 0.2f;
        private float dropTimer;
        private Interactable interactable;

        private void Start()
        {
            interactable = GetComponent<Interactable>();
            body = GetComponent<Rigidbody>();
        }

        private void FixedUpdate()
        {
            bool used = false;
            if (interactable != null)
                used = interactable.attachedToHand;

            if (used)
            {
                body.isKinematic = false;
                dropTimer = -1;

                if (transform.name.Equals("PickByVoice"))
                {
                    transform.GetComponent<Animation>().Rewind();
                    transform.GetComponent<Animation>().Play();
                    transform.GetComponent<Animation>().Sample();
                    transform.GetComponent<Animation>().Stop();
                }
            }
            else // element released. Check the distance.
            {
                if ((Vector3.Distance(snapTo.position, transform.position)) <= snapDistance) // distanz passt, attach to parent
                {
                    dropTimer += Time.deltaTime / (snapTime / 2);

                    body.isKinematic = dropTimer > 1;

                    if (dropTimer > 1)
                    {
                        //transform.parent = snapTo;
                        transform.position = snapTo.position;
                        transform.rotation = snapTo.rotation;
                    }
                    else
                    {
                        float t = Mathf.Pow(35, dropTimer);

                        body.velocity = Vector3.Lerp(body.velocity, Vector3.zero, Time.fixedDeltaTime * 4);
                        if (body.useGravity)
                            body.AddForce(-Physics.gravity);

                        transform.position = Vector3.Lerp(transform.position, snapTo.position, Time.fixedDeltaTime * t * 3);
                        transform.rotation = Quaternion.Slerp(transform.rotation, snapTo.rotation, Time.fixedDeltaTime * t * 2);

                        if (transform.name.Equals("PickByVoice"))
                        {
                            transform.GetComponent<Animation>().Play();
                        }

                        if (transform.name.Equals("PickByVision"))
                        {
                            Debug.Log("PickByVision is active");

                        }
                    }

                    transform.SetParent(snapTo);
                }
                else
                {
                    transform.parent = null;

                    if (transform.name.Equals("PickByVision"))
                    {
                        Debug.Log("PickByVision is NOT active");
                    }
                }
            }
        }
    }
}