using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MixedReality.Toolkit.SpatialManipulation;
using UnityEngine.Events;

public class Pierce : MonoBehaviour
{
    [SerializeField, Tooltip("�񕜑ϋv�l")]
    private int durabilityRecoveryAmount;
    [SerializeField, Tooltip("�񕜃X�R�A")]
    private int scoreRecoveryAmount;
    [SerializeField, Tooltip("�����\�H")]
    private bool canConnect;
    // �������Ă���H
    private bool isConnected = false;

    // �I�u�W�F�N�g�j�󎞂ɌĂяo���C�x���g�o�^
    public UnityEvent onBreakEvent;
    // �I�u�W�F�N�g�������ɌĂяo���C�x���g�o�^
    public UnityEvent onConnectEvent;

    // ����������W�̐ݒ�
    private void DecideConnectPosition()
    {

    }

    /// <summary>
    /// �h�ˑ����ɂ�錋���̊J�n
    /// </summary>
    /// <param name="breaker">�󂷂��̃N���X</param>
    /// <returns>�񕜂���ϋv�l�A�X�R�A</returns>
    public (int, int) Connect(Breaker breaker)
    {
        // �R���C�_�[�̎擾
        Collider breakerCollider = breaker.gameObject.GetComponent<Collider>();
        Collider myCollider = this.gameObject.GetComponent<Collider>();

        // �����ł��Ȃ��I�u�W�F�N�g�̏ꍇ
        if (!canConnect)
        {
            // �j�󎞂ɌĂяo�����C�x���g���Ăяo��
            onBreakEvent?.Invoke();
            return (0, 0);
        }

        // ���Ɍ������Ă���I�u�W�F�N�g�ɑ΂��āA�h�ˑ����ōĂщ󂵂��ꍇ
        if(isConnected)
        {
            isConnected = false;
            // ���������I�u�W�F�N�g�Ԃ̏Փ˔���̗L����
            Physics.IgnoreCollision(myCollider, breakerCollider, false);

            // �j�󎞂ɌĂяo�����C�x���g���Ăяo��
            onBreakEvent?.Invoke();

            return (0, 0);
        }

        /*
                // �����Ō�������I�u�W�F�N�g�̍��W�𒲐�����
        */

        // �I�u�W�F�N�g�̓����̈ˑ��Ώۂ̐ݒ�
        FixedJoint fixedJoint = this.gameObject.GetComponent<FixedJoint>(); ;
        if (fixedJoint == null)
        {
            fixedJoint = this.gameObject.AddComponent<FixedJoint>();
        }
        fixedJoint.connectedBody = breaker.GetRigidbody();

        isConnected = true;

        // ���������I�u�W�F�N�g�Ԃ̏Փ˔���̖�����
        Physics.IgnoreCollision(myCollider, breakerCollider, true);

        // �������ɌĂяo�����C�x���g���Ăяo��
        onConnectEvent?.Invoke();

        // �񕜂���ϋv�l��Ԃ�
        return (durabilityRecoveryAmount, scoreRecoveryAmount);
    }
}