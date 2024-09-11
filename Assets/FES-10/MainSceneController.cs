using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;

public class MainSceneController : MonoBehaviour
{
    [SerializeField] public ScoreController scoreController;
    [SerializeField] private float _timeLimit = 120;
    [SerializeField] private TextMeshProUGUI timeText = null;

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
        // �\������
        if(timeText != null) {
            timeText.SetText(timeToString());
        }
        else {
            Debug.Log(timeToString());
        }
        enableCountdown = false;

        StartGame();
    }

    private void Update()
    {
        if (!enableCountdown) return;
        // �J�E���g�_�E��
        _nowTime -= Time.deltaTime;
        if (_nowTime < 0) _nowTime = 0;
        // �\������
        if (timeText != null) {
            timeText.SetText(timeToString());
        }
        else {
            Debug.Log(timeToString());
        }
        // �J�E���g�[���ŃQ�[���C��
        if (_nowTime == 0) {
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

    /// <summary>
    /// �b��mm:ss�`���ɂ���
    /// </summary>
    private string timeToString()
    {
        int mini = (int)_nowTime / 60;
        int sec = (int)_nowTime % 60;
        return mini + ":" + sec;
    }
}
