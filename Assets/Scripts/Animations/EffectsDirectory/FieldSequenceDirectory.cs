public class FieldSequenceDirectory : SequenceDirectoryBase
{
    FieldMatrix _field;
    public FieldSequenceDirectory(SequenceBuilder builder, FieldMatrix field) : base(builder)
    {
        _field = field;
    }

    public SequenceBuilder CellsPatternBalanceChange(float value)
    {
        var inter = ChangeBalance(_field.cellsMaterialProvider, value);
        AddInterpolationToBuilder(inter);
        return builder;
    }

    public SequenceBuilder CompletionSpriteBalanceChange(float value)
    {
        var inter = ChangeBalance(_field.completionSprite, value);
        AddInterpolationToBuilder(inter);
        return builder;
    }

    Interpolator<float> ChangeBalance(PatternMaterialProvider provider, float value)
    {
        var inter = Animator.Interpolate(provider.balanceTarget, value,
                GlobalConfig.Instance.balanceSetAnimationTime)
            .PassDelta(v =>
            {
                provider.balance += v;
                provider.SetShaderProperties();
            });
        provider.balanceTarget = value;
        return inter;
    }

    public SequenceBuilder ShapeSidesThicknessChange(float value)
    {
        var t = _field.ShapeSidesThickness;
        var from = t.target;
        t.target = value;
        _field.ShapeSidesThickness = t;
        var inter = Animator.Interpolate(from, value, GlobalConfig.Instance.sidesThicknessRecoverTime)
            .PassDelta(v =>
            {
                t = _field.ShapeSidesThickness;
                t.value += v;
                _field.ShapeSidesThickness = t;
            }).Type(InterpolationType.InvSquare);
        AddInterpolationToBuilder(inter);
        return builder;
    }
}