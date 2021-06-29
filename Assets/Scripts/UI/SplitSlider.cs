using System;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[ExecuteAlways]
public class SplitSlider : MonoBehaviour, IDragHandler, IPointerClickHandler, IEndDragHandler, IBeginDragHandler
{
    [SerializeField] RawImage top, bottom;
    [SerializeField] RectTransform rect;
    [SerializeField] TextMeshProUGUI valueText;
    [SerializeField] bool vertical;
    [SerializeField] bool showDecimal;

    float _valueBefore;
    [field: Range(0f, 1f)] public float value;
    public float Min = 0f, Max = 1f;
    public float MultipliedValueFloat => Min + value * (Max - Min);
    public int MultipliedValueInt => (int) MultipliedValueFloat;

    public void InitValue(float v)
    {
        value = (v - Min) / (Max - Min);
    }

    float TopSize => vertical ? top.rectTransform.rect.height : top.rectTransform.rect.width; 
    float BotSize => vertical ? bottom.rectTransform.rect.height : bottom.rectTransform.rect.width;
    float TotalSize => vertical ? rect.rect.height : rect.rect.width;
    void Update()
    {
        if (Math.Abs(TopSize - GetSize(value, true)) > 0.001f ||
            Math.Abs(BotSize - GetSize(value, false)) > 0.001f)
        {
            value = Mathf.Clamp(value, 0f, 1f);
            ValueUpdated(value);
            _valueBefore = value;
        }
    }

    float GetSize(float v, bool top)
    {
        return (top ? 1 - v : v) * TotalSize;
    }
    void ValueUpdated(float v)
    {
        var topSize = GetSize(v, true);
        var bottomSize = GetSize(v, false);
        
        top.rectTransform.SetSizeWithCurrentAnchors(vertical ? RectTransform.Axis.Vertical : RectTransform.Axis.Horizontal, topSize);
        bottom.rectTransform.SetSizeWithCurrentAnchors(vertical ? RectTransform.Axis.Vertical : RectTransform.Axis.Horizontal, bottomSize);
        var textRectTransform = valueText.rectTransform;
        var textPos = textRectTransform.anchoredPosition;
        var textHeight = textRectTransform.rect.height;
        if (vertical)
            textPos.y = Mathf.Clamp(bottomSize - textHeight, 0, TotalSize - textHeight * 2);
        else textPos.x = bottomSize;
        textRectTransform.anchoredPosition = textPos;

        if (showDecimal)
            valueText.text = MultipliedValueFloat.ToString("F2");
        else valueText.text = Mathf.RoundToInt(MultipliedValueInt).ToString();
        
        onValueChange?.Invoke(MultipliedValueFloat);
    }

    bool _dragging;
    public void OnDrag(PointerEventData eventData)
    {
        if (vertical)
            value += Utils.ScaledScreenCoords(eventData.delta, transform).y / rect.rect.height;
        else value += Utils.ScaledScreenCoords(eventData.delta, transform).x / rect.rect.width;
    }

    public Action<float> onValueChange;
    public Action onClick;
    public Action onDragEnd;
    public void OnPointerClick(PointerEventData eventData)
    {
        if (_dragging) return;
        onClick?.Invoke();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        _dragging = true;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        _dragging = false;
        onDragEnd?.Invoke();
    }
}