using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = System.Random;

public class GameManager : MonoBehaviour
{
    void Update()
    {
        Animator.Update();
        
        DebugInput();
    }

    void DebugInput()
    {

        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(0);
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            var matrix = FindObjectOfType<FieldMatrix>();
            matrix.MoveAttachedShape(false);
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            var matrix = FindObjectOfType<FieldMatrix>();
            matrix.MoveAttachedShape(true);
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            var matrix = FindObjectOfType<FieldMatrix>();
            matrix.InsertShape();
        }
    }
}