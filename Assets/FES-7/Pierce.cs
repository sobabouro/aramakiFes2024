using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pierce : MonoBehaviour
{
    [SerializeField]
    private int durabilityRecoveryAmount;

    public int Connect(Breaker breaker)
    {
        this.gameObject.transform.SetParent(breaker.GetContainer());        // ���g�̐e��Breaker.container�ɂ���
        GameObject container = breaker.GetContainer().gameObject;           
        container.GetComponent<Container>().SetRegisteredObject(this.gameObject);   // Container�N���X�̓o�^�I�u�W�F�N�g�����g�ɂ���

        return durabilityRecoveryAmount; // �񕜂���ϋv�l��Ԃ�
    }

    private Vector3 DecideConnectPosition()
    {
        Vector3 connectPosition = new Vector3();

        return connectPosition;
    }
}
