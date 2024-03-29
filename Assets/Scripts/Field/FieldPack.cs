using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

[ExecuteInEditMode]
public class FieldPack : MonoBehaviour
{
    public static FieldPack active;
    
    [SerializeField] int sideSize = 3;
    int FieldsCount => sideSize == 3 ? 8 : (sideSize == 2 ? 4 : 1);
    public int packId;
    public FieldMatrix[] fields;
    public float cameraSizeMultiplier = 1f, size;

    [SerializeField] bool refreshPositions;

    void OnValidate()
    {
        if (refreshPositions)
        {
            refreshPositions = false;
            LoadFields();
            PlaceFields();
        }
    }

    public float Height => sideSize * transform.localScale.x;

    void LoadFields()
    {
        fields = GetComponentsInChildren<FieldMatrix>();
        for (var fieldId = 0; fieldId < fields.Length; fieldId++)
        {
            var field = fields[fieldId];
            field.transform.SetParent(transform);
            field.fieldId = fieldId;
            field.packId = packId;
            field.gameObject.name = $"fm {packId}_{fieldId}";
            
#if UNITY_EDITOR
            EditorUtility.SetDirty(field);
            AssetDatabase.Refresh();
#endif
        }
    }

    public void Activate()
    {
        if (active != null)
            foreach (var field in active.fields)
                field.SetHovered(false);
        active = this;
        PostFxController.Instance.LoadPackPalette(packId);
    }

    public bool Complete { get; private set; }
    public void FieldCompleted(bool reopenedField = false)
    {
        PlaceFields();
        if (fields.All(field => field.completion == FieldCompletion.Complete))
        {
            Complete = true;
            if (!reopenedField)
                FieldPacksCollection.GetFirstIncompletePack().Activate();
        }
    }

    public void PlaceFields()
    {
        for (var fieldId = 0; fieldId < FieldsCount; fieldId++)
        {
            var field = fields[fieldId];
            PlaceField(field);
        }

        foreach (var pack in GetPacksByParent()) 
            pack.RefreshPosition();
    }

    void PlaceField(FieldMatrix field)
    {
        var scale = 1f / field.Size;
        field.transform.localScale = new Vector3(scale, scale, scale);
        // field.transform.localPosition = FieldIdToUnitPos(field.fieldId) * (field.completion == FieldCompletion.Complete ? 1f : PushApartMult);
        field.transform.localPosition = new Vector3(field.packPositionX, field.packPositionY);
    }

    protected void RefreshPosition()
    {
        Vector2 min = Vector2.zero, max = Vector2.zero;
        foreach (var field in fields)
        {
            min.x = Mathf.Min(field.packPositionX, min.x);
            min.y = Mathf.Min(field.packPositionY, min.y);
            max.x = Mathf.Max(field.packPositionX, max.x);
            max.y = Mathf.Max(field.packPositionY, max.y);
        }
        transform.localPosition = -new Vector3((min.x + max.x) / 2, (min.y + max.y) / 2);
        if (packId > 8)
        {
            transform.localPosition = new Vector3(0f, -transform.localScale.y * (packId - 8));
        }
        size = Mathf.Max(max.x - min.x, max.y - min.y) + 1;
        RefreshScale();
    }

    protected void RefreshScale()
    {
        if (packId == 0)
        {
            transform.localScale = Vector3.one;
            return;
        }

        var prevPack = GetPacksByParent()[packId - 1];
        var scale = (packId > 9 ? 1f : prevPack.size) * prevPack.transform.localScale;
        transform.localScale = scale;
    }

    FieldPack[] GetPacksByParent()
    {
        return GetComponentInParent<FieldPacksCollection>().GetComponentsInChildren<FieldPack>();
    }

    public void SetHoveredByUnlockIndex()
    {
        if (fields.All(f => f.completion == FieldCompletion.Complete)) return;
        var index = fields.Where(f => f.completion != FieldCompletion.Complete).Min(f => f.unlockIndex);
        fields.First(f => f.unlockIndex == index).SetHovered(true);
    }

    public void EnterHoveredField()
    {
        var field = fields.FirstOrDefault(f => f.hovered);
        if (field != null)
        {
            field.SetScreenState(FieldScreenState.Active);
        }
    }

    public static FieldPack Create(int packId)
    {
        var go = new GameObject($"FieldPack{packId}");
        var size = packId == 0 ? 2 : 3;
        var fp = go.AddComponent<FieldPack>();
        fp.sideSize = size;
        fp.packId = packId;
        
        fp.LoadFields();
        fp.PlaceFields();
        return fp;
    }
}