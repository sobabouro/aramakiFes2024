using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Type
{
    plane,
    slash,
    crash,
    pierce
}

public class Breaker : MonoBehaviour
{
    [SerializeField, Tooltip("��b�U����")]
    private int _baseATK = default;
    [SerializeField, Tooltip("����")]
    private Type _type = Type.plane;

    // ���x���擾���邽�߂�Rigitbody
    [SerializeField]
    private Rigidbody my_rigitbody;

    // �_���[�W���������邽�߂ɕK�v�ȍŒ���̑��x
    [SerializeField]
    private float _velocity_threshold = 0;



    private int CalcATK(float other_velocity)
    {
        float velocity = my_Rigitbody.velocity.magnitude - other_velocity;
        if (velocity < velocity_threshold) velocity = 0;
        int finalATK = (int)(baseATK * my_Rigitbody.velocity.magnitude);
        return finalATK;
    }

}
