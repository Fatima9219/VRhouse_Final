using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using Valve.VR;

// BodyPart könnte wohl weg
public class IndexInput : MonoBehaviour
{
    public SteamVR_Action_Vector2 ThumbstickAction, TrackpadAction = null;

    public SteamVR_Action_Boolean trigger = SteamVR_Input.GetAction<SteamVR_Action_Boolean>("default", "Trigger");

    public SteamVR_Action_Single SqueezeAction = null;
    public SteamVR_Action_Boolean GripAction, ButtonAAction, ButtonBAction = null;

    public SteamVR_Action_Skeleton SkeletonAction = null;
    public SteamVR_Action_Vibration hapticAction;

    private Camera VRCamera;
    private CharacterController character;
    public GameObject BodyPart;
    private readonly float speed = 1.4f;

    private bool soundIsActive = true; //setting

    private GameObject Menu;
    private bool MenuIsActive = false;
    private GameObject Navigator;

    public GameObject forkliftJumpInSnapPosition;
    public GameObject forkliftJumpOutSnapPosition;
    public GameObject PlayerHigh;
    public GameObject ExitCollider;

    private bool PlayerEnteredCockpitMode = false;
    private bool gravityOn = true;



    private void Start()
    {
        VRCamera = GameObject.Find("VRCamera").GetComponent<Camera>();
        character = GetComponent<CharacterController>();

        //test
        capsuleCollider = GetComponent<CapsuleCollider>();


        //Menu = GameObject.Find("Canvas");
        //Menu.SetActive(false);
        //Navigator = GameObject.Find("Index_Knuckles_Pointer");
        //Navigator.SetActive(false);

        /// if in menu, do not allow to walk
    }

    private void Update()
    {
        if (!ControlRegister.ForkliftController)   // we need a handler instead of an update for such things..
        {
            if (PlayerEnteredCockpitMode)
            {
                PlayerEnteredCockpitMode = false;
                gravityOn = true;
                //StartCoroutine(CockpitMode(false, BodyPart.transform.position, forkliftJumpOutSnapPosition.transform.position, 1f));
            }
            // default actions activated here
            ButtonA();
            ButtonB();
            MenuView();

            ControlRegister.right_trigger = SteamVR_Input.GetAction<SteamVR_Action_Boolean>("default", "Trigger").state;
        }

        else
        {
            // we deactivate default actions and activate the CharacterController Animation once
            if (!PlayerEnteredCockpitMode)
            {
                PlayerEnteredCockpitMode = true;
                gravityOn = false;
                //StartCoroutine(CockpitMode(true, BodyPart.transform.position, forkliftJumpInSnapPosition.transform.position, 1f));
            }
        }

        ApplyGravity();
    }

    //test
    public Transform cameraTransform;
    private CapsuleCollider capsuleCollider;

    private void FixedUpdate()
    {
        //float distanceFromFlor = Vector3.Dot(cameraTransform.localPosition, Vector3.up);
        //capsuleCollider.height = Mathf.Max(capsuleCollider.radius, distanceFromFlor);
        //capsuleCollider.center = cameraTransform.localPosition - 0.5f * distanceFromFlor * Vector3.up;

        float distanceFromFlor = Vector3.Dot(cameraTransform.localPosition, Vector3.up);
        character.height = Mathf.Max(character.radius, distanceFromFlor);
        character.center = cameraTransform.localPosition - 0.5f * distanceFromFlor * Vector3.up;

        if (!ControlRegister.ForkliftController)
        {
            Vector3 direction = new Vector3(ThumbstickAction.axis.x, 0, ThumbstickAction.axis.y);
            character.Move(Quaternion.Euler(new Vector3(0, VRCamera.transform.eulerAngles.y, 0)) * direction * Time.fixedDeltaTime * speed);

            if (character.velocity.magnitude >= 0.2f)
                LogicStatusRegister.userOnMove = true;
            else
                LogicStatusRegister.userOnMove = false;
        }

        /*
        // steps sound
        if (soundIsActive) {
            if (character.isGrounded == true && character.velocity.magnitude > 1f && GetComponent<AudioSource>().isPlaying == false) {
                GetComponent<AudioSource>().volume = Random.Range(0.8f, 1.0f);
                GetComponent<AudioSource>().pitch  = Random.Range(0.8f, 1.0f);
                GetComponent<AudioSource>().Play();
            }
        }*/
    }

    private void ApplyGravity()
    {
        if (gravityOn)
        {
            Vector3 gravity = new Vector3(0, Physics.gravity.y * 9.81f, 0);
            gravity.y *= Time.deltaTime;

            character.Move(gravity * Time.deltaTime);
        }
    }

    private void ButtonB()
    {
        if (ButtonBAction.stateDown)
        {
            if (!MenuIsActive)
            {
                Vector3 spawnPos = VRCamera.transform.position + (VRCamera.transform.forward * 2f);
                spawnPos.y = 1.5f;
                Menu.transform.position = spawnPos;

                Quaternion tempRotation = VRCamera.transform.rotation;
                tempRotation.x = 0;
                tempRotation.z = 0;
                Menu.transform.rotation = tempRotation;

                Menu.SetActive(true);
                Navigator.SetActive(true);

                MenuIsActive = true;
            }
            else
            {
                Menu.SetActive(false);
                Navigator.SetActive(false);
                MenuIsActive = false;
            }
        }
    }

    //IEnumerator CockpitMode(bool enable, Vector3 pos1, Vector3 pos2, float duration)
    //{
    //    Vector3 ftarget = character.transform.position + (pos2 - new Vector3(pos1.x, 0, pos1.z));

    //    character.enabled = false;
    //    for (float t = 0f; t < duration; t += Time.deltaTime)
    //    {
    //        Vibration(0.1f, 5, 4);
    //        transform.position = Vector3.Lerp(character.transform.position, enable
    //                             ? new Vector3(ftarget.x, ftarget.y + 0.25f, ftarget.z)
    //                             : new Vector3(ftarget.x, ftarget.y - 0.25f, ftarget.z), t / duration);
    //        yield return 0;
    //    }
    //    character.enabled = true;

    //    // TODO IMPORTANT!!!!!!!!!!!!!
    //    // activate ExitCollider, set to true, deactivate body collider?
    //    // make handle available again
    //}

    private void ButtonA()
    { // no specific functionality for now
        if (ButtonAAction.stateDown)
        {
            //Vibration(1f, 5, 4);
            //ScenesTransitioner.Load("Menu", "Results", "true");
        }
    }

    private void MenuView()
    { // breakout
        if (ButtonAAction.stateDown && SteamVR_Input.GetAction<SteamVR_Action_Boolean>("default", "Trigger").state)
            LoadScene(1);
    }

    private void LoadScene(int lv)
    {
        StartCoroutine(FadeToNextLevel(lv));
    }
    // ToDo check the timings to avoid the frame problem
    private IEnumerator FadeToNextLevel(int lv)
    {
        SteamVR_Fade.View(Color.black, 2f);
        yield return new WaitForSeconds(1.8f);
        SceneManager.LoadScene(lv);
        SteamVR_Fade.View(Color.clear, 4f);
        yield return new WaitForSeconds(3.8f);
    }

    private void Vibration(float duration, float frequency, float amplitude)
    {
        hapticAction.Execute(0, duration, frequency, amplitude, SteamVR_Input_Sources.Any);
    }
}