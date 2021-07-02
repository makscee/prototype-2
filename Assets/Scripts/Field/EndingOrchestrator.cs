using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EndingOrchestrator : MonoBehaviour
{
    [SerializeField] bool begin;
    [SerializeField] AudioSource audioSource;
    [SerializeField] GameObject endingCanvas;
    const int Bpm = 130;
    [SerializeField] int[] beats;
    [SerializeField] int[] endingBeats;
    [SerializeField] int endScreenSamples;

    Queue<FieldMatrix> _q = new Queue<FieldMatrix>();
    Queue<int> _beatSamples = new Queue<int>();
    int _nextBeat;
    public void Begin()
    {
        foreach (var pack in GetComponentsInChildren<FieldPack>().Reverse())
        {
            foreach (var field in pack.fields)
            {
                _q.Enqueue(field);
            }
        }
        audioSource.Play();
        _playing = true;
        GameManager.instance.isEnding = true;
        SoundsPlayer.instance.StopBg();
        foreach (var i in beats) 
            _beatSamples.Enqueue(i);

        var b = beats.Last();
        _duration = 60f / Bpm;
        for (var i = 1; i < 65; i++) 
            _beatSamples.Enqueue(b + i * 44100 * 60 / 130);

        foreach (var i in endingBeats) 
            _beatSamples.Enqueue(i);

        _nextBeat = _beatSamples.Dequeue();
    }

    bool _playing;
    float _t, _duration;
    FieldMatrix _nextField;
    void Update()
    {
        if (!_playing) return;
        if (audioSource.timeSamples > _nextBeat)
        {
            if (_nextField == null || _nextField.packId != 0)
                Beat();
            else BeatLast();
            if (_beatSamples.Count > 0)
            {
                _nextBeat = _beatSamples.Dequeue();
            }
            else
            {
                _nextBeat = int.MaxValue;
            }
        }
    }

    void Beat()
    {
        var field = _nextField;
        if (field != null)
        {
            field.Explode(1, _duration);
        }
        _nextField = _q.Dequeue();
        FieldPack.active = _nextField.Pack;
        PostFxController.Instance.LoadPackPalette(_nextField.packId);
        if (_nextField.packId != 0) _nextField.Explode(0, _duration);
    }

    bool _prepared;
    void BeatLast()
    {
        if (_prepared)
        {
            _nextField.Explode(1, _duration);
            if (_q.Count == 0)
            {
                CameraScript.instance.EndingPanOut();
                endingCanvas.SetActive(true);
                return;
            }
            _nextField = _q.Dequeue();
            _prepared = false;
        }
        else
        {
            _nextField.Explode(0, _duration / 3);
            _prepared = true;
        }
    }

    void OnValidate()
    {
        if (begin)
        {
            Begin();
        }
    }
}