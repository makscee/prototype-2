public class LevelLoader : LevelSelectorBox
{
    protected override void OnEntryClick(LevelSelectorEntry entry)
    {
        var container = ShapeContainerSerialized
            .LoadFromJson(FileStorage.ReadJsonFile(FileStorage.LevelPath(entry.Text, false)))
            .Deserialize(FieldMatrix.current);
        FieldMatrix.current.AddContainer(container);
        Hide();
    }
}