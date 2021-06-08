using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;


public class TestingService : MonoBehaviour
{
    int StartingFrameNumber;
    [SerializeField] BaseInputModule DefaultInputModule;
    [SerializeField] TextMeshProUGUI PressedButtonText;

    [SerializeField]
    private TextMeshProUGUI ConsoleText;
    
    TestingInputModule TestingInputModuleField;
    ManualTest ManualTest;

    TestingInputModule TestingInputModule
    {
        get
        {
            try
            {
                if (DefaultInputModule != null)
                {
                    if (DefaultInputModule.GetType() != typeof(TestingInputModule))
                    {
                        var inputModuleGo = DefaultInputModule.gameObject;
                        Destroy(DefaultInputModule);
                        TestingInputModuleField = inputModuleGo.AddComponent<TestingInputModule>();
                        DefaultInputModule = null;
                    }
                }
            }
            catch (Exception e)
            {
                ConsoleText.text = e.ToString();
                Debug.LogError(ConsoleText.text);
            }

            return TestingInputModuleField;
        }
    }

    void Awake()
    {
        // making sure framerate is consistent for poc
        Application.targetFrameRate = 60;
    }


    public void StartRecording()
    {
        try
        {
            TestingInputModule.StartRecording();
            ConsoleText.text = "Started recording";
            Debug.Log(ConsoleText.text);
        }
        catch (Exception e)
        {
            ConsoleText.text = e.ToString();
            Debug.LogError(ConsoleText.text);
        }
    }

    public void EndRecording()
    {
        try
        {
            if (TestingInputModule.IsPlaybacking) return;
            ManualTest = TestingInputModule.EndRecording();
            ConsoleText.text = "Ended recording";
            Debug.Log(ConsoleText.text);
        }
        catch (Exception e)
        {
            ConsoleText.text = e.ToString();
            Debug.LogError(ConsoleText.text);
        }
    }


    public void Playback()
    {
        if (ManualTest == null) return;

        try
        {
            ConsoleText.text = "Playback started";
            Debug.Log(ConsoleText.text);
            TestingInputModule.StartPlayback(ManualTest, OnPlaybackEnd);
        }
        catch (Exception e)
        {
            ConsoleText.text = e.ToString();
            Debug.LogError(ConsoleText.text);
        }
    }

    void OnPlaybackEnd()
    {
        try
        {
            ConsoleText.text = "Playback ended";
            Debug.Log(ConsoleText.text);
        }
        catch (Exception e)
        {
            ConsoleText.text = e.ToString();
            Debug.LogError(ConsoleText.text);
        }
    }

    public void ButtonPressed(string buttonName)
    {
        try
        {
            ConsoleText.text = $"{buttonName} pressed";
            Debug.Log(ConsoleText.text);
            PressedButtonText.text = buttonName;
        }
        catch (Exception e)
        {
            ConsoleText.text = e.ToString();
            Debug.LogError(ConsoleText.text);
        }
    }
}