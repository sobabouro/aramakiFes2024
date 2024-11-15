using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class tutorial_TakeWeapon : MonoBehaviour
{
    [SerializeField, Tooltip("�\������UI�e�L�X�g")]
    private TMP_Text messageText; // TextMeshPro�p�̃t�B�[���h

    [SerializeField, Tooltip("EquipWeapons �X�N���v�g���A�^�b�`���ꂽ�I�u�W�F�N�g")]
    private EquipWeapons equipWeapons; // EquipWeapons �X�N���v�g�ւ̎Q��

    [SerializeField, Tooltip("�������Ƃ��̃��b�Z�[�W")]
    private string holdingMessage = "You are holding a weapon!";

    [SerializeField, Tooltip("�ʏ펞�̃��b�Z�[�W")]
    private string defaultMessage = "No weapon equipped.";

    private static bool isGameStart = false;

    /// <summary>
    /// �Q�[���J�n���Ƀf�t�H���g�̃��b�Z�[�W��ݒ�
    /// </summary>
    private void Start()
    {
        if (messageText != null)
        {
            messageText.text = defaultMessage;
        }
    }

    /// <summary>
    /// ���t���[������̑�����Ԃ��`�F�b�N
    /// </summary>
    private void Update()
    {
        if (isGameStart) return;

        if (equipWeapons != null && messageText != null)
        {
            // ����̑�����Ԃ��m�F
            if (equipWeapons.GetIsEquipWeapon())
            {
                messageText.text = holdingMessage;
            }
            else
            {
                messageText.text = defaultMessage;
            }
        }
        else if (equipWeapons == null)
        {
            Debug.LogWarning("EquipWeapons script reference is not set in the inspector.");
        }
    }

    public void SetIsGameStart()
    {
        isGameStart = true;
    }
}
