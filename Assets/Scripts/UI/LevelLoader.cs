public class LevelLoader : LevelSelectorBox
{
    protected override void OnEntryClick(LevelSelectorEntry entry)
    {
        var field = FieldMatrixSerialized.Load(entry.Text)?.Deserialize();
        if (field != null)
        {
            FieldMatrix.current = field;
        }
        Hide();
    }
}