using UnityEngine;

public class SoundsPlayer : MonoBehaviour
{
    [SerializeField] AudioSource insertMain, insertStart, undo, moveAttached, moveAttachedRotate, playTheme0;
    
    public static SoundsPlayer instance;
    public void Awake()
    {
        instance = this;
    }

    public void PlayInsertSound()
    {
        // insertMain.pitch = Random.Range(0.95f, 1.05f);
        insertMain.Play();
    }

    public void PlayInsertStartSound()
    {
        insertStart.Play();
    }

    public void PlayUndoSound()
    {
        undo.Play();
    }

    public void PlayMoveAttachedSound()
    {
        moveAttached.Play();
    }

    public void PlayMoveAttachedRotateSound()
    {
        moveAttachedRotate.Play();
    }

    public void EnablePlayTheme(bool value)
    {
        if (value)
            playTheme0.Play();
        else playTheme0.Stop();
    }
}