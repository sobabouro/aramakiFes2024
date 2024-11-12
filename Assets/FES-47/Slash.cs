using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Slash : MonoBehaviour
{
    // �c��ؒf�\��
    [SerializeField]
    private int numberOfCanSlash = 2;

    [SerializeField, Tooltip("�ؒf�ʗp�̃}�e���A��")]
    Material surfaceMat;

    // �I�u�W�F�N�g�ؒf���ɌĂяo���C�x���g�o�^
    public UnityEvent onSlashEvent;

    // �I�u�W�F�N�g�j�󎞂ɌĂяo���C�x���g�o�^
    public UnityEvent onBreakEvent;

    // �ؒf�N���X�̌Ăяo�����ɂ͂��߂ɌĂяo����AActSubdivide�ɐؒf������
    public void CallSlash(Breaker breaker)
    {
        if (numberOfCanSlash <= 0)
        {
            Destroy(this.gameObject);

            // �j�󎞂ɌĂяo�����C�x���g���Ăяo��
            onBreakEvent?.Invoke();
        }
        else
        {
            numberOfCanSlash--;

            // ActSubdivide���ŃI�u�W�F�N�g�̔j���͂����Ȃ���i�͂��j
            ActSubdivide.Subdivide(this.gameObject, breaker.GetCutter(), surfaceMat);

            // �ؒf���ɌĂяo�����C�x���g���Ăяo��
            onSlashEvent?.Invoke();
        }

    }
}
