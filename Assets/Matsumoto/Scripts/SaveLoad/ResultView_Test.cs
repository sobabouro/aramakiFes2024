using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class ResultView_Test : MonoBehaviour
{
    // �v���C���[�̃X�R�A�\���p�e�L�X�g
    [SerializeField]
    private TMPro.TMP_Text playerScore;

    // �����L���O�̃X�R�A��\���p�e�L�X�g���X�g
    [SerializeField]
    private List<TMPro.TMP_Text> rankingScoreList = new List<TMP_Text>();

    // �v���C���[�̃R�����g�p�e�L�X�g
    [SerializeField]
    private TMPro.TMP_Text playerComment; 


    // �f�[�^��ێ����Ă���X�N���v�g
    [SerializeField]
    private CreateNewData createNewData;

    private List<ScoreData> scoreDataList;

    private ScoreData scoreData;

    void Start()
    {
        playerScore.SetText("");
        playerComment.SetText("");
        InitializeScoreData();
        ShowScore();
        scoreDataList = createNewData.GetSaveData().GetScoreDataList();        
        scoreDataList.Add(scoreData);
        SortScoreDataList();
    }

    // ���ݕۑ����Ă���X�R�A��\������
    private void ShowScore()
    {
        playerScore.SetText(scoreData.GetScore().ToString());
        
    }

    // ���ݕۑ����Ă����JSON�f�[�^��\������
    public void ShowJsonData()
    {
        Debug.Log(createNewData.GetSaveData().GetJsonData());
    }

    // ����̃X�R�A�f�[�^���܂߂��f�[�^���Z�[�u����
    public void Save()
    {
        
        PlayerPrefs.SetString("PlayerData", createNewData.GetSaveData().GetJsonData());
    }

    // ����̃v���C��userComment��ݒ肷��
    public void SetuserComment(string userComment)
    {
        playerComment.SetText(userComment);
        scoreData.SetUserComment(userComment);
    }

    // PlayerPrefs����擾�����f�[�^��SaveData�̃f�[�^�ɏ㏑������
    public void LoadFromJsonOverwrite()
    {
        
        if (PlayerPrefs.HasKey("PlayerData"))
        {
            var data = PlayerPrefs.GetString("PlayerData");
            JsonUtility.FromJsonOverwrite(data, createNewData.GetSaveData());
            Debug.Log(createNewData.GetSaveData().GetJsonData());
        }
    }

    // �ۑ����Ă���Z�[�u�f�[�^�����̂܂ܕ\��
    public void ShowSaveData()
    {
        
        if (PlayerPrefs.HasKey("PlayerData"))
        {
            var data = PlayerPrefs.GetString("PlayerData");
            SaveData otherSaveData = JsonUtility.FromJson<SaveData>(data);
            Debug.Log(otherSaveData.GetJsonData());
        }
    }

    // �f�[�^���폜����
    private void DeleteData()
    {
        Debug.Log("SaveDataDeleate!");
        PlayerPrefs.DeleteKey("PlayerData");
    }

    // ScoreData�̏�����
    public void InitializeScoreData()
    {
        int score = PlayerPrefs.GetInt("Score", 0);
        string userComment = "";
        scoreData = new ScoreData(score, userComment);
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
