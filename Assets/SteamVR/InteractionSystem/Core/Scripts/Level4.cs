/*
    Pick-By-Voice
 */

using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows.Speech;
using System.Collections;

// If background music is on, the recognition doesn't work.
public class Level4 : MonoBehaviour
{
    private Dictionary<string, Action> keywordActions = new Dictionary<string, Action>();

    private KeywordRecognizer keywordRecognizer;

    private AudioSource audioSource;

    private AudioClip aisleAC, bayAC, binAC, confirmAC, loginAC, noInputDetectedAC, orderAC, orderFinishedAC, pieceAC, piecesAC, positionAC, positionsAC, rackAC, shelfAC, subsetAC, subsetConfirmAC, withAC, zoneAC,
                            A_AC, B_AC, C_AC, D_AC, E_AC, F_AC, G_AC, P_AC,
                            oneAC, twoAC, threeAC, fourAC, fiveAC, sixAC, sevenAC, eightAC, nineAC, zeroAC;

    private Queue<AudioClip> computerTerms = new Queue<AudioClip>();
    private Queue<string> terms = new Queue<string>();

    private GameObject PickerCartTechnics, PickByVision;

    private void Awake()
    {
        PickerCartTechnics = GameObject.Find("PickerCart").transform.GetChild(0).gameObject.transform.GetChild(1).gameObject;
        PickerCartTechnics.SetActive(false);

        PickByVision = GameObject.Find("PickByVision").transform.gameObject;
        PickByVision.SetActive(false);

        //Computer Voice Dictionary
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.panStereo = 1;

        aisleAC = Resources.Load<AudioClip>("Audio/aisle");
        bayAC = Resources.Load<AudioClip>("Audio/bay");
        binAC = Resources.Load<AudioClip>("Audio/bin");
        confirmAC = Resources.Load<AudioClip>("Audio/confirm");
        loginAC = Resources.Load<AudioClip>("Audio/login");
        noInputDetectedAC = Resources.Load<AudioClip>("Audio/noInputDetected");
        orderAC = Resources.Load<AudioClip>("Audio/order");
        orderFinishedAC = Resources.Load<AudioClip>("Audio/orderFinished");
        pieceAC = Resources.Load<AudioClip>("Audio/piece");
        piecesAC = Resources.Load<AudioClip>("Audio/pieces");
        positionAC = Resources.Load<AudioClip>("Audio/position");
        positionsAC = Resources.Load<AudioClip>("Audio/positions");
        rackAC = Resources.Load<AudioClip>("Audio/rack");
        shelfAC = Resources.Load<AudioClip>("Audio/shelf");
        subsetAC = Resources.Load<AudioClip>("Audio/subset");
        subsetConfirmAC = Resources.Load<AudioClip>("Audio/subsetConfirm");
        withAC = Resources.Load<AudioClip>("Audio/with");
        zoneAC = Resources.Load<AudioClip>("Audio/zone");

        oneAC = Resources.Load<AudioClip>("Audio/1");
        twoAC = Resources.Load<AudioClip>("Audio/2");
        threeAC = Resources.Load<AudioClip>("Audio/3");
        fourAC = Resources.Load<AudioClip>("Audio/4");
        fiveAC = Resources.Load<AudioClip>("Audio/5");
        sixAC = Resources.Load<AudioClip>("Audio/6");
        sevenAC = Resources.Load<AudioClip>("Audio/7");
        eightAC = Resources.Load<AudioClip>("Audio/8");
        nineAC = Resources.Load<AudioClip>("Audio/9");
        zeroAC = Resources.Load<AudioClip>("Audio/0");

        A_AC = Resources.Load<AudioClip>("Audio/A");
        B_AC = Resources.Load<AudioClip>("Audio/B");
        C_AC = Resources.Load<AudioClip>("Audio/C");
        D_AC = Resources.Load<AudioClip>("Audio/D");
        E_AC = Resources.Load<AudioClip>("Audio/E");
        F_AC = Resources.Load<AudioClip>("Audio/F");
        G_AC = Resources.Load<AudioClip>("Audio/G");
        P_AC = Resources.Load<AudioClip>("Audio/P");

        //Human Voice Dictionary
        keywordActions.Add("OK", () => _gotOK = true);
        keywordActions.Add("Repeat", () => _gotREPEAT = true);
        keywordActions.Add("Order", () => _gotORDER = true);
        keywordActions.Add("Start", () => terms.Enqueue("Start"));
        keywordActions.Add("Finish", () => terms.Enqueue("Finish"));

        keywordActions.Add("one", () => { terms.Enqueue("1"); computerTerms.Enqueue(oneAC); Debug.Log("1"); });
        keywordActions.Add("two", () => { terms.Enqueue("2"); computerTerms.Enqueue(twoAC); Debug.Log("2"); });
        keywordActions.Add("three", () => { terms.Enqueue("3"); computerTerms.Enqueue(threeAC); Debug.Log("3"); });
        keywordActions.Add("four", () => { terms.Enqueue("4"); computerTerms.Enqueue(fourAC); Debug.Log("4"); });
        keywordActions.Add("five", () => { terms.Enqueue("5"); computerTerms.Enqueue(fiveAC); Debug.Log("5"); });
        keywordActions.Add("six", () => { terms.Enqueue("6"); computerTerms.Enqueue(sixAC); Debug.Log("6"); });
        keywordActions.Add("seven", () => { terms.Enqueue("7"); computerTerms.Enqueue(sevenAC); Debug.Log("7"); });
        keywordActions.Add("eight", () => { terms.Enqueue("8"); computerTerms.Enqueue(eightAC); Debug.Log("8"); });
        keywordActions.Add("nine", () => { terms.Enqueue("9"); computerTerms.Enqueue(nineAC); Debug.Log("9"); });
        keywordActions.Add("zero", () => { terms.Enqueue("0"); computerTerms.Enqueue(zeroAC); Debug.Log("0"); });

        /// TODO
        /*Read Order List*/
    }

    void Start()
    {
        keywordRecognizer = new KeywordRecognizer(keywordActions.Keys.ToArray());
        keywordRecognizer.OnPhraseRecognized += OnKeywordsRecognized;
    }

    private void OnKeywordsRecognized(PhraseRecognizedEventArgs args)
    {
        Debug.Log("Keyword: " + args.text + "confidence: " + args.confidence);
        keywordActions[args.text].Invoke();
    }

    // LOGIC
    private bool partOneIsDone = false,   // Finish Login initialization
                 partTwoIsDone = false,   // Finish Order-List
                 partThreeIsDone = false;   // Return to Terminal

    string expectedLoginNr = "010";  //get/load from elsewhere. zerlegen zu terms... hmm -> oneAC zeroAC oneAC

    bool _headSetOnHead = false;
    bool onLogin = false;
    bool _gotOK = false;
    bool _gotREPEAT = false;
    bool _gotORDER = false;
    bool loginACplayed = false;
    bool confirmACplayed = false;
    bool waitForLoginConfirm = false;
    bool waitForStartOrder = false;

    bool callActualAisle = false;

    AudioClip lastInstruction;

    private IEnumerator _repeatInCR;

    // Logic Flow
    private void Logic()
    {
        if (!partOneIsDone)                             /** LOGIN Sequence **/
        {
            if (_headSetOnHead)                         // wait until HeadSetIsOnHead
            {
                if (!loginACplayed)                     // If not played yet
                {
                    loginACplayed = true;               // Make sure to play it once

                    if (_repeatInCR != null)            // stop counter for repeat if already running
                        StopCoroutine(_repeatInCR);

                    else
                    {
                        _repeatInCR = (RepeatIn(15, nameof(loginACplayed))); // repeat in 15 seconds if login not confirmed
                        StartCoroutine(_repeatInCR);
                    }

                    StartCoroutine(PlayAudio(loginAC)); // "Please Log In"
                }

                if (_gotOK && !waitForLoginConfirm)     // after user say the login key (0,1,0) and (OK)
                {
                    keywordRecognizer.Stop();
                    Debug.Log("#2 no speak!");
                    _gotOK = false;                     // lock OK-key again as false
                    waitForLoginConfirm = true;         // let's wait for confirmation
                }

                if (waitForLoginConfirm)
                {
                    if (!confirmACplayed)
                    {
                        Debug.Log("#3 no speak!");
                        AudioClip[] loginConfirmSentence = new AudioClip[computerTerms.Count + 1];
                        loginConfirmSentence[0] = confirmAC;
                        int i = 1;
                        foreach (AudioClip term in computerTerms)
                        {
                            loginConfirmSentence[i] = term; i++;
                        }

                        confirmACplayed = true;

                        if (computerTerms.Count != 0)
                            StartCoroutine(PlayAudio(lastInstruction = BuildSentence(loginConfirmSentence))); // build sentence confirm + user pin  (0 . 1 . 0)    starte sprechen mode    
                        else
                        {
                            StartCoroutine(PlayAudio(noInputDetectedAC, "noPin"));              // if no User input
                            _gotREPEAT = false;
                            terms.Clear();
                            computerTerms.Clear();
                        }
                    }

                    if (_gotOK)
                    {
                        _gotOK = false;
                        keywordRecognizer.Stop();
                        StartCoroutine(PlayAudio(BuildSentence(new AudioClip[] { orderAC, sevenAC, sevenAC, oneAC, fourAC, eightAC, withAC, sixAC, positionsAC, zoneAC, P_AC })));
                        waitForStartOrder = true;
                    }

                    if (_gotREPEAT)                            // User will have to repeat the PIN once again. Reset all Values to do so.
                    {
                        Debug.Log("5# Let Me Repeat");
                        _gotREPEAT = false;
                        terms.Clear();
                        computerTerms.Clear();
                        loginACplayed = false;
                        waitForLoginConfirm = false;
                        confirmACplayed = false;
                    }
                }

                if (waitForStartOrder)
                {
                    if (_gotORDER)
                    {
                        Debug.Log("#6 no speak!");
                        // check for 'Start' term
                        _gotORDER = false;
                        partOneIsDone = true;
                    }
                }
            }
        }

        if (partOneIsDone && !partTwoIsDone)
        {   /** Order List Sequence **/  // -> TODO!
            // EXAMPLE for first position out of the picking list
            if (!callActualAisle)
            {
                terms.Clear();
                computerTerms.Clear();
                callActualAisle = true;
                audioSource.PlayOneShot(lastInstruction = BuildSentence(new AudioClip[] { aisleAC, A_AC, bayAC, twoAC }));
            }

            if (_gotOK)
            {
                _gotOK = false;

                if (terms.Count == 0)                
                    audioSource.PlayOneShot(lastInstruction = BuildSentence(new AudioClip[] { shelfAC, threeAC, binAC, eightAC }));
                
                else
                {
                    audioSource.PlayOneShot(lastInstruction = BuildSentence(new AudioClip[] { oneAC, pieceAC, /*TO*/ binAC, threeAC}));

                    // Check Number for example 6.2.
                    Debug.Log("in else" + terms.Count);
                    string[] checkNr = new string[terms.Count];
                    int i = 0;
                    foreach (string term in terms)
                    {
                        checkNr[i] = term; i++;
                        Debug.Log("proofe: " + checkNr[i]);   // proofe check nr.
                    }
                }
            }

            //10.H. "6.2.OK"
            //11.C. "2.Pieces"
            //12.H. "2.OK"

            if (_gotREPEAT)
            {
                _gotREPEAT = false;
                audioSource.PlayOneShot(lastInstruction);
            }
        }
    }

    private IEnumerator PlayAudio(AudioClip AC)
    {
        audioSource.PlayOneShot(AC);
        yield return new WaitForSeconds(AC.length);
        keywordRecognizer.Start();
    }

    private IEnumerator PlayAudio(AudioClip AC, string instruction)
    {
        audioSource.PlayOneShot(AC);
        yield return new WaitForSeconds(AC.length);

        if (instruction.Equals("noPin"))
        {
            loginACplayed = false;
            waitForLoginConfirm = false;
            confirmACplayed = false;
        }

        keywordRecognizer.Start();
    }

    private IEnumerator RepeatIn(float duration, string instruction)
    {
        while (true)
        {
            yield return new WaitForSeconds(duration);

            if (instruction.Equals("loginACplayed"))
            { // if the time is up and we are here without confirm from user, repeat instruction
                if (!waitForLoginConfirm)
                {     // HAVE to change somthing here
                    keywordRecognizer.Stop();
                    computerTerms.Clear();
                    loginACplayed = false;
                }
            }
        }
    }

    void Update()
    {
        Logic();

        // check in loop and close after headset was taken, or completely move to logic with this one?
        if (!_headSetOnHead)
            if (ControlRegister.PickByVoice)
                _headSetOnHead = true;
    }

    public static AudioClip BuildSentence(AudioClip[] clips)
    {
        if (clips == null || clips.Length == 0)
            return null;

        int length = 0;
        for (int i = 0; i < clips.Length; i++)
        {
            if (clips[i] == null)
                continue;

            length += clips[i].samples * clips[i].channels;
        }

        float[] data = new float[length];
        length = 0;
        for (int i = 0; i < clips.Length; i++)
        {
            if (clips[i] == null)
                continue;

            float[] buffer = new float[clips[i].samples * clips[i].channels];
            clips[i].GetData(buffer, 0);
            buffer.CopyTo(data, length);
            length += buffer.Length;
        }

        if (length == 0)
            return null;

        AudioClip result = AudioClip.Create("Combine", length / 2, 2, 44100, false);

        result.SetData(data, 0);

        return result;
    }
}