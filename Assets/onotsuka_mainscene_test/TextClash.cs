using UnityEngine;

public class TextClash : MonoBehaviour {
	[SerializeField, Tooltip("�Đ�����A�j���[�V���������� Animator")]
	private Animator animator;

	[SerializeField, Tooltip("�A�j���[�V�����̃g���K�[��")]
	private string animationTriggerName = "PlayAnimation";

	private void OnCollisionEnter(Collision collision) {
		// �Փˎ��ɃA�j���[�V�������Đ�
		if (animator != null) {
			animator.SetTrigger(animationTriggerName);
		}
	}
}
