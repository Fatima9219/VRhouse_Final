// PickerCartPhysics

using UnityEngine;

// namespace auf eigenen wechseln!!! (Interactable namesänderung usw.)
namespace Valve.VR.InteractionSystem
{
	[RequireComponent(typeof(Interactable))]
	public class PickerCart : MonoBehaviour
	{
		public SteamVR_Action_Vector2 ThumbstickAction, TrackpadAction = null;
		public SteamVR_Action_Single SqueezeAction = null;
		public SteamVR_Action_Boolean GripAction, ButtonAAction, ButtonBAction = null;
		public SteamVR_Action_Skeleton SkeletonAction = null;
		private Camera VRCamera;
		private CharacterController character;
		private readonly float speed = 1.4f;
		private readonly float gravityMultiplier = 9.81f;
		private bool soundIsActive = true; //setting
		private GameObject Menu;
		private bool MenuIsActive = false;
		private GameObject Navigator;

		private void ButtonA()
		{
			if (ButtonAAction.stateDown) { }
		}

		private void ButtonB()
		{
			if (ButtonBAction.stateDown) { }
		}

		GameObject Avatar;

		private void AvatarMove()
		{
			//right hand, left hand bestimmung
			Vector3 direction = new Vector3(ThumbstickAction.axis.x, 0, ThumbstickAction.axis.y);
			character.Move(Quaternion.Euler(new Vector3(0, VRCamera.transform.eulerAngles.y, 0)) * direction * Time.fixedDeltaTime * speed);
			// calculate collider
			float distanceFromFloor = Vector3.Dot(VRCamera.transform.localPosition, Vector3.up);
			character.height = Mathf.Max(character.radius, distanceFromFloor);
			character.center = VRCamera.transform.localPosition - 0.5f * distanceFromFloor * Vector3.up;
		}

		void ApplyGravity()
		{
			Vector3 gravity = new Vector3(0, Physics.gravity.y * gravityMultiplier, 0);
			gravity.y *= Time.deltaTime;

			character.Move(gravity * Time.deltaTime);
		}


		public bool repositionGameObject = true; // soll nur gegriffen oder auch repositionierbar sein?

		protected Hand.AttachmentFlags attachmentFlags = Hand.AttachmentFlags.DetachFromOtherHand; // Art des Attachment. Viele Varianten vorhanden!

		protected Interactable interactable;

		GameObject[] Wheel = new GameObject[4];
        readonly GameObject[] PTL_BulbLED = new GameObject[6];
		private GameObject Tablet, Tablet_Display;
		private Material tablet_material;
		public Material tablet_LED_material;

		public float value = 0f;

		public bool on = true;

		private void Start()
        {
			for (int i=0; i!=4; i++)
				Wheel[i] = transform.GetChild(0).gameObject.transform.GetChild(0).gameObject.transform.GetChild(2).gameObject.transform.GetChild(i).gameObject;

			for (int i=0; i!=6; i++)
				PTL_BulbLED[i] = transform.GetChild(0).gameObject.transform.GetChild(1).gameObject.transform.GetChild(3+i).gameObject.transform.GetChild(4).gameObject.transform.GetChild(0).gameObject;

			Tablet = new GameObject();
			Tablet = transform.GetChild(0).gameObject.transform.GetChild(1).gameObject.transform.GetChild(2).gameObject;
			tablet_material = Tablet.GetComponent<Renderer>().material;

			Tablet_Display = new GameObject();
			Tablet_Display = transform.GetChild(0).gameObject.transform.GetChild(1).gameObject.transform.GetChild(2).gameObject.transform.GetChild(0).gameObject;

			/*var mats = Tablet.GetComponent<Renderer>().materials;
			mats[1] = tablet_LED_material;
			Tablet.GetComponent<Renderer>().materials = mats;
			*/
		}

		
		protected virtual void Awake()
		{
			//Avatar = GameObject.Find("Player");
			VRCamera = GameObject.Find("VRCamera").GetComponent<Camera>();
			character = GameObject.Find("Player").GetComponent<CharacterController>();
			interactable = GetComponent<Interactable>();
		}

		private void Update()
		{
			if (LogicStatusRegister.pickerCartGrabbed) {}
		}

		protected virtual void HandHoverUpdate(Hand hand)   //Aufruf wenn die hand das Objekt berührt
		{
			GrabTypes startingGrabType = hand.GetGrabStarting();

			if (interactable.attachedToHand == null && startingGrabType != GrabTypes.None)
			{
				hand.AttachObject(gameObject, startingGrabType, attachmentFlags); // der eigentliche Aufruf des Greifens   gameObject = handle
			}
		}

		protected virtual void HandAttachedUpdate(Hand hand)
		{
			UpdateLinearMapping(hand.transform);   // while holding

			if (Vector3.Distance(hand.transform.position, transform.position) > 0.2f) // wenn sich die Hand zu weit entfernt  // distanceTolerance
				hand.DetachObject(gameObject);

			if (hand.IsGrabEnding(gameObject))
			{ // beim beenden des haltens
				hand.DetachObject(gameObject);  //loslassen des Objekts
			}
		}

		// Diese beiden Funktionen bestimmen die richtung
		protected void UpdateLinearMapping(Transform handObjTransform)
		{
			CalculateWheelRotation();

			//float currentVelocity = 0; // variablen hier oder auserhalb?
			//float targetVelocity = 20;

			float offsetFix = 45;
			if (repositionGameObject)
			{
				Vector3 targetPoint = new Vector3(handObjTransform.position.x, transform.position.y, handObjTransform.position.z);

				// interessante alternative, acceleration rotation fehlt!
				//currentVelocity = (Mathf.Lerp(currentVelocity, targetVelocity, Time.deltaTime ));
				//transform.position += transform.forward * currentVelocity * Time.deltaTime;   // transform forward

				transform.position = Vector3.Lerp(gameObject.transform.position, targetPoint, Time.time);  // standard
				transform.eulerAngles = new Vector3(0, handObjTransform.eulerAngles.y + offsetFix, 0);
			}
		}

		Vector3 previous;
		float velocity;
		Vector3[] prevLoc = new Vector3[4];
		protected void CalculateWheelRotation() {
			velocity = ((transform.position - previous).magnitude) / Time.deltaTime;
			previous = transform.position;

			if (velocity > .02) {
				for (int i = 0; i != 4; i++)
				{
					Vector3 curVel = -(Wheel[i].transform.position - prevLoc[i]) / Time.deltaTime;
					Wheel[i].transform.rotation = Quaternion.Slerp(Wheel[i].transform.rotation, Quaternion.LookRotation(curVel), Time.deltaTime * 500f);
					prevLoc[i] = Wheel[i].transform.position;
				}
			}
		}

		//DELETE
		private void TabletStatus(bool on) {
			if (Time.fixedTime % 0.5 < .2) {
				tablet_material.SetColor("_EmissiveColor", Color.black);
				Tablet_Display.SetActive(false);

				tablet_LED_material.SetColor("_EmissiveColor", Color.red);
			}
			else {
				//Tablet on/off
				tablet_material.SetColor("_EmissiveColor", Color.white);
				tablet_material.SetFloat("_EmissiveIntensity", 20f);
				tablet_material.SetFloat("_EmissiveExposureWeight", 0.5f);

				//Display_content visible/invisible
				Tablet_Display.SetActive(true);

				//PowerLED on/off
				tablet_LED_material.SetColor("_EmissiveColor", Color.green);
			}
		}
	}
}