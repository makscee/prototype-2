using System.Linq;
using UnityEngine;

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

    public SequenceBuilder ShapeSidesThicknessRandomBlink()
    {
        var shapes = _field.shapesContainer.shapes;
        var prevIndex = -1;

        var duration = GlobalConfig.Instance.fieldCompleteShapeThicknessBlinkTime;
        if (_field.packId == 0) duration /= 2;
        var inter = Animator.Interpolate(0f, 0.99999f, duration)
            .PassValue(v =>
            {
                var third = v * 6f;
                var part = Mathf.FloorToInt(third);
                var t = third - part;
                var shapeIndex = Mathf.FloorToInt(t * shapes.Count);
                var shapeT = t * shapes.Count - shapeIndex;
                var closing = part % 2 == 0;
                var shapeThickness = Mathf.Lerp(GlobalConfig.Instance.thicknessBase, 1.2f,
                    closing ? shapeT : 1f - shapeT);
                if (prevIndex != shapeIndex)
                    shapes[shapeIndex].shapeObject.PlaySidesSound(!closing);
                if (shapeIndex > 0 && prevIndex != shapeIndex)
                {
                    var prevThickness = Mathf.Lerp(GlobalConfig.Instance.thicknessBase, 1.2f,
                        part % 2 == 0 ? 1f : 0f);
                    foreach (var shapeCell in shapes[prevIndex].cells)
                    {
                        shapeCell.shapeCellObject.sidesContainer.SetThicknessOverride(prevThickness);
                    }
                }
                foreach (var shapeCell in shapes[shapeIndex].cells)
                {
                    shapeCell.shapeCellObject.sidesContainer.SetThicknessOverride(shapeThickness);
                }

                prevIndex = shapeIndex;
            }).Type(InterpolationType.Square).WhenDone(() =>
            {
                foreach (var shapeCell in shapes.SelectMany(shape => shape.cells))
                {
                    shapeCell.shapeCellObject.sidesContainer.ClearThicknessOverride();
                }
            });
        AddInterpolationToBuilder(inter);
        return builder;
    }
}