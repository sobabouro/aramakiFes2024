using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effect : MonoBehaviour
{
    [SerializeField, Tooltip("����������G�t�F�N�g(�p�[�e�B�N��)")]
    private ParticleSystem particle;

    [SerializeField, Tooltip("�G�t�F�N�g�𔭐�������Ώۂ̃I�u�W�F�N�g���X�g")]
    private List<GameObject> targetObjects; // �C���X�y�N�^�[�Ŏw��\�ȃI�u�W�F�N�g���X�g

    /// <summary>
    /// �Փ˂�����
    /// </summary>
    /// <param name="collision"></param>
    private void OnCollisionEnter(Collision collision)
    {
        // �Փ˂����I�u�W�F�N�g���w�胊�X�g�Ɋ܂܂�Ă���ꍇ
        if (targetObjects.Contains(collision.gameObject))
        {
            // �Փ˒n�_���擾
            ContactPoint contact = collision.contacts[0];
            Vector3 hitPosition = contact.point;
            Quaternion hitRotation = Quaternion.LookRotation(contact.normal);

            // �p�[�e�B�N���V�X�e���̃C���X�^���X�𐶐�
            ParticleSystem newParticle = Instantiate(particle);
            // �Փ˒n�_�Ƀp�[�e�B�N����z�u
            newParticle.transform.position = hitPosition;
            // �Փ˂̌����ɉ�]������
            newParticle.transform.rotation = hitRotation;
            // �p�[�e�B�N���V�X�e����GameObject�ɒǏ]�����邽�߂ɐe��ݒ肷��
            newParticle.transform.SetParent(this.transform);
            // �p�[�e�B�N���𔭐�������
            newParticle.Play();

            // ��莞�Ԍ�ɍ폜����
            Destroy(newParticle.gameObject, newParticle.main.duration);
        }
    }

}
