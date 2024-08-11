using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ResultView_Test : MonoBehaviour
{
    // �f�[�^�\���p�e�L�X�g
    [SerializeField]
    private TMPro.TMP_Text dataText;

    // �f�[�^��ێ����Ă���X�N���v�g
    [SerializeField]
    private CreateNewData createNewData;

    // �f�[�^�\���̃e�L�X�g����ɂ���
    public void ResetText()
    {
        dataText.SetText("");
    }

    // ���ݕۑ����Ă��郆�[�U�[���ƃX�R�A��\������
    public void ShowScoreData()
    {
        ResetText();
        dataText.SetText(createNewData.GetSaveData().GetScoreData());
    }

    // ���ݕۑ����Ă����JSON�f�[�^��\������
    public void ShowJsonData()
    {
        ResetText();
        dataText.SetText(createNewData.GetSaveData().GetJsonData());
    }

    // ����̃X�R�A�f�[�^���܂߂��f�[�^���Z�[�u����
    public void Save()
    {
        ResetText();
        PlayerPrefs.SetString("PlayerData", createNewData.GetSaveData().GetJsonData());
    }

    // ����̃v���C��userName��ݒ肷��
    public void SetUserName(string userName)
    {
        createNewData.GetSaveData().SetUserName(userName);
    }

    // PlayerPrefs����擾�����f�[�^��SaveData�̃f�[�^�ɏ㏑������
    public void LoadFromJsonOverwrite()
    {
        ResetText();
        if (PlayerPrefs.HasKey("PlayerData"))
        {
            var data = PlayerPrefs.GetString("PlayerData");
            JsonUtility.FromJsonOverwrite(data, createNewData.GetSaveData());
            dataText.SetText(createNewData.GetSaveData().GetJsonData());
        }
    }

    // �ۑ����Ă���Z�[�u�f�[�^�����̂܂ܕ\��
    public void ShowSaveData()
    {
        ResetText();
        if (PlayerPrefs.HasKey("PlayerData"))
        {
            var data = PlayerPrefs.GetString("PlayerData");
            SaveData otherSaveData = JsonUtility.FromJson<SaveData>(data);
            dataText.SetText(otherSaveData.GetJsonData());
        }
    }

    // �f�[�^���폜����
    private void DeleteData()
    {
        ResetText();
        PlayerPrefs.DeleteKey("PlayerData");
    }
}
