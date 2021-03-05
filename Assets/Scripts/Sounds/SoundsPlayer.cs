using UnityEngine;

public class SoundsPlayer : MonoBehaviour
{
    [SerializeField] AudioSource insertMain, insertStart, undo, moveAttachedLeft, moveAttachedRight,
        moveAttachedRotateLeft, moveAttachedRotateRight, selectScreenTheme0;
    
    public static SoundsPlayer instance;
    public void Awake()
    {
        instance = this;
    }

    public void PlayInsertSound()
    {
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

    public void EnableSelectScreenTheme(bool value)
    {
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
                .WhenDone(selectScreenTheme0.Stop).NullCheck(gameObject).SetOwner(selectScreenTheme0);
        }
    }
}