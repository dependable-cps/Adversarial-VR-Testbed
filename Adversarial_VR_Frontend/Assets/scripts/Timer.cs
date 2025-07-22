using System;
using System.Collections;
using UnityEngine;
using TMPro;
using Random = System.Random;

public class Timer : MonoBehaviour
{
    [Header("Component")] public TextMeshProUGUI timerText;
    public TextMeshProUGUI WMText;

    [Header("Timer Settings")] public float currentTime;
    public bool countDown;

    private string lastSpokenText = ""; // Track last spoken message
    private Random rnd = new Random(); // Random generator initialized once
    private string formattedString = ""; // Store generated number
    private bool startTimer;
    private Coroutine digitRecallCoroutine;
    private Coroutine ratingPromptCoroutine;

    void Start()
    {
        startTimer = false;
        WMText.enabled = false;
    }

    private void OnEnable()
    {
        ImageToggleController.ActiveTimer += StartTimer;
    }

    private void OnDisable()
    {
        ImageToggleController.ActiveTimer -= StartTimer;
    }

    private void StartTimer(bool start)
    {
        startTimer = start;
        if (startTimer)
        {
            if (digitRecallCoroutine == null)
                digitRecallCoroutine = StartCoroutine(DigitRecallSequence());

            if (ratingPromptCoroutine == null)
                ratingPromptCoroutine = StartCoroutine(ManageRatingPrompt()); // Start managing rating message
        }
        else
        {
            if (digitRecallCoroutine != null)
            {
                StopCoroutine(digitRecallCoroutine);
                digitRecallCoroutine = null;
            }

            if (ratingPromptCoroutine != null)
            {
                StopCoroutine(ratingPromptCoroutine);
                ratingPromptCoroutine = null;
            }
        }
    }

    void Update()
    {
        if (!startTimer) return;

        currentTime = countDown ? currentTime - Time.deltaTime : currentTime + Time.deltaTime;
        timerText.text = "Time: " + TimeSpan.FromSeconds(currentTime).ToString(@"mm\:ss");
        AttentionScoringSystem.globalTime = (int)currentTime;

        if (currentTime < 40)
            timerText.faceColor = Color.red;
        else if (currentTime < 120)
            timerText.faceColor = Color.yellow;
    }

    /// <summary>
    /// 🔹 Task 1: Sequence - Show digits → Wait 5s → Say "say the digits out loud!" → Repeat every 10s
    /// </summary>
    private IEnumerator DigitRecallSequence()
    {
        while (true)
        {
            formattedString = GenerateFormattedNumber();

            yield return new WaitForSeconds(2f);
            ShowMessage("Remember the following digits \n            " + formattedString);

            yield return new WaitForSeconds(15f);
            ShowMessage("say the digits out loud!");

            yield return new WaitForSeconds(8f);
            ShowMessage("Please Rate your discomfort on a scale from 0 to 10");
            yield return new WaitForSeconds(10f);
        }
    }

    /// <summary>
    /// 🔹 Task 2: Manage rating prompt so it plays at the beginning or end of DigitRecallSequence
    /// </summary>
    private IEnumerator ManageRatingPrompt()
    {
        while (true)
        {
            yield return new WaitForSeconds(60f); // Wait 60 seconds first

            // Wait for a safe moment to play the message (before or after DigitRecallSequence)
            if (WMText.text.Contains("Remember the following digits")) // If digits are showing, wait for them to finish
            {
                while (WMText.text.Contains("Remember the following digits") 
                       || WMText.text.Contains("say the digits out loud!")
                       || WMText.text.Contains("Please Rate your discomfort on a scale from 0 to 10"))
                {
                    yield return null; // Wait until the sequence ends
                }
            }

            if (digitRecallCoroutine != null)
            {
                StopCoroutine(digitRecallCoroutine);
                digitRecallCoroutine = null;
            }

            // Play the message at a safe moment
            WindowsVoice.speak("Please Rate your Physical, Mental and temporal Demand on a scale from 0 to 10");
            yield return new WaitForSeconds(8f);
            digitRecallCoroutine ??= StartCoroutine(DigitRecallSequence());
        }
    }

    private string GenerateFormattedNumber()
    {
        int num = rnd.Next(10000, 99999);
        string numString = num.ToString();
        return $"{numString[0]}  {numString[1]}  {numString[2]}  {numString[3]}  {numString[4]}";
    }

    private void ShowMessage(string message)
    {
        if (WMText.text != message)
        {
            WMText.enabled = true;
            WMText.text = message;

            if (lastSpokenText != message)
            {
                WindowsVoice.speak(message);
                lastSpokenText = message;
            }
        }
    }
}