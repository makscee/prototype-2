using UnityEngine;

public class SoundsPlayer : MonoBehaviour
{
    [SerializeField] AudioSource insertMain, insertStart, undo, moveAttachedLeft, moveAttachedRight, moveAttachedRotateLeft, moveAttachedRotateRight, playTheme0;
    
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

    public void EnablePlayTheme(bool value)
    {
        if (value)
        {
            Animator.ClearByOwner(playTheme0);
            playTheme0.Play();
            Animator.Interpolate(0f, 1f, 3f).PassValue(v => playTheme0.volume = v)
                .NullCheck(gameObject).SetOwner(playTheme0);
        }
        else
        {
            Animator.ClearByOwner(playTheme0);
            Animator.Interpolate(1f, 0f, 3f).PassValue(v => playTheme0.volume = v)
                .WhenDone(playTheme0.Stop).NullCheck(gameObject).SetOwner(playTheme0);
        }
    }
}