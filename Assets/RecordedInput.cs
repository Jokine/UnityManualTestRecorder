using System.Collections.Generic;
using UnityEngine;


public struct RecordedInput
{
    public Touch[] Touches;
    public int FrameNumber;
}

public class ManualTest
{
    public List<RecordedInput> RecordedInputs = new List<RecordedInput>();
    public int StartingFrame;
}