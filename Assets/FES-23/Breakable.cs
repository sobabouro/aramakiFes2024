using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Breakable : MonoBehaviour
{
	[SerializeField, Tooltip("�ϋv�l")]
	private int durability = default;
	[SerializeField, Tooltip("�ؒf�ϐ�"), Header("�����ϐ�")]
	private int slashResist = default;
	[SerializeField, Tooltip("�Ռ��ϐ�"),]
	private int crashResist = default;
	[SerializeField, Tooltip("�ђʑϐ�"),]
	private int pierceResist = default;
	[SerializeField, Tooltip("�X�R�A")]
	private int score = default;

}
