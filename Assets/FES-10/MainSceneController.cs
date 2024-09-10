using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainSceneController : MonoBehaviour
{
    [SerializeField] public ScoreController scoreController;
    [SerializeField] private float _timeLimit = 0;

    private float _nowTime;

    public float NowTime
    {
        get => _nowTime;
        set => _nowTime = value;
    }

    private bool enableCountdown = false;

    private void Start()
    {
        _nowTime = _timeLimit;
        enableCountdown = false;
    }

    private void Update()
    {
        if (!enableCountdown) return;
        // �J�E���g�_�E��
        _nowTime -= Time.deltaTime;
        // �J�E���g�[���ŃQ�[���C��
        if (_nowTime < 0) { 
            _nowTime = 0;
            FinishGame();
        }
    }

    /// <summary>
    /// �Q�[���J�n������
    /// </summary>
    private void StartGame()
    {
        enableCountdown = true;
    }

    /// <summary>
    /// �Q�[���I��������
    /// </summary>
    private void FinishGame()
    {
        enableCountdown = false;
    }
}
