using UnityEngine;

public struct FloatTarget
{
    public float value, target;

    public FloatTarget(float value, float target)
    {
        this.target = target;
        this.value = value;
    }

    public FloatTarget(float value)
    {
        this.value = value;
        target = value;
    }

    public bool TargetMet => value == target;

    public void ProgressToTarget(float delta)
    {
        if (TargetMet) return;
        value = Mathf.Lerp(value, target, delta);
    }
}