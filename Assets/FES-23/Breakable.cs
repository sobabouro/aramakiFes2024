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



    /// <summary>
    /// �U�����ꂽ���ɌĂяo�����\�b�h�B
    /// </summary>
    /// <param name="receivedATK">�󂯂�U����</param>
    /// <param name="attackType">�󂯂�U���̑���</param>
    /// <returns></returns>
    private bool ReciveAttack(int receivedATK, Type attackType)
    {
        int damage = CalcDamage(receivedATK, attackType);
        Debug.Log($"damage: {damage}");
        durability -= damage;
        Debug.Log($"durability: {durability}");
        if (durability < 0) {
            Break(attackType);
            return true;
        }
        return false;
    }

    /// <summary>
    /// �ϋv�l���O�ɂȂ����Ƃ��̃��\�b�h
    /// </summary>
    /// <param name="type">�ǂ̑����ɉ󂳂�邩</param>
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


    /// <summary>
    /// �^����ꂽ�U���͂Ƒ����A���g�̑ϐ��A�ŏI�I�ȃ_���[�W�̒l���v�Z����B
    /// </summary>
    /// <param name="receivedATK">�󂯂�U����</param>
    /// <param name="attackType">�󂯂�U���̑���</param>
    /// <returns></returns>
    private int CalcDamage(int receivedATK, Type attackType)
	{
		int damage = receivedATK - resists[attackType];
		if (damage < 0) damage = 0;
        return damage;
	}

}
