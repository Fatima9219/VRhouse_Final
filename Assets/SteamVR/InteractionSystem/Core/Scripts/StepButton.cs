using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Valve.VR.InteractionSystem;

// Switch between M.Points | It's and absolute QND and have to be rewritten

[RequireComponent(typeof(Interactable))]
public class StepButton : MonoBehaviour
{
    public Transform movingPart;

    public Vector3 localMoveDistance = new Vector3(0, -0.1f, 0);

    [Range(0, 1)]
    public float engageAtPercent = 0.95f;

    [Range(0, 1)]
    public float disengageAtPercent = 0.9f;

    public HandEvent onButtonDown;
    public HandEvent onButtonUp;
    public HandEvent onButtonIsPressed;

    public bool engaged = false;
    public bool buttonDown = false;
    public bool buttonUp = false;

    private Vector3 startPosition;
    private Vector3 endPosition;

    private Vector3 handEnteredPosition;

    private bool hovering;

    private Hand lastHoveredHand;

    public GameObject mPoint_0, mPoint_1, mPoint_2, mPoint_3;

    public GameObject[] mPointArray;
    public GameObject frame;
    public GameObject correct_sign;
    public GameObject incorrect_sign;
    public GameObject pick_mPoint;

    public GameObject actualPositionText;
    private void Start()
    {
        mPointArray = new GameObject[5];

        if (movingPart == null && this.transform.childCount > 0)
            movingPart = this.transform.GetChild(0);

        startPosition = movingPart.localPosition;
        endPosition = startPosition + localMoveDistance;
        handEnteredPosition = endPosition;

        mPointArray[0] = mPoint_0.gameObject;
        mPointArray[1] = mPoint_1.gameObject;
        mPointArray[2] = mPoint_2.gameObject;
        mPointArray[3] = mPoint_3.gameObject;
    }
    private void HandHoverUpdate(Hand hand)
    {
        hovering = true;
        lastHoveredHand = hand;

        bool wasEngaged = engaged;

        float currentDistance = Vector3.Distance(movingPart.parent.InverseTransformPoint(hand.transform.position), endPosition);
        float enteredDistance = Vector3.Distance(handEnteredPosition, endPosition);

        if (currentDistance > enteredDistance)
        {
            enteredDistance = currentDistance;
            handEnteredPosition = movingPart.parent.InverseTransformPoint(hand.transform.position);
        }

        float distanceDifference = enteredDistance - currentDistance;

        float lerp = Mathf.InverseLerp(0, localMoveDistance.magnitude, distanceDifference);

        if (lerp > engageAtPercent)
            engaged = true;
        else if (lerp < disengageAtPercent)
            engaged = false;

        movingPart.localPosition = Vector3.Lerp(startPosition, endPosition, lerp);

        InvokeEvents(wasEngaged, engaged);
    }
    private void LateUpdate()
    {
        if (hovering == false)
        {
            movingPart.localPosition = startPosition;
            handEnteredPosition = endPosition;

            InvokeEvents(engaged, false);
            engaged = false;
        }

        hovering = false;
    }
    private void InvokeEvents(bool wasEngaged, bool isEngaged)
    {
        buttonDown = wasEngaged == false && isEngaged == true;
        buttonUp = wasEngaged == true && isEngaged == false;

        if (buttonDown && onButtonDown != null)
        {
            onButtonDown.Invoke(lastHoveredHand);

            if (transform.gameObject.name.Equals("Step-Button") && !ControlRegister.right_trigger)
                nextMPoint();

            if (transform.gameObject.name.Equals("StepBack-Button") && !ControlRegister.right_trigger)
                prevMPoint();

            if (transform.gameObject.name.Equals("Step-Button") && ControlRegister.right_trigger)
                scanModus();
        }
        if (buttonUp && onButtonUp != null)
            onButtonUp.Invoke(lastHoveredHand);

        if (isEngaged && onButtonIsPressed != null)
            onButtonIsPressed.Invoke(lastHoveredHand);
    }

    private bool loadingcompleted = false;
    private bool allowedToStep = true;
    private bool allowedToStepBack = true;
    private bool bootingStarted = false;
    private bool onPicking = false;
    private bool onTraveling = false;

    // have to be replaced with the real list here! -> level/Pick-By-Light
    private int actualPos = 0;
    string[] pos = new string[] { "ZP.A.02.3.08", "ZP.B.03.3.08", "ZP.C.03.2.06", "ZP.D.04.3.04"}; // !!! -> send number to ScanColliderPBVi to compare with!
    string[] quant = new string[] { "1", "3", "2", "6"};

    // logic is an QND just to see what direction it goes, so have to be fully replaced/rewritten
    private void nextMPoint() {
        if (allowedToStep) {
            if (!LogicStatusRegister.bootingPBVicompleted) {
                if (!loadingcompleted && !bootingStarted) {
                    allowedToStep = false;
                    bootingStarted = true;
                    StartCoroutine(LoadingScreen());
                }
            }

            if (LogicStatusRegister.bootingPBVicompleted) {
                if (ControlRegister.actualMPoint != mPointArray.Length - 2) {  // check if not pickFrame and/or if correct scanned
                    mPointArray[ControlRegister.actualMPoint].SetActive(false);
                    mPointArray[ControlRegister.actualMPoint + 1].SetActive(true);
                    ControlRegister.actualMPoint++;
                }
            }

            if (onPicking) {
                //TODO
            }
        }
    }
    private void prevMPoint() {
        Debug.Log(allowedToStepBack);
        if (allowedToStepBack) {
            if (ControlRegister.actualMPoint > 2)
            {
                mPointArray[ControlRegister.actualMPoint].SetActive(false);
                mPointArray[ControlRegister.actualMPoint - 1].SetActive(true);
                ControlRegister.actualMPoint--;
            }
        }
    }

    private bool scanModeOn = false;
    private void scanModus() {
        if (ControlRegister.actualMPoint == 3) {
            scanModeOn = true;
            frame.SetActive(true);
        }
    }
    private void Update()
    {
        if (LogicStatusRegister.bootingPBVicompleted) {
            if (LogicStatusRegister.pickerCartGrabbed && LogicStatusRegister.userOnMove)
                CommissioningTime.travelTimeIsRunning = true;
            else
                CommissioningTime.travelTimeIsRunning = false;
        }

        if (scanModeOn && LogicStatusRegister.scannedNumber) { //we are in scan mode and waiting for scannedNumber here
            checkNumber();
        }
    }
    private void checkNumber() {
        switch (LogicStatusRegister.correctNumberScanned)
        {
            case true:
                //StartCoroutine(switchToPick(true));
                mPoint_3.SetActive(false);
                pick_mPoint.SetActive(true);
                allowedToStepBack = false;
                break;
            case false:
                //StartCoroutine(switchToPick(false));
                break;
        }
    }
    private IEnumerator switchToPick(bool val)
    {
        if (val)
        {
            correct_sign.SetActive(true);
            incorrect_sign.SetActive(false);
            yield return new WaitForSeconds(3f);
            correct_sign.SetActive(false);
            incorrect_sign.SetActive(false);
        }
        else {
            correct_sign.SetActive(false);
            incorrect_sign.SetActive(true);
            yield return new WaitForSeconds(3f);
            correct_sign.SetActive(false);
            incorrect_sign.SetActive(false);
        }
    }
    private IEnumerator LoadingScreen()
    {
        mPointArray[ControlRegister.actualMPoint].SetActive(false);
        mPointArray[ControlRegister.actualMPoint+1].SetActive(true);
        yield return new WaitForSeconds(3f);
        mPointArray[1].SetActive(false);
        mPointArray[2].SetActive(true);
        actualPositionText.GetComponent<Text>().text = pos[actualPos];
        actualPos++;
        onPicking = true;
        ControlRegister.actualMPoint = 2;
        loadingcompleted = true;
        LogicStatusRegister.bootingPBVicompleted = true;
        CommissioningTime.basisTimeIsRunning = false;
        allowedToStep = true;
    }
}