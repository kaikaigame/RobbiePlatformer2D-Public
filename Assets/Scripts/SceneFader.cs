using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneFader : MonoBehaviour
{
    Animator anim;
    int faderID;//生成动画序列号

    private void Start()
    {
        anim = GetComponent<Animator>();

        faderID = Animator.StringToHash("Fade");

        GameManager.RegisterSceneFader(this);
    }

    public void FadeOut()//启动Fade动画
    {
        anim.SetTrigger(faderID);
    }
}
