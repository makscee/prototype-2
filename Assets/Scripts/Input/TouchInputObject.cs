using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class TouchInputObject : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler, IPointerClickHandler
{
    Vector2 _startPos, _curPos, _prevPos;
    bool _horizontalSwipe, _done, _dragging;
    const float SquarePercent = 0.1f;

    public static void SetEnabled(bool value)
    {
        if (_instance == null) return;
        _instance.gameObject.SetActive(value);
    }
    static TouchInputObject _instance;

    void Awake()
    {
        _instance = this;
        gameObject.SetActive(false);
    }

    public void OnDrag(PointerEventData eventData)
    {
        var touchPos = eventData.position / ScreenMaxSquare;
        var delta = touchPos - _prevPos;
        if (Mathf.Abs(delta.x) > Mathf.Abs(delta.y))
            delta.y = 0;
        else delta.x = 0;
        _curPos += delta;
        
        if (!_done && Mathf.Abs(_curPos.x - _startPos.x) > SquarePercent)
        {
            if (_curPos.x > _startPos.x)
                FieldMatrix.Active.MoveAttachedShape(true);
            else FieldMatrix.Active.MoveAttachedShape(false);
            Vibration.Vibrate(25);
            _startPos = _curPos;
            _horizontalSwipe = true;
        }
        if (!_horizontalSwipe && !_done && Mathf.Abs(_curPos.y - _startPos.y) > SquarePercent * 2)
        {
            if (_curPos.y > _startPos.y)
                FieldMatrix.Active.InsertShape();
            else FieldMatrix.Active.Undo();
            Vibration.Vibrate(100);
            _startPos = _curPos;
            _done = true;
        }

        _prevPos = _curPos;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        _dragging = true;
        _startPos = eventData.pressPosition / ScreenMaxSquare;
        _prevPos = eventData.pressPosition / ScreenMaxSquare;
        _curPos = eventData.position / ScreenMaxSquare;
        _horizontalSwipe = false;
        _done = false;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        _dragging = false;
    }

    static Vector2 _maxSquareCache;

    static Vector2 ScreenMaxSquare
    {
        get
        {
            if (_maxSquareCache == Vector2.zero) 
                _maxSquareCache = Screen.width > Screen.height ? new Vector2(Screen.height, Screen.height) : 
                    new Vector2(Screen.width, Screen.width);
            return _maxSquareCache;
        }
    }

    public static Action onTap;
    public void OnPointerClick(PointerEventData eventData)
    {
        if (_dragging)
            return;
        onTap?.Invoke();
        onTap = null;
    }
}