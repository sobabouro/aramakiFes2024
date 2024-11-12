using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class Slash : MonoBehaviour
{
    [SerializeField, Tooltip("�c��ؒf�\��")]
    private int _numberOfCanSlash = 2; 
    [SerializeField, Tooltip("�ؒf�ʗp�̃}�e���A��")]
    private Material _surfaceMat; 
    [SerializeField, Tooltip("�ؒf���ꂽ��̃I�u�W�F�N�g")]
    private GameObject _dividedObjectPrefab;

    // �I�u�W�F�N�g�ؒf���ɌĂяo���C�x���g�o�^
    public UnityEvent onSlashEvent; 
    // �I�u�W�F�N�g�j�󎞂ɌĂяo���C�x���g�o�^
    public UnityEvent onBreakEvent;

    /// <summary>
    /// �ؒf�N���X�̌Ăяo�����ɂ͂��߂ɌĂяo����AActSubdivide�ɐؒf������
    /// </summary>
    /// <param name="breaker">�U���������̏��</param>
    /// <returns></returns>
    public void CallSlash(Breaker breaker)
    {
        /*if (_numberOfCanSlash <= 0)
        {
            Destroy(this.gameObject);
            // �j�󎞂̃C�x���g���Ăяo��
            onBreakEvent?.Invoke();
        }
        else
        {
            // ���������I�u�W�F�N�g�Ɗ����Ȃ��悤��Collider�𖳌���
            this.gameObject.GetComponent<Collider>().enabled = false;
            (Mesh mesh1, Mesh mesh2) = ActSubdivide.Subdivide(this.gameObject, breaker.GetCutter());
            // �ؒf���ꂽ��̃I�u�W�F�N�g�𐶐�����
            CreateDividedObject(transform.position, mesh1);
            CreateDividedObject(transform.position, mesh2); 
            
            Destroy(this.gameObject);
            // �ؒf���̃C�x���g���Ăяo��
            onSlashEvent?.Invoke();
        }*/
    }

    /// <summary>
    /// �ؒf���ꂽ��̃I�u�W�F�N�g�𐶐�����
    /// </summary>
    /// <param name="originPosition">���I�u�W�F�N�g�̍��W</param>
    /// <param name="newMesh">�쐬�������b�V��</param>
    /// <returns></returns>
    public void CreateDividedObject(Vector3 originPosition, Mesh newMesh)
    {
        GameObject dividedObject = Instantiate(_dividedObjectPrefab, originPosition, Quaternion.identity);
        Mesh mesh = dividedObject.GetComponent<Mesh>();
        // �����ō쐬�������b�V������
        // ���W�𒲐�������etc
    }
}
