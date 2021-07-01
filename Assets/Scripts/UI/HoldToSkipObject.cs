using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class HoldToSkipObject : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
{
    [SerializeField] float holdTime = 3f;
    [SerializeField] RectTransform bar;
    [SerializeField] RectTransform barParent;
    float _heldT;
    bool _isHolding;
    void Update()
    {
        if (Input.anyKey || _isHolding)
        {
            _heldT += Time.deltaTime;
            barParent.gameObject.SetActive(true);
        }
        else
        {
            _heldT = 0f;
            barParent.gameObject.SetActive(false);
        }
        bar.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, barParent.rect.width * _heldT / holdTime);

        if (_heldT >= holdTime)
        {
            GameManager.instance.LoadGame();
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        _isHolding = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        _isHolding = false;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        
    }
}