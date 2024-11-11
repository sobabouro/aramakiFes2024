using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slash : MonoBehaviour
{
    // �c��ؒf�\��
    [SerializeField]
    private int numberOfCanSlash = 2;

    [SerializeField, Tooltip("�ؒf�ʗp�̃}�e���A��")]
    Material surfaceMat;

    // �ؒf�N���X�̌Ăяo�����ɂ͂��߂ɌĂяo����AActSubdivide�ɐؒf������
    public void CallSlash(Breaker breaker)
    {
        if (numberOfCanSlash <= 0)
        {

        }
        else
        {
            ActSubdivide.Subdivide(breaker.gameObject, breaker.GetCutter(), surfaceMat);
        }
    }
}
