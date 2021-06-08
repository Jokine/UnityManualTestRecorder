using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;


public class TestingService : MonoBehaviour
{
    int StartingFrameNumber;
    [SerializeField] BaseInputModule DefaultInputModule;
    [SerializeField] TextMeshProUGUI PressedButtonText;
    TestingInputModule TestingInputModuleField;
    ManualTest ManualTest;

    TestingInputModule TestingInputModule
    {
        get
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
        TestingInputModule.StartRecording();
        Debug.Log("Started recording");
    }

    public void EndRecording()
    {
        if (TestingInputModule.IsPlaybacking) return;
        ManualTest = TestingInputModule.EndRecording();
        Debug.Log($"Ended recording");
    }


    public void Playback()
    {
        if (ManualTest == null) return;
        Debug.Log("Playback started");
        TestingInputModule.StartPlayback(ManualTest, OnPlaybackEnd);
    }

    void OnPlaybackEnd()
    {
        Debug.Log("Playback ended");
    }

    public void ButtonPressed(string buttonName)
    {
        Debug.Log($"{buttonName} pressed");
        PressedButtonText.text = buttonName;
    }
}