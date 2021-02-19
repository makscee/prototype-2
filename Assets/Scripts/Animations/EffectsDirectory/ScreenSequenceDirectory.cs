using UnityEngine;

public class ScreenSequenceDirectory : SequenceDirectoryBase
{
    public ScreenSequenceDirectory(SequenceBuilder builder) : base(builder)
    {
    }

    public SequenceBuilder GoToSelectScreen()
    {
        AddActionToBuilder(v =>
        {
            Debug.Log($"go to select screen test {v}");
        }, 2f, 0f, true);
        return builder;
    }
}