using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MixedReality.Toolkit.SpatialManipulation;

public class EquipWeapons : MonoBehaviour
{
    [SerializeField]
    private GameObject container;

    private Collider weaponCollider = null;
    private bool isEquipWeapon = false;
    private int breakableObject_mass = 1000;
    private Color originalColor;  // ���̐F��ێ�

    // Start is called before the first frame update
    void Start()
    {
        originalColor = this.gameObject.GetComponent<MeshRenderer>().material.color;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void EquipWeapon()
    {
        if (isEquipWeapon)
        {
            // ���ɕ���𑕔����Ă���ꍇ�̏����i������̂Ă�j
            foreach (Transform child in container.transform)
            {
                // Rigidbody�̒ǉ��ƒ���
                Rigidbody rigidbody = child.gameObject.AddComponent<Rigidbody>();
                rigidbody.useGravity = true;
                rigidbody.mass = breakableObject_mass;

                // HoloLens2�ł̑���ł̍��W�ړ��̑Ώۂ�container�ɂ���
                if (child.gameObject.GetComponent<ObjectManipulator>() != null)
                {
                    child.gameObject.GetComponent<ObjectManipulator>().HostTransform = child.gameObject.transform;
                }

                // Breaker�N���X�ɕۑ������rigidbody�ɓo�^
                child.gameObject.GetComponent<Breaker>().SetRigidbody(rigidbody);

                child.transform.parent = null;
            }

            this.gameObject.GetComponent<MeshRenderer>().material.SetColor("_Color", originalColor);
        }
        else
        {
            // ����𑕔����Ă��Ȃ��Ƃ��̏����i����𑕔�����j
            if (weaponCollider == null) return;
            if (weaponCollider.gameObject.GetComponent<Breaker>() == null) return;

            // �R���e�i�̎q�I�u�W�F�N�g�ɂ����rigidbody�̔j��
            Rigidbody rigidbody = this.gameObject.GetComponent<Rigidbody>();
            Destroy(rigidbody);
            // ���g�̐e�̐ݒ�
            weaponCollider.gameObject.transform.parent = container.transform;
            // ���W�̒���
            weaponCollider.gameObject.transform.position = container.transform.position;
            weaponCollider.gameObject.transform.rotation = container.transform.rotation;
            // Container�N���X�̓o�^�I�u�W�F�N�g�����g�ɂ���
            container.GetComponent<Container>().SetRegisteredObject(weaponCollider.gameObject);

            this.gameObject.GetComponent<MeshRenderer>().material.SetColor("_Color", new Color(originalColor.r, originalColor.g, originalColor.b, 0.0f));
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        weaponCollider = other;
    }

    private void OnTriggerExit(Collider other)
    {
        weaponCollider = null;
    }
}
