using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;


public class TestingInputModule : StandaloneInputModule
{
    public bool IsPlaybacking { get; private set; }
    public bool IsRecording { get; private set; }

    List<RecordedInput> RecordedInputs = new List<RecordedInput>();
    Action OnPlaybackEnd;
    ManualTest CurrentPlaybackTest;
    int RecordStartingFrame;
    int PlaybackStartingFrame;


    public void StartRecording()
    {
        RecordStartingFrame = Time.frameCount;
        IsRecording = true;
    }

    public ManualTest EndRecording()
    {
        IsRecording = false;
        var recordedInputs = RecordedInputs.ToList();
        RecordedInputs.Clear();
        return new ManualTest()
        {
            StartingFrame = RecordStartingFrame,
            RecordedInputs = recordedInputs
        };
    }

    public void StartPlayback(ManualTest manualTest, Action onEnd)
    {
        if (manualTest.RecordedInputs == null || manualTest.RecordedInputs.Count == 0)
        {
            onEnd?.Invoke();
            return;
        }

        PlaybackStartingFrame = Time.frameCount;
        OnPlaybackEnd = onEnd;
        IsPlaybacking = true;
        CurrentPlaybackTest = manualTest;
    }

    public void EndPlayback()
    {
        IsPlaybacking = false;
        OnPlaybackEnd.Invoke();
        OnPlaybackEnd = null;
        CurrentPlaybackTest = null;
    }


    public override void Process()
    {
        if (!eventSystem.isFocused && ShouldIgnoreEventsOnNoFocus())
            return;


        bool usedEvent = SendUpdateEventToSelectedObject();

        // case 1004066 - touch / mouse events should be processed before navigation events in case
        // they change the current selected gameobject and the submit button is a touch / mouse button.

        // touch needs to take precedence because of the mouse emulation layer
        if (!ProcessTouchEvents() && input.mousePresent)
            ProcessMouseEvent();

        if (eventSystem.sendNavigationEvents)
        {
            if (!usedEvent)
                usedEvent |= SendMoveEventToSelectedObject();

            if (!usedEvent)
                SendSubmitEventToSelectedObject();
        }
    }

    // copied from StandaloneInputModule so this method can be called 
    private bool ShouldIgnoreEventsOnNoFocus()
    {
#if UNITY_EDITOR
        return !UnityEditor.EditorApplication.isRemoteConnected;
#else
            return true;
#endif
    }

    // Most of the content is copied from StandaloneInputModule
    private bool ProcessTouchEvents()
    {
        if (IsPlaybacking)
        {
            if (CurrentPlaybackTest.RecordedInputs.Count > 0)
            {
                if (CurrentPlaybackTest.RecordedInputs[0].FrameNumber != Time.frameCount - PlaybackStartingFrame)
                {
                    return input.touchCount > 0;
                }
            }
            else
            {
                return input.touchCount > 0;
            }
        }

        if (IsRecording && input.touchCount > 0)
        {
            Debug.Log($"Found {input.touchCount} inputs");
            RecordedInputs.Add(new RecordedInput()
            {
                FrameNumber = Time.frameCount - RecordStartingFrame,
                Touches = new Touch[input.touchCount]
            });
        }

        var inputCount = IsPlaybacking ? CurrentPlaybackTest.RecordedInputs[0].Touches?.Length : input.touchCount;

        for (int i = 0; i < inputCount; ++i)
        {
            Touch touch;
            if (IsPlaybacking)
            {
                touch = CurrentPlaybackTest.RecordedInputs[0].Touches[i];
            }
            else
            {
                touch = input.GetTouch(i);
            }

            if (IsRecording)
            {
                RecordedInputs[RecordedInputs.Count - 1].Touches[i] = touch;
            }

            if (touch.type == TouchType.Indirect)
                continue;

            bool released;
            bool pressed;
            var pointer = GetTouchPointerEventData(touch, out pressed, out released);

            ProcessTouchPress(pointer, pressed, released);

            if (!released)
            {
                ProcessMove(pointer);
                ProcessDrag(pointer);
            }
            else
                RemovePointerData(pointer);
        }

        if (IsPlaybacking)
        {
            CurrentPlaybackTest.RecordedInputs.RemoveAt(0);
            if (CurrentPlaybackTest.RecordedInputs.Count == 0)
            {
                EndPlayback();
            }
        }


        return input.touchCount > 0;
    }
}