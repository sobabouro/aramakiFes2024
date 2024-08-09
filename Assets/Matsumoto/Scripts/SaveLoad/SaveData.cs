using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveData : object
{
    [SerializeField]
    private List<ScoreData> scoreDataList = new List<ScoreData>();

    private ScoreData scoreData;

    public void SetScoreData(int score, string userName)
    {
        scoreData = new ScoreData(score, userName);
    }

    public void SetScoreDataList(List<ScoreData> scoreDataList)
    {
        this.scoreDataList = scoreDataList;
    }

    public List<ScoreData> GetScoreDataList()
    {
        return scoreDataList;
    }

    // ���[�U�[���݂̂�ύX����
    public void SetUserName(string userName)
    {
        scoreData.SetUserName(userName);
    }

    // �S�ẴX�R�A�f�[�^�̃��[�U�[���ƃX�R�A��Ԃ�
    public string GetScoreData()
    {
        string objectString = "";
        foreach (ScoreData scoreData in scoreDataList)
        {
            objectString += "UserName: " + scoreData.GetUserName() + " Score" + scoreData.GetScore() + "\n";
        }

        return objectString;
    }

    // �t�B�[���h�l��JSOn�`���ɂ����������Ԃ�
    public string GetJsonData()
    {
        return JsonUtility.ToJson(this);
    }

}
