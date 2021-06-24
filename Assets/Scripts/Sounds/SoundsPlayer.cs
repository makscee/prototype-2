using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using Random = UnityEngine.Random;

public class SoundsPlayer : MonoBehaviour
{
    [SerializeField] AudioSource insertMain, insertStart, undo, moveAttachedLeft, moveAttachedRight,
        moveAttachedRotateLeft, moveAttachedRotateRight, selectScreenTheme0, fieldOpen, fieldComplete, fieldClose, bg;

    [SerializeField] AudioClip[] bgClips;
    Queue<AudioClip> bgClipsQueue;
    
    public static SoundsPlayer instance;
    public void Awake()
    {
        instance = this;
        _curBgVolume = bg.volume;
        var arr = bgClips.ToArray();
        for (var i = arr.Length - 1; i > 0; i--)
        {
            var randomIndex = Random.Range(0, i + 1);
 
            var temp = arr[i];
            arr[i] = arr[randomIndex];
            arr[randomIndex] = temp;
        }
        bgClipsQueue = new Queue<AudioClip>(arr);
        GetNextBgClip();
        SetBgVolume(GlobalConfig.Instance.bgVolumeSelectScreen);
    }

    public void PlayInsertSound()
    {
        insertMain.Play();
        DuckBg(0.5f);
    }

    public void PlayInsertStartSound()
    {
        insertStart.Play();
    }

    public void PlayUndoSound()
    {
        undo.Play();
    }

    public void PlayMoveAttachedSound(bool left)
    {
        if (left)
            moveAttachedLeft.Play();
        else moveAttachedRight.Play();
    }

    public void PlayMoveAttachedRotateSound(bool left)
    {
        if (left)
            moveAttachedRotateLeft.Play();
        else moveAttachedRotateRight.Play();
    }

    public void EnableSelectScreenTheme(bool value)
    {
        return;
        if (value)
        {
            Animator.ClearByOwner(selectScreenTheme0);
            selectScreenTheme0.Play();
            Animator.Interpolate(0f, 1f, 3f).PassValue(v => selectScreenTheme0.volume = v)
                .NullCheck(gameObject).SetOwner(selectScreenTheme0);
        }
        else
        {
            Animator.ClearByOwner(selectScreenTheme0);
            Animator.Interpolate(1f, 0f, 3f).PassValue(v => selectScreenTheme0.volume = v)
                .WhenDone(selectScreenTheme0.Pause).NullCheck(gameObject).SetOwner(selectScreenTheme0);
        }
    }

    public void PlayFieldOpen()
    {
        fieldOpen.Play();
    }

    public void PlayFieldComplete()
    {
        fieldComplete.Play();
    }

    public void PlayFieldClose()
    {
        fieldClose.Play();
    }

    float _curBgVolume;
    public void SetBgVolume(float value)
    {
        bg.volume = value;
        _curBgVolume = value;
    }

    public void DuckBg(float release)
    {
        Animator.Interpolate(0f, 1f, release).PassValue(v => bg.volume = Mathf.Lerp(0f, _curBgVolume, v));
    }

    void GetNextBgClip()
    {
        bg.clip = bgClipsQueue.Dequeue();
        bgClipsQueue.Enqueue(bg.clip);
        bg.Play();
    }

    void Update()
    {
        if (!bg.isPlaying)
        {
            GetNextBgClip();
        }
    }
}