using TMPro;
using UnityEngine;

public class LevelSaver : LevelSelectorBox
{
    [SerializeField] TMP_InputField inputField;

    void Awake()
    {
        var submitEvent = new TMP_InputField.SubmitEvent();
        submitEvent.AddListener(Save);
        inputField.onSubmit = submitEvent;
    }
    protected override void OnEntryClick(LevelSelectorEntry entry)
    {
        inputField.text = entry.Text;
        inputField.Select();
    }

    public void Save(string name)
    {
        FieldMatrix.current.shapesContainer.SaveToFile(FileStorage.LevelPath(name, false));
        Refresh();
    }
}