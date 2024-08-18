using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[System.Serializable]
public class SaveData : object
{
    [SerializeField]
    private List<ScoreData> scoreDataList = new List<ScoreData>();

    private ScoreData scoreData;

    void start()
    {
        InitializeScoreData();
    }

    // ScoreData�̏�����
    private void InitializeScoreData()
    {
        int score = PlayerPrefs.GetInt("Score");
        string userComment = "";
        scoreData = new ScoreData(score, userComment);
    }

    public void SetScoreData(int score, string userComment)
    {
        scoreData = new ScoreData(score, userComment);
    }

    public void SetScoreDataList(List<ScoreData> scoreDataList)
    {
        this.scoreDataList = scoreDataList;
    }

    public List<ScoreData> GetScoreDataList()
    {
        return scoreDataList;
    }

    // ���[�U�[�R�����g�݂̂�ύX����
    public void SetUserComment(string userComment)
    {
        scoreData.SetuserComment(userComment);
    }

    public string GetUserComment()
    {
        return scoreData.GetUserComment();
    }

    // ���݂̃X�R�A�f�[�^��Ԃ�
    public ScoreData GetScoreData()
    {
        return scoreData;
    }

    // �t�B�[���h�l��JSON�`���ɂ����������Ԃ�
    public string GetJsonData()
    {
        return JsonUtility.ToJson(this);
    }

    // �����L���O���X�R�A���傫�����Ƀ\�[�g����
    public void SortScoreDataList()
    {
        var c = new Comparison<ScoreData>(Compare);
        scoreDataList.Sort(c);
    }


    static int Compare(ScoreData a, ScoreData b)
    {
        return b.GetScore() - a.GetScore();
    }
}
