using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;

enum Type { plane, slash, crash, pierce }

public class Breakable : MonoBehaviour
{
	[SerializeField, Tooltip("�ϋv�l")]
	private int durability = default;
	[Header("�����ϐ�")]
	[SerializeField, Tooltip("�ؒf�ϐ�")]
	private int slashResist = default;
	[SerializeField, Tooltip("�Ռ��ϐ�"),]
	private int crashResist = default;
	[SerializeField, Tooltip("�ђʑϐ�"),]
	private int pierceResist = default;
	[SerializeField, Tooltip("�X�R�A")]
	private int score = default;

    // �����ϐ��̎���
    private Dictionary<Type, int> resists = new Dictionary<Type, int>();
    // �������Ă���Ƃ��̌��������Breaker�N���X
    // private Breaker Breaker = null;

    private void Start()
    {
		resists.Add(Type.slash, slashResist);
		resists.Add(Type.crash, crashResist);
		resists.Add(Type.pierce, pierceResist);
    }

    private void Break(Type type)
    {
        Debug.Log("Break");
        // addScore(_socre) 
        // Breaker.enable = ture;
        switch (type)
		{
			case Type.slash:
                // Slash�N���X���Ăяo��
                break;
            case Type.crash:
                // Crash�N���X���Ăяo��
                break;
            case Type.pierce:
                // Pierce�N���X���Ăяo��
                break;
            default:
				break;
        }
    }
}
