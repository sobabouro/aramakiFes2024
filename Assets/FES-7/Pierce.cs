using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MixedReality.Toolkit.SpatialManipulation;

public class Pierce : MonoBehaviour
{
    [SerializeField]
    private int durabilityRecoveryAmount;
    private bool isConnected = false;

    // ����������W�̐ݒ�
    private void DecideConnectPosition()
    {

    }

    // �h�ˑ����ɂ�錋���̊J�n
    public int Connect(Breaker breaker)
    {
        // ���Ɍ������Ă���I�u�W�F�N�g�ɑ΂��āA�h�ˑ����ōĂщ󂵂��ꍇ
        if(isConnected)
        {
            isConnected = false;
            Physics.IgnoreCollision(this.gameObject.GetComponent<Collider>(), breaker.gameObject.GetComponent<Collider>(), false);
            return 0;
        }

        /*
                // �����Ō�������I�u�W�F�N�g�̍��W�𒲐�����
        */

        // �I�u�W�F�N�g�̓����̈ˑ��Ώۂ̐ݒ�
        FixedJoint fixedJoint = this.gameObject.AddComponent<FixedJoint>();
        fixedJoint.connectedBody = breaker.GetRigidbody();

        isConnected = true;

        // ���������I�u�W�F�N�g�Ԃ̏Փ˔���̖�����
        Physics.IgnoreCollision(this.gameObject.GetComponent<Collider>(), breaker.gameObject.GetComponent<Collider>(), true);

        // �񕜂���ϋv�l��Ԃ�
        return durabilityRecoveryAmount;
    }
}