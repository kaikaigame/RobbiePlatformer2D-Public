using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    Animator anim;
    PlayerMovement movement;
    Rigidbody2D rb;

    int groundID;//编号
    int hangingID;
    int crouchID;
    int speedID;
    int fallID;

    void Start()
    {
        anim = GetComponent<Animator>();
        movement = GetComponentInParent<PlayerMovement>();//获得父级
        rb = GetComponentInParent<Rigidbody2D>();

        groundID = Animator.StringToHash("isOnGround");//字符型转换为数值型
        hangingID = Animator.StringToHash("isHanging");
        crouchID = Animator.StringToHash("isCrouching");
        speedID = Animator.StringToHash("speed");
        fallID = Animator.StringToHash("verticalVelocity");
    }


    void Update()
    {
        //anim.SetFloat("speed", Mathf.Abs(movement.xVelocity));//取得移动速度绝对值
        //anim.SetBool("isOnGround", movement.isOnGround);

        anim.SetBool(groundID, movement.isOnGround);
        anim.SetBool(hangingID, movement.isHanging);
        anim.SetBool(crouchID, movement.isCrouch);
        anim.SetFloat(speedID, Mathf.Abs(movement.xVelocity));//取得移动速度绝对值
        anim.SetFloat(fallID, rb.velocity.y);//获得y值参数
    }

    public void StepAudio()//走路声音
    {
        AudioManager.PlayFootstepAudio();
    }

    public void CrouchStepAudio()//下蹲走路声音
    {
        AudioManager.PlayCrouchFootstepAudio();
    }
}
