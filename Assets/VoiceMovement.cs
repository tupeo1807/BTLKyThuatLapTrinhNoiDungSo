using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Windows.Speech;
using UnityEngine.Animations;

public class VoiceMovement : MonoBehaviour
{
    public Vector3 thirdViewPosition = new Vector3(10f, 20f, 50f);
    public Vector3 firstViewPosition = new Vector3(0f,0f,0f);
    private KeywordRecognizer keywordRecognizer;
    private Dictionary<string, Action> actions = new Dictionary<string, Action>();

    void Start()
    {
        actions.Add("switch", SwitchView);
        keywordRecognizer = new KeywordRecognizer(actions.Keys.ToArray());
        keywordRecognizer.OnPhraseRecognized += RecognizedSpeed;
        keywordRecognizer.Start();
    }

    private void RecognizedSpeed(PhraseRecognizedEventArgs speech)
    {
        Debug.Log(speech.text);
        actions[speech.text].Invoke();
    }

    private void SwitchView()
    {
        if (transform.localPosition == thirdViewPosition)
        {
            transform.localPosition = firstViewPosition;
        } else if (transform.localPosition == firstViewPosition)
        {
            transform.localPosition = thirdViewPosition;
        }
    }
}
