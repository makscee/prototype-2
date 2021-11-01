using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public static bool IsTrailer => instance.isTrailer;
    public static bool IsEnding => instance.isEnding;
    public bool isEnding;
    [SerializeField] bool isTrailer;
    [SerializeField] AudioMixerSnapshot game, trailer;
    [SerializeField] AudioMixer mixer;
    [SerializeField] SettingsUI settings;
    [SerializeField] HelpCanvas helpCanvas;
    [SerializeField] bool resetProgress, completeAllButOne;

    void Awake()
    {
        instance = this;
        InputSystem.onLeftPress = OnLeft;
        InputSystem.onRightPress = OnRight;
        InputSystem.onUpPress = OnUp;
        InputSystem.onDownPress = onDown;
        InputSystem.onEnterPress = OnEnter;
    }

    void Start()
    {
#if UNITY_ANDROID || UNITY_IPHONE
        Application.targetFrameRate = 60;
#endif
        if (!isTrailer)
            settings.Init();
        if (isTrailer)
        {
            StartCoroutine(TransitionToSnapshot(trailer));
            Mute();
        }
        else
            StartCoroutine(TransitionToSnapshot(game));
        if (FieldPack.active == null) return;
        FieldPack.active.SetHoveredByUnlockIndex();
        if (!Progress.IsComplete(0, 0)) 
            FieldPack.active.EnterHoveredField();
    }

    void Mute()
    {
        mixer.SetFloat(SettingsUI.MusicParam, -80f);
        mixer.SetFloat(SettingsUI.SfxParam, -80f);
    }

    IEnumerator TransitionToSnapshot(AudioMixerSnapshot s)
    {
        yield return null;
        s.TransitionTo(0f);
    }

    static void OnEnter()
    {
        if (Field != null) return;
        FieldPack.active.EnterHoveredField();
    }

    static void OnUp()
    {
        if (Field == null)
        {
            DirSelectField(0);
            return;
        }
        Field.InsertShape();
    }

    static void onDown()
    {
        if (Field == null)
        {
            DirSelectField(2);
            return;
        }
        Field.Undo();
    }

    static void OnLeft()
    {
        if (Field == null)
        {
            DirSelectField(3);
            return;
        }
        Field.MoveAttachedShape(false);
    }

    static void OnRight()
    {
        if (Field == null)
        {
            DirSelectField(1);
            return;
        }
        Field.MoveAttachedShape(true);
    }

    static void DirSelectField(int dir)
    {
        if (Field != null || FieldPack.active == null)
            return;
        
        var hovered = FieldPack.active.fields.FirstOrDefault(f => f.hovered);
        if (hovered == null) return;

        FieldMatrix next = null;
        switch (dir)
        {
            case 0:
                next = FieldPack.active.fields.FirstOrDefault(f =>
                    f.completion == FieldCompletion.Unlocked && hovered.packPositionX == f.packPositionX &&
                    hovered.packPositionY < f.packPositionY);
                break;
            case 1:
                next = FieldPack.active.fields.FirstOrDefault(f =>
                    f.completion == FieldCompletion.Unlocked && hovered.packPositionX < f.packPositionX &&
                    hovered.packPositionY == f.packPositionY);
                break;
            case 2:
                next = FieldPack.active.fields.FirstOrDefault(f =>
                    f.completion == FieldCompletion.Unlocked && hovered.packPositionX == f.packPositionX &&
                    hovered.packPositionY > f.packPositionY);
                break;
            case 3:
                next = FieldPack.active.fields.FirstOrDefault(f =>
                    f.completion == FieldCompletion.Unlocked && hovered.packPositionX > f.packPositionX &&
                    hovered.packPositionY == f.packPositionY);
                break;
        }

        if (next != null)
        {
            next.SetHovered(true);
        }
    }

    public GameObject shapeCellsParticlesContainer;
    void Update()
    {
        Animator.Update();

        // var config = GlobalConfig.Instance;
        // config.thickness = config.thicknessBase + Mathf.Sin(Time.time * config.sinTimeScale) * config.sinScale;
        DebugInput();
    }

    static FieldMatrix Field => FieldMatrix.Active;
    void DebugInput()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Exit();
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(0);
        }
        
#if UNITY_EDITOR
        
        DebugCheckNumberKeys();
        
        // if (Input.GetKeyDown(KeyCode.C) && Field != null)
        // {
        //     Progress.SetComplete(Field.packId, Field.fieldId);
        //     Field.CompleteTransition();
        // }

        if (Input.GetKeyDown(KeyCode.V) && Field == null)
        {
            foreach (var field in FieldPack.active.fields) 
                Progress.SetComplete(field.packId, field.fieldId);
            SceneManager.LoadScene(0);
        }
        if (Input.GetKeyDown(KeyCode.F))
        {
            var container = Field.shapesContainer; 
            container
                .InsertAtCurrent(ShapeSerialized.CreateFromString(new[] {"*"})
                .Deserialize());
            if (Field.attachedShape == null)
                Field.AttachShape(container.GetNext());
        }

        if (Input.GetKeyDown(KeyCode.T))
        {
            ClearProgress();
        }

        if (Input.GetKeyDown(KeyCode.Y))
        {
            Progress.ResetPackAndSave(FieldPack.active.packId);
            SceneManager.LoadScene(0);
        }

        if (Field != null && Input.GetKey(KeyCode.LeftControl))
        {
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                Field.Size++;
                Field.CreateCells();
            } else if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                Field.Size--;
                Field.CreateCells();
            }
        }
#endif
    }

    void OnValidate()
    {
        if (resetProgress)
        {
            Progress.ResetAndSave();
            resetProgress = false;
        }

        if (completeAllButOne)
        {
            for (var i = 0; i < 17; i++)
            {
                var jMax = 8;
                if (i == 0)
                    jMax = 4;
                if (i > 8)
                    jMax = 1;
                for (var j = 0; j < jMax; j++) Progress.SetComplete(i, j);
            }
            Progress.UnsetComplete(17, 0);

            completeAllButOne = false;
        }
    }

    public void ClearProgress()
    {
        Progress.ResetAndSave();
        SceneManager.LoadScene("entry");
        Animator.Reset();
    }

    public void LoadGame()
    {
        SceneManager.LoadScene("game");
        Animator.Reset();
    }

    public void LoadTrailer()
    {
        SceneManager.LoadScene("trailer");
        Animator.Reset();
    }

    public void Exit()
    {
        if (helpCanvas.isEnabled)
        {
            helpCanvas.Enable(false);
        } else if (settings.gameObject.activeSelf)
        {
            settings.gameObject.SetActive(false);
        }
        else if (FieldMatrix.Active != null)
        {
            FieldMatrix.Active = null;
            SoundsPlayer.instance.PlayFieldClose();
        }
        else
        {
            Application.Quit();
        }
    }

    public void Quit()
    {
        Application.Quit();
    }

    static void DebugCheckNumberKeys()
    {
        const int key1 = (int)KeyCode.Alpha1;
        const int keyf1 = (int)KeyCode.F1;
        for (var i = 0; i < 9; i++)
        {
            if (Input.GetKeyDown((KeyCode)(key1 + i)))
            {
                FieldPacksCollection.DebugActivateFieldPack(i);
            }
            if (Input.GetKeyDown((KeyCode)(keyf1 + i)))
            {
                FieldPacksCollection.DebugActivateFieldPack(i + 9);
            }
        }
    }
}
