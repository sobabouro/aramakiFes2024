using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class MainSceneController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // ��X�ʂ̃R���|�[�l���g�ɋ@�\���ړ�����\������
    // ResultScene(Result_Test)�ɑJ�ڂ���
    public void ChangeToResultScene()
    {
        SceneManager.LoadScene("Result_Test");
    }
}
