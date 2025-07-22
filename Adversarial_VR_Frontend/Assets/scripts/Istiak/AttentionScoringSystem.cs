using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class AttentionScoringSystem : MonoBehaviour
{
    [Header("XR Input Actions")]
    [SerializeField] private InputActionReference rightTriggerAction;
    [SerializeField] private InputActionReference leftTriggerAction;
    [SerializeField] private InputActionReference rightGripAction;
    [SerializeField] private InputActionReference leftGripAction;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI attentionScoreText;

    private bool startTimer = false;
    private int attentionScore = 0;
    private string currentWord = "Bravo";
    private bool buttonCheck = false;
    private int lastTime = -1;

    public static int globalTime = 0;

    private void OnEnable()
    {
        ImageToggleController.ActiveTimer += StartTimer;
        EnableActions();
    }

    private void OnDisable()
    {
        ImageToggleController.ActiveTimer -= StartTimer;
        DisableActions();
    }

    private void EnableActions()
    {
        GetInputAction(rightTriggerAction)?.Enable();
        GetInputAction(leftTriggerAction)?.Enable();
        GetInputAction(rightGripAction)?.Enable();
        GetInputAction(leftGripAction)?.Enable();
    }

    private void DisableActions()
    {
        GetInputAction(rightTriggerAction)?.Disable();
        GetInputAction(leftTriggerAction)?.Disable();
        GetInputAction(rightGripAction)?.Disable();
        GetInputAction(leftGripAction)?.Disable();
    }

    private static InputAction GetInputAction(InputActionReference actionRef)
    {
#pragma warning disable IDE0031
        return actionRef != null ? actionRef.action : null;
#pragma warning restore IDE0031
    }

    private void StartTimer(bool value)
    {
        startTimer = value;
    }

    private void Update()
    {
        if (!startTimer) return;

        bool rightTrigger = GetInputAction(rightTriggerAction)?.triggered ?? false;
        bool leftTrigger = GetInputAction(leftTriggerAction)?.triggered ?? false;
        bool rightGrip = GetInputAction(rightGripAction)?.triggered ?? false;
        bool leftGrip = GetInputAction(leftGripAction)?.triggered ?? false;

        var inputs = new[] { rightTrigger, leftTrigger, rightGrip, leftGrip };

        if (globalTime % 5 == 0 && globalTime != lastTime)
        {
            if (buttonCheck && currentWord == "Bravo") attentionScore++;

            string[] words = { "Alpha", "Bravo", "Charlie", "Delta" };
            currentWord = words[UnityEngine.Random.Range(0, words.Length)];
            WindowsVoice.speak(currentWord);

            lastTime = globalTime;
            buttonCheck = true;
        }

        if (buttonCheck)
        {
            if (currentWord != "Bravo")
            {
                if ((currentWord == "Alpha" && rightTrigger) ||
                    (currentWord == "Charlie" && leftTrigger) ||
                    (currentWord == "Delta" && rightGrip))
                {
                    attentionScore++;
                    buttonCheck = false;
                }
                else if (Array.Exists(inputs, x => x))
                {
                    buttonCheck = false; // wrong input
                }
            }
            else if (Array.Exists(inputs, x => x))
            {
                buttonCheck = false; // wrong press on Bravo
            }
        }

        attentionScoreText.text = "Attention Score: " + attentionScore;
    }
}
