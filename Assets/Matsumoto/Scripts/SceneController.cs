using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    // TargetScene�ɑJ�ڂ���
    public void ChangeToTargetScene(string targetSceneName)
    {
        SceneManager.LoadScene(targetSceneName);
    }
}
