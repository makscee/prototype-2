using System;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[ExecuteInEditMode]
public class LevelSelectorEntry : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] RawImage rawImage;
    [SerializeField] TextMeshProUGUI nameText;

    [SerializeField] string _text;

    public Action<LevelSelectorEntry> onClick;

    public string Text
    {
        get => _text;
        set
        {
            _text = value;
            Refresh();
        }
    }

    void OnValidate()
    {
        Refresh();
    }

    void Refresh()
    {
        rawImage.color = transform.GetSiblingIndex() % 2 == 0
            ? ScriptableConfigObject.Instance.levelSelectorEntry1
            : ScriptableConfigObject.Instance.levelSelectorEntry2;
        nameText.text = _text;
    }

    public void Delete()
    {
        var path = FileStorage.LevelPath(_text, true);
        if (File.Exists(path))
        {
            File.Delete(path);
            Debug.Log($"{_text} deleted");
        }
        else Debug.LogError($"{path} not found");

        Destroy(gameObject);
    }

    public static LevelSelectorEntry Create(Transform parent, string filename)
    {
        var go = Instantiate(Prefabs.Instance.levelEntry, parent);
        var entry = go.GetComponent<LevelSelectorEntry>();
        entry.Text = filename;
        return entry;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        onClick?.Invoke(this);
    }
}