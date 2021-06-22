using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    Animator anim;
    int openID;//动画中参数id

    void Start()
    {
        anim = GetComponent<Animator>();
        openID = Animator.StringToHash("Open");

        //registerDoor
        GameManager.RegisterDoor(this);
    }

    public void Open()//播放Door动画
    {
        anim.SetTrigger(openID);

        //play audio
        AudioManager.PlayDoorOpenAudio();
    }

}
