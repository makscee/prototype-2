public class LevelLoader : LevelSelectorBox
{
    protected override void OnEntryClick(LevelSelectorEntry entry)
    {
        var container = ShapeContainerSerialized.LoadByName(entry.Text).Deserialize(FieldMatrix.current);
        FieldMatrix.current.SetContainer(container);
        Hide();
    }
}