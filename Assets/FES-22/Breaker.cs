using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Breaker : MonoBehaviour
{
    [SerializeField, Tooltip("��b�U����")]
    private int baseATK = default;
    [SerializeField, Tooltip("����")]
    private Type type = Type.plane;
    private enum Type
    {
        plane,
        slash,
        crash,
        pierce
    }

    // ���x���擾���邽�߂�Rigitbody
    [SerializeField]
    private Rigidbody my_Rigitbody;

}
