using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using Random = UnityEngine.Random;

public class SoundsPlayer : MonoBehaviour
{
    [SerializeField] AudioSource fx, bg;

    [SerializeField] AudioClip insertMain,
        insertStart,
        undo,
        moveAttachedLeft,
        moveAttachedRight,
        moveAttachedRotateLeft,
        moveAttachedRotateRight,
        fieldOpen,
        fieldComplete,
        fieldClose,
        sidesOpen,
        sidesClose;
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
        fx.PlayOneShot(insertMain);
        DuckBg(0.5f);
    }

    public void PlayInsertStartSound()
    {
        fx.PlayOneShot(insertStart);
    }

    public void PlayUndoSound()
    {
        fx.PlayOneShot(undo);
    }

    public void PlayMoveAttachedSound(bool left)
    {
        if (left)
            fx.PlayOneShot(moveAttachedLeft);
        else fx.PlayOneShot(moveAttachedRight);
    }

    public void PlayMoveAttachedRotateSound(bool left)
    {
        if (left)
            fx.PlayOneShot(moveAttachedRotateLeft);
        else fx.PlayOneShot(moveAttachedRotateRight);
    }

    public void PlayFieldOpen()
    {
        fx.PlayOneShot(fieldOpen);
    }

    public void PlayFieldComplete()
    {
        fx.PlayOneShot(fieldComplete);
    }

    public void PlayFieldClose()
    {
        fx.PlayOneShot(fieldClose);
    }

    public void PlaySidesOpenSound()
    {
        fx.PlayOneShot(sidesOpen);
    }

    public void PlaySidesCloseSound()
    {
        fx.PlayOneShot(sidesClose);
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

    bool _bgStopped;
    public void StopBg()
    {
        bg.Stop();
        _bgStopped = true;
    }

    void GetNextBgClip()
    {
        bg.clip = bgClipsQueue.Dequeue();
        bgClipsQueue.Enqueue(bg.clip);
        bg.Play();
    }

    void Update()
    {
        if (!_bgStopped && !bg.isPlaying)
        {
            GetNextBgClip();
        }
    }
}