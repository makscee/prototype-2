using System;
using System.Collections.Generic;
using UnityEngine;

public class TrailerOrchestrator : MonoBehaviour
{
    [SerializeField] AudioSource audioSource;
    const int Bpm = 130, SampleStart = 133120;
    int _curBeat = -1;
    bool _isDone;
    int BeatNum
    {
        get
        {
            var raw = (audioSource.timeSamples - SampleStart + 44100 * GlobalConfig.Instance.shapeAnimationTime) /
                      (44100f * 60 / Bpm);
            return raw < 0 ? -1 : Mathf.FloorToInt(raw);
        }
    }

    [SerializeField] bool _init;
    void OnValidate()
    {
        if (_init)
        {
            _init = false;
            Init();
        }
    }

    void Init()
    {
        var x = 0;
        foreach (var fieldMatrix in GetComponentsInChildren<FieldMatrix>())
        {
            fieldMatrix.transform.position = new Vector3(x, 0, 0);
            x++;
        }
    }

    [SerializeField] List<TrailerAutoInserter> inserters;
    [SerializeField] List<FieldMatrix> fields;
    FieldMatrix[] _fieldsCopy;
    void Start()
    {
        fields[0].SetScreenState(FieldScreenState.Active);
        for (var i = 1; i < fields.Count; i++)
        {
            fields[i].SetScreenState(FieldScreenState.Disabled);
        }
        inserters[0].MoveToNext();
        audioSource.Play();
        _fieldsCopy = fields.ToArray();
    }

    [SerializeField] float doneWait = 8f;
    void Update()
    {
        if (!_isDone && _curBeat != BeatNum && BeatNum >= 0) Beat();
        if (_isDone)
        {
            doneWait -= Time.deltaTime;
            if (doneWait < 0f)
            {
                GameManager.instance.LoadGame();
            }
        }
    }

    void Beat()
    {
        _curBeat = BeatNum;
        if (inserters[0].isDone)
        {
            var inserter = inserters[0];
            var field = fields[0];
            inserters.RemoveAt(0);
            fields.RemoveAt(0);
            if (fields.Count == 0)
            {
                _isDone = true;
                foreach (var f in _fieldsCopy)
                {
                    f.gameObject.SetActive(true);
                }

                FieldMatrix.Active = null;
                var cs = CameraScript.instance;
                cs.SetSizeTarget(5.5f);
                cs.targetPosition = Vector2.zero;
                cs.targetRotation = Quaternion.identity;
                PostFxController.Instance.LoadPackPalette(0);
                return;
            }
            inserter.gameObject.SetActive(false);
            field.SetScreenState(FieldScreenState.Disabled);
            fields[0].SetScreenState(FieldScreenState.Active);
            inserters[0].MoveToNext();
            PostFxController.Instance.LoadPackPalette(fields[0].fieldId);
            return;
        }
        inserters[0].Insert();
        if (!inserters[0].isDone)
            Animator.Invoke(inserters[0].MoveToNext).In(60f / Bpm / 2);
    }
}