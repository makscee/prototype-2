using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class TouchInputObject : MonoBehaviour, IDragHandler, IBeginDragHandler
{
    Vector2 _startPos, _curPos, _prevPos;
    bool _horizontalSwipe;

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
        
        if (Mathf.Abs(_curPos.x - _startPos.x) > 0.1f)
        {
            if (_curPos.x > _startPos.x)
                FieldMatrix.Active.MoveAttachedShape(true);
            else FieldMatrix.Active.MoveAttachedShape(false);
            Vibration.Vibrate(50);
            _startPos = _curPos;
        }
        if (Mathf.Abs(_curPos.y - _startPos.y) > 0.2f)
        {
            if (_curPos.y > _startPos.y)
                FieldMatrix.Active.InsertShape();
            else FieldMatrix.Active.Undo();
            Vibration.Vibrate(100);
            _startPos = _curPos;
        }

        _prevPos = _curPos;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        _startPos = eventData.pressPosition / ScreenMaxSquare;
        _prevPos = eventData.pressPosition / ScreenMaxSquare;
        _curPos = eventData.position / ScreenMaxSquare;
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
}