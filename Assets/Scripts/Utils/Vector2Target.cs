using UnityEngine;

public class Vector2Target
{
    public Vector2 target, value;

    public Vector2Target(Vector2 value, Vector2 target)
    {
        this.target = target;
        this.value = value;
    }

    public Vector2Target(Vector2 value)
    {
        this.value = value;
        target = value;
    }

    public bool TargetMet => target == value;

    public void ProgressToTarget(float delta)
    {
        if (TargetMet) return;
        value = Vector2.Lerp(value, target, delta);
    }
}