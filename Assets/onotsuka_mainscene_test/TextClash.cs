using UnityEngine;

public class TextClash : MonoBehaviour {
	// ���̃X�N���v�g���A�^�b�`����Ă���I�u�W�F�N�g�����̃I�u�W�F�N�g�ƏՓ˂����Ƃ��ɌĂяo�����
	[SerializeField]
	private GameObject mainObject;
	[SerializeField]
	private GameObject otherObject;
	private void OnCollisionEnter(Collision collision) {

		// �����̃I�u�W�F�N�g�̃A�N�e�B�u��Ԃ𔽓]
		mainObject.SetActive(!gameObject.activeSelf); // �������g�̃A�N�e�B�u��Ԃ𔽓]
		otherObject.SetActive(!otherObject.activeSelf);
	}
}
