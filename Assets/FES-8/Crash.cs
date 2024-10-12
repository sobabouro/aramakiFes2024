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
    private List<GameObject> _brokenObjectPrefabList;
    // �I�u�W�F�N�g�j�󎞂ɌĂяo���C�x���g�o�^
    public UnityEvent onBreakEvent;
    // �j���̃I�u�W�F�N�g���Ăяo���ۂɉ�����O�����̗�
    [SerializeField]
    private float _addImpulse = 1;

    // Start is called before the first frame update
    void Start()
    {
        onBreakEvent.AddListener(DebugMessage);
    }

    // �󑮐��ɂ��I�u�W�F�N�g�̔j�󏈗����Ăяo�����ۂɌĂяo��
    public void CallCrash()
    {
        // ���g�̓����蔻�������������
        this.gameObject.GetComponent<Collider>().enabled = false;


        // �j�󎞂ɌĂяo�����C�x���g���Ăяo��
        onBreakEvent?.Invoke();

        // �t���O�ɂ���Ĕj���̃I�u�W�F�N�g���Ăяo�����肷��
        if (_canCallBrokenObject)
        {
            CallBrokenObject();
        }

        // �I�u�W�F�N�g��j�󂷂�
        Debug.Log("CrashDestroy! : " + this.gameObject);
        Destroy(this.gameObject);
    }

    // �j���ɃI�u�W�F�N�g�����ۂɌĂяo��
    private void CallBrokenObject()
    {
        Debug.Log("CallBrokenObject!");
        // �j���ɌĂяo���I�u�W�F�N�g�𐶐����āA�O���Ɍ����Ă�����x�̗�(_addForce)�����ăI�u�W�F�N�g�𓮂���
        foreach (GameObject targetObject in _brokenObjectPrefabList)
        {
            GameObject createObject = Instantiate(targetObject, this.gameObject.transform.position, this.gameObject.transform.rotation);

            Rigidbody rigidbody = createObject.GetComponent<Rigidbody>();
            Vector3 insideUnitSphere = Random.insideUnitSphere; // ���a 1 �̋��̂̓����̃����_���ȓ_��Ԃ��܂�

            rigidbody.AddForce(_addImpulse * insideUnitSphere, ForceMode.Impulse);
        }
    }

    private void DebugMessage()
    {
        Debug.Log("onBreakEvent.Invoke!");
    }

}
