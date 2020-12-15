public class BackCarcassSprite : CarcassSprite
{
    protected override void OnEnable()
    {
        sortingOrder = 7;
        color = GlobalConfig.Instance.palette3;
        base.OnEnable();
    }
}