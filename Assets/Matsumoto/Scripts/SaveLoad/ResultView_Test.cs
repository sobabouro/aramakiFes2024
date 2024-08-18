using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ResultView_Test : MonoBehaviour
{
    // debug�p�̃f�[�^�\���p�e�L�X�g
    [SerializeField]
    private TMPro.TMP_Text debugText;

    // �v���C���[�̃X�R�A�\���p�e�L�X�g
    [SerializeField]
    private TMPro.TMP_Text playerScore;

    // �����L���O�̃X�R�A��\���p�e�L�X�g���X�g
    [SerializeField]
    private List<TMPro.TMP_Text> rankingScoreList;


    // �f�[�^��ێ����Ă���X�N���v�g
    [SerializeField]
    private CreateNewData createNewData;

    void Start()
    {
        ShowScore();
    }

    // ���ݕۑ����Ă���X�R�A��\������
    private void ShowScore()
    {
        playerScore.SetText(createNewData.GetSaveData().GetScoreData().GetScore().ToString());
    }

    // ���ݕۑ����Ă����JSON�f�[�^��\������
    public void ShowJsonData()
    {
        debugText.SetText(createNewData.GetSaveData().GetJsonData());
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
        createNewData.GetSaveData().SetUserComment(userComment);
    }

    // PlayerPrefs����擾�����f�[�^��SaveData�̃f�[�^�ɏ㏑������
    public void LoadFromJsonOverwrite()
    {
        
        if (PlayerPrefs.HasKey("PlayerData"))
        {
            var data = PlayerPrefs.GetString("PlayerData");
            JsonUtility.FromJsonOverwrite(data, createNewData.GetSaveData());
            debugText.SetText(createNewData.GetSaveData().GetJsonData());
        }
    }

    // �ۑ����Ă���Z�[�u�f�[�^�����̂܂ܕ\��
    public void ShowSaveData()
    {
        
        if (PlayerPrefs.HasKey("PlayerData"))
        {
            var data = PlayerPrefs.GetString("PlayerData");
            SaveData otherSaveData = JsonUtility.FromJson<SaveData>(data);
            debugText.SetText(otherSaveData.GetJsonData());
        }
    }

    // �f�[�^���폜����
    private void DeleteData()
    {
        
        PlayerPrefs.DeleteKey("PlayerData");
    }

    private void ResetDebugText()
    {
        debugText.SetText("");
    }
}
