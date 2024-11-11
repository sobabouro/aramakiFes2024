using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MixedReality.Toolkit.SpatialManipulation;
using UnityEngine.Events;

public class Pierce : MonoBehaviour
{
    [SerializeField]
    private int durabilityRecoveryAmount;
    [SerializeField]
    private int scoreRecoveryAmount;
    [SerializeField]
    private bool canConnect;
    private bool isConnected = false;

    // �I�u�W�F�N�g�j�󎞂ɌĂяo���C�x���g�o�^
    public UnityEvent onBreakEvent;

    // �I�u�W�F�N�g�������ɌĂяo���C�x���g�o�^
    public UnityEvent onConnectEvent;

    // ����������W�̐ݒ�
    private void DecideConnectPosition()
    {

    }

    // �h�ˑ����ɂ�錋���̊J�n
    public (int, int) Connect(Breaker breaker)
    {
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
            Physics.IgnoreCollision(this.gameObject.GetComponent<Collider>(), breaker.gameObject.GetComponent<Collider>(), false);

            // �j�󎞂ɌĂяo�����C�x���g���Ăяo��
            onBreakEvent?.Invoke();

            return (0, 0);
        }

        /*
                // �����Ō�������I�u�W�F�N�g�̍��W�𒲐�����
        */

        // �I�u�W�F�N�g�̓����̈ˑ��Ώۂ̐ݒ�
        FixedJoint fixedJoint;
        if (this.gameObject.GetComponent<FixedJoint>() == null)
        {
            fixedJoint = this.gameObject.AddComponent<FixedJoint>();
        }
        else
        {
            fixedJoint = this.gameObject.GetComponent<FixedJoint>();
        }
        fixedJoint.connectedBody = breaker.GetRigidbody();

        isConnected = true;

        // ���������I�u�W�F�N�g�Ԃ̏Փ˔���̖�����
        Physics.IgnoreCollision(this.gameObject.GetComponent<Collider>(), breaker.gameObject.GetComponent<Collider>(), true);

        // �j�󎞂ɌĂяo�����C�x���g���Ăяo��
        onConnectEvent?.Invoke();

        // �񕜂���ϋv�l��Ԃ�
        return (durabilityRecoveryAmount, scoreRecoveryAmount);
    }
}