public class FrontCarcassSprite : CarcassSprite
{
    protected override void OnEnable()
    {
        sortingOrder = 8;
        color = GlobalConfig.Instance.palette1;
        base.OnEnable();
    }
}