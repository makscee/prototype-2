using UnityEngine;

public class Vector3Target
{
    public Vector3 target, value;

    public Vector3Target(Vector3 value, Vector3 target)
    {
        this.target = target;
        this.value = value;
    }

    public Vector3Target(Vector3 value)
    {
        this.value = value;
        target = value;
    }
    public bool TargetMet => target == value;

    public void ProgressToTarget(float delta)
    {
        if (TargetMet) return;
        value = Vector3.Lerp(value, target, delta);
    }
}