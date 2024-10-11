using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Crash : MonoBehaviour
{
    // �j���̃I�u�W�F�N�g���Ăяo�����̃t���O
    [SerializeField]
    private bool _canCallBrokenObject;
    // �I�u�W�F�N�g�̔j���ɌĂяo�����I�u�W�F�N�g
    [SerializeField]
    private GameObject _brokenObjectPrefab;
    // �I�u�W�F�N�g�j�󎞂ɌĂяo���C�x���g�o�^
    public UnityEvent onBreakEvent;
    // �j���̃I�u�W�F�N�g���Ăяo���ۂɉ�����O�����̗�
    private Vector3 _addImpulse;

    // Start is called before the first frame update
    void Start()
    {

    }

    // �󑮐��ɂ��I�u�W�F�N�g�̔j�󏈗����Ăяo�����ۂɌĂяo��
    public void CallCrash()
    {
        /* // ���g�̓����蔻�������������
        �����ɏ������L�q
         */

        // �j�󎞂ɌĂяo�����C�x���g���Ăяo��
        onBreakEvent?.Invoke();

        // �t���O�ɂ���Ĕj���̃I�u�W�F�N�g���Ăяo�����肷��
        if (_canCallBrokenObject)
        {
            CallBrokenObject();
        }

        // �I�u�W�F�N�g��j�󂷂�
        Destroy(this.gameObject);
    }

    // �j���ɃI�u�W�F�N�g�����ۂɌĂяo��
    private void CallBrokenObject()
    {
        /* // �j���ɌĂяo���I�u�W�F�N�g�𒆐S����O���Ɍ����Ă�����x�̗�(_addForce)�����ăI�u�W�F�N�g�𐶐�����
        �����ɏ������L�q
         */
    }

}
