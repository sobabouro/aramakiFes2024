using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effect : MonoBehaviour
{
    //<summary>���ƂȂ����v���O����
    //</summary>

    //[SerializeField, Tooltip("����������G�t�F�N�g(�p�[�e�B�N��)")]
    //private ParticleSystem particle;

    //[SerializeField, Tooltip("�G�t�F�N�g�𔭐�������Ώۂ̃I�u�W�F�N�g���X�g")]
    //private List<GameObject> targetObjects; // �C���X�y�N�^�[�Ŏw��\�ȃI�u�W�F�N�g���X�g

    ///// <summary>
    ///// �Փ˂�����
    ///// </summary>
    ///// <param name="collision"></param>
    //private void OnCollisionEnter(Collision collision)
    //{
    //    // �Փ˂����I�u�W�F�N�g���w�胊�X�g�Ɋ܂܂�Ă���ꍇ
    //    if (targetObjects.Contains(collision.gameObject))
    //    {
    //        // �Փ˒n�_���擾
    //        ContactPoint contact = collision.contacts[0];
    //        Vector3 hitPosition = contact.point;
    //        Quaternion hitRotation = Quaternion.LookRotation(contact.normal);

    //        // �p�[�e�B�N���V�X�e���̃C���X�^���X�𐶐�
    //        ParticleSystem newParticle = Instantiate(particle);
    //        // �Փ˒n�_�Ƀp�[�e�B�N����z�u
    //        newParticle.transform.position = hitPosition;
    //        // �Փ˂̌����ɉ�]������
    //        newParticle.transform.rotation = hitRotation;
    //        // �p�[�e�B�N���V�X�e����GameObject�ɒǏ]�����邽�߂ɐe��ݒ肷��
    //        newParticle.transform.SetParent(this.transform);
    //        // �p�[�e�B�N���𔭐�������
    //        newParticle.Play();

    //        // ��莞�Ԍ�ɍ폜����
    //        Destroy(newParticle.gameObject, newParticle.main.duration);
    //    }
    //}

    //[System.Serializable]
    //public struct WeaponEffect
    //{
    //    public GameObject weapon; // ����I�u�W�F�N�g
    //    public ParticleSystem particle; // ���̕���ɑΉ�����G�t�F�N�g
    //}

    //[SerializeField, Tooltip("���킲�ƂɈقȂ�G�t�F�N�g�̐ݒ�")]
    //private List<WeaponEffect> weaponEffects = new List<WeaponEffect>();

    //[SerializeField, Tooltip("�G�t�F�N�g�̏o���ʒu�����炷�I�t�Z�b�g")]
    //private Vector3 offsetPosition; // �C���X�y�N�^�[�Ŏw��ł���I�t�Z�b�g

    //private ParticleSystem overrideParticle; // UnityEvent�Őݒ肷��p�[�e�B�N���v���n�u

    ////[SerializeField]
    ////private ParticleSystem pDestroy; // �j�����̃G�t�F�N�g

    ///// <summary>
    ///// UnityEvent�o�R�Ńp�[�e�B�N���v���n�u��I�����郁�\�b�h
    ///// </summary>
    ///// <param name="particlePrefab">�I������p�[�e�B�N���v���n�u</param>
    //public void SetParticle(GameObject particlePrefab)
    //{
    //    if (particlePrefab != null)
    //    {
    //        overrideParticle = particlePrefab.GetComponent<ParticleSystem>();
    //    }
    //}

    ///// <summary>
    ///// �Փ˂������ɃG�t�F�N�g�𔭐�������
    ///// </summary>
    ///// <param name="collision"></param>
    //private void OnCollisionEnter(Collision collision)
    //{
    //    // �C���X�y�N�^�[�Őݒ肳�ꂽ���킲�Ƃ̃p�[�e�B�N��������
    //    foreach (var weaponEffect in weaponEffects)
    //    {
    //        if (weaponEffect.weapon == collision.gameObject)
    //        {
    //            // �Փˈʒu�Ɖ�]���擾
    //            ContactPoint contact = collision.contacts[0];
    //            Vector3 hitPosition = contact.point + offsetPosition;
    //            Quaternion hitRotation = Quaternion.LookRotation(contact.normal);

    //            // �D��x: UnityEvent�Őݒ肳�ꂽ�p�[�e�B�N��
    //            if (overrideParticle != null)
    //            {
    //                InstantiateAndPlayParticle(overrideParticle, hitPosition, hitRotation);
    //            }
    //            else
    //            {
    //                InstantiateAndPlayParticle(weaponEffect.particle, hitPosition, hitRotation);
    //            }
    //            return;
    //        }
    //    }
    //}

    ///// <summary>
    ///// �w�肳�ꂽ�p�[�e�B�N�����C���X�^���X�����čĐ�����
    ///// </summary>
    ///// <param name="particle">����������p�[�e�B�N��</param>
    ///// <param name="position">�G�t�F�N�g�̈ʒu</param>
    ///// <param name="rotation">�G�t�F�N�g�̉�]</param>
    //private void InstantiateAndPlayParticle(ParticleSystem particle, Vector3 position, Quaternion rotation)
    //{
    //    if (particle != null)
    //    {
    //        ParticleSystem newParticle = Instantiate(particle);
    //        newParticle.transform.position = position;
    //        newParticle.transform.rotation = rotation;
    //        newParticle.transform.SetParent(this.transform);
    //        newParticle.Play();

    //        // ��莞�Ԍ�ɍ폜
    //        Destroy(newParticle.gameObject, newParticle.main.duration);
    //    }
    //}

    ////private void OnDestroy()
    ////{
    ////    ParticleSystem dParticle = Instantiate(pDestroy);
    ////    dParticle.Play();
    ////}

    [System.Serializable]
    public struct WeaponEffect
    {
        public GameObject weapon; // ����I�u�W�F�N�g
        public ParticleSystem particle; // ���̕���ɑΉ�����G�t�F�N�g
    }

    [SerializeField, Tooltip("���킲�ƂɈقȂ�G�t�F�N�g�̐ݒ�")]
    private List<WeaponEffect> weaponEffects = new List<WeaponEffect>();

    [SerializeField, Tooltip("�L���[�u����ꂽ���̃G�t�F�N�g")]
    private ParticleSystem destroyParticle; // �L���[�u�j�󎞂̐�p�p�[�e�B�N��

    [SerializeField, Tooltip("�G�t�F�N�g�̏o���ʒu�����炷�I�t�Z�b�g")]
    private Vector3 offsetPosition; // �C���X�y�N�^�[�Ŏw��ł���I�t�Z�b�g

    private ParticleSystem overrideParticle; // UnityEvent�Őݒ肷��p�[�e�B�N���v���n�u

    /// <summary>
    /// UnityEvent�o�R�Ńp�[�e�B�N���v���n�u��I�����郁�\�b�h
    /// </summary>
    /// <param name="particlePrefab">�I������p�[�e�B�N���v���n�u</param>
    public void SetParticle(GameObject particlePrefab)
    {
        if (particlePrefab != null)
        {
            overrideParticle = particlePrefab.GetComponent<ParticleSystem>();
        }
    }

    /// <summary>
    /// �I�u�W�F�N�g���j�󂳂�钼�O�ɃG�t�F�N�g�𔭐�������
    /// </summary>
    private void OnDestroy()
    {
        if (destroyParticle != null)
        {
            Vector3 destroyPosition = transform.position + offsetPosition;
            InstantiateAndPlayParticle(destroyParticle, destroyPosition, Quaternion.identity, false);
        }
    }

    /// <summary>
    /// �Փ˂������ɃG�t�F�N�g�𔭐�������
    /// </summary>
    /// <param name="collision"></param>
    private void OnCollisionEnter(Collision collision)
    {
        // �C���X�y�N�^�[�Őݒ肳�ꂽ���킲�Ƃ̃p�[�e�B�N��������
        foreach (var weaponEffect in weaponEffects)
        {
            if (weaponEffect.weapon == collision.gameObject)
            {
                // �Փˈʒu�Ɖ�]���擾
                ContactPoint contact = collision.contacts[0];
                Vector3 hitPosition = contact.point + offsetPosition;
                Quaternion hitRotation = Quaternion.LookRotation(contact.normal);

                // �D��x: UnityEvent�Őݒ肳�ꂽ�p�[�e�B�N��
                if (overrideParticle != null)
                {
                    InstantiateAndPlayParticle(overrideParticle, hitPosition, hitRotation, true);
                }
                else
                {
                    InstantiateAndPlayParticle(weaponEffect.particle, hitPosition, hitRotation, true);
                }
                return;
            }
        }
    }

    /// <summary>
    /// �w�肳�ꂽ�p�[�e�B�N�����C���X�^���X�����čĐ�����
    /// </summary>
    /// <param name="particle">����������p�[�e�B�N��</param>
    /// <param name="position">�G�t�F�N�g�̈ʒu</param>
    /// <param name="rotation">�G�t�F�N�g�̉�]</param>
    /// <param name="setParent">�G�t�F�N�g���I�u�W�F�N�g�ɒǏ]���邩�ǂ���</param>
    private void InstantiateAndPlayParticle(ParticleSystem particle, Vector3 position, Quaternion rotation, bool setParent)
    {
        if (particle != null)
        {
            ParticleSystem newParticle = Instantiate(particle);
            newParticle.transform.position = position;
            newParticle.transform.rotation = rotation;

            // �e��ݒ肷�邩�ǂ����������Ő���
            if (setParent)
            {
                newParticle.transform.SetParent(this.transform);
            }

            newParticle.Play();

            // ��莞�Ԍ�ɍ폜
            Destroy(newParticle.gameObject, newParticle.main.duration);
        }
    }
}
