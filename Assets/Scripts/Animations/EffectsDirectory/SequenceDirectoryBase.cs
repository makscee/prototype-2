using System;

public class SequenceDirectoryBase
{
    protected SequenceBuilder builder;
    public SequenceDirectoryBase(SequenceBuilder builder)
    {
        this.builder = builder;
    }

    protected void AddActionToBuilder(Action<float> a, float duration, float delay, bool passValue = false)
    {
        var anim = Animator.Interpolate(0f, 1f, duration).Delay(delay);
        if (passValue) anim.PassValue(a);
        else anim.PassDelta(a);
        builder.AddToChain(anim);
    }

    protected void AddInterpolationToBuilder(Interpolator<float> a)
    {
        builder.AddToChain(a);
    }
}