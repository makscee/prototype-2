public class FrontCarcassSprite : CarcassSprite
{
    protected override void OnEnable()
    {
        sortingOrder = 8;
        color = GlobalConfig.Instance.palette2;
        base.OnEnable();
    }
}