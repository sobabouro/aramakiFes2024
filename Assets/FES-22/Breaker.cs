using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// breakable.cs �Œ�`����
// public enum Type { plane, slash, crash, pierce }

public class Breaker : MonoBehaviour
{
    [SerializeField, Tooltip("��b�U����")]
    private int _baseATK = default;
    [SerializeField, Tooltip("����")]
    private Type _type = Type.plane;
    // ���x���擾���邽�߂�Rigidbody
    [SerializeField]
    private Rigidbody my_rigidbody;
    // �_���[�W���������邽�߂ɕK�v�ȍŒ���̑��x
    [SerializeField]
    private float _velocity_threshold = 0;

    // ���������Őؒf����ꍇ�ɕK�v�Ȍ��݂ƈ�t���O�̍��W
    private Vector3 prePos = Vector3.zero;
    private Vector3 prePos2 = Vector3.zero;

    private Plane cutter;

    public Type Type { get { return _type; } }

    private void Start()
    {

    }

    void FixedUpdate()
    {
        prePos = prePos2;
        prePos2 = transform.position;
    }

    private int CalcATK(Vector3 other_velocity)
    {
        float velocity = (my_rigidbody.velocity - other_velocity).magnitude;
        if (velocity < _velocity_threshold) velocity = 0;
        int finalATK = (int)(_baseATK * velocity);
        return finalATK;
    }

    //  �ؒf�N���X�p�̐ؒf���ʌv�Z
    private void CalcCutter(Collision collision)
    {
        // �Փ˓_�̃��[���h���W���擾
        ContactPoint contactPoint = collision.contacts[0];
        Vector3 collisionPositionWorld = contactPoint.point;

        // �Փˑ���̃��[�J�����W�ɕϊ�
        Vector3 collisionPositionLocal = collision.transform.InverseTransformPoint(collisionPositionWorld);

        // �J�b�^�[�̖@���x�N�g�������[���h��ԂŌv�Z
        Vector3 worldNormal = Vector3.Cross(transform.forward.normalized, prePos - transform.position).normalized;

        // ���ʂ̋������v�Z�F���ʂ̖@���x�N�g�����烏�[���h��Ԃ̔C�ӂ̓_�i�Ⴆ�� collisionPositionWorld�j�ւ̋���
        float worldDistance = Vector3.Dot(worldNormal, collisionPositionWorld);

        // �J�b�^�[�̕��ʂ����[���h���W�Őݒ�
        cutter = new Plane(worldNormal, worldDistance);
    }

    /// <summary>
    /// �U�����郁�\�b�h�B�I�u�W�F�N�g�ƏՓˎ��ɌĂяo���B
    /// </summary>
    /// <param name="collision">�Փ˃f�[�^�S��</param>
    public void Attack(Collision collision)
    {
        Breakable breakable = collision.gameObject.GetComponent<Breakable>();
        
        if (breakable == null) return;

        Rigidbody otherRigitbody = collision.gameObject.GetComponent<Rigidbody>();
        int finalATK = CalcATK(otherRigitbody.velocity);
        
        breakable.ReciveAttack(finalATK, this);

        // �ؒf�N���X�p�̕��ʌv�Z�Ăяo��
        CalcCutter(collision);

        Debug.Log("Attack! : " + this.gameObject + " to " + breakable + " : " + finalATK + " : " + otherRigitbody.velocity + " : " + my_rigidbody.velocity);
    }

    public void SetRigidbody(Rigidbody rigidbody)
    {
        my_rigidbody = rigidbody;
    }

    public Rigidbody GetRigidbody()
    {
        return my_rigidbody;
    }

    public Plane GetCutter()
    {
        return cutter;
    }
}
