using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D rb;
    private BoxCollider2D coll;

    [Header("Movement Parameters")]
    public float speed = 8f;//速度
    public float crouchSpeedDivisor = 3f;//下蹲速度除数

    [Header("Jumping Parameters")]
    public float jumpForce = 6.3f;//跳跃力
    public float jumpHoldForce = 1.9f;//长按跳跃额外加成的力
    public float jumpHoldDuration = 0.1f;//长按跳跃持续时间
    public float crouchJumpBoost = 2.5f;//下蹲额外跳跃加成
    public float hangingJumpForce = 15f;//挂着的额外跳跃

    float jumpTime;//跳跃时间

    [Header("Condition")]
    public bool isCrouch;//正在下蹲
    public bool isOnGround;//是否踩在地上
    public bool isJump;//正在跳跃
    public bool isHeadBlocked;//头顶被挡住
    public bool isHanging;//正在悬挂

    [Header("Environment Detection")]
    public float footOffset = 0.4f;//左右脚的距离，collider宽度一半
    public float headClearance = 0.5f;//头顶检测距离
    public float groundDistance = 0.2f;//与地面之间距离
    float playerHeight;//角色高度，头顶位置
    public float eyeHeight = 1.5f;//眼睛高度
    public float grabDistance = 0.4f;//判断与墙的距离
    public float reachOffset = 0.7f;//判断头顶向上无墙壁，向下有墙壁

    public LayerMask groundLayer;
    public float xVelocity;//判断X轴力的方向

    //按键设置
    bool jumpPressed;
    bool jumpHeld;
    bool crouchPressed;
    bool crouchHeld;

    //碰撞体尺寸
    Vector2 colliderStandSize;//站立尺寸
    Vector2 colliderStandOffset;//站立坐标
    Vector2 colliderCrouchSize;//下蹲尺寸
    Vector2 colliderCrouchOffset;//下蹲坐标

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        coll = GetComponent<BoxCollider2D>();

        playerHeight = coll.size.y;//获得游戏角色高度

        colliderStandSize = coll.size;
        colliderStandOffset = coll.offset;
        colliderCrouchSize = new Vector2(coll.size.x, coll.size.y / 2f);//全新的大小，y除以2
        colliderCrouchOffset = new Vector2(coll.offset.x, coll.offset.y / 2f);//全新的坐标，y除以2
    }

    void Update()
    {
        if (GameManager.GameOver())//游戏已经结束
        {
            xVelocity = 0f;
            rb.bodyType = RigidbodyType2D.Static;
            return;
        }

        jumpPressed = Input.GetButtonDown("Jump");//单次跳跃
        jumpHeld = Input.GetButton("Jump");//长按跳跃，连续判断
        crouchPressed = Input.GetButtonDown("Crouch");//单次蹲下
        crouchHeld = Input.GetButton("Crouch");//长按蹲下，连续判断
    }

    private void FixedUpdate()
    {
        if (GameManager.GameOver())//游戏已经结束
        {
            xVelocity = 0f;
            rb.bodyType = RigidbodyType2D.Static;
            return;
        }

        PhysicsCheck();
        GroundMovement();
        MidAirMovement();

    }

    void PhysicsCheck()//物理检测
    {
        //左右脚射线
        //角色+偏移=左脚脚步偏移，方向向下，地面距离，地面图层
        RaycastHit2D leftCheck = Raycast(new Vector2(-footOffset, 0f), Vector2.down, groundDistance, groundLayer);

        //角色+偏移=右脚脚步偏移，方向向下，地面距离，地面图层
        RaycastHit2D rightCheck = Raycast(new Vector2(footOffset, 0f), Vector2.down, groundDistance, groundLayer);

        //if (coll.IsTouchingLayers(groundLayer))//如果触碰ground层
        if (leftCheck || rightCheck)
            isOnGround = true;
        else isOnGround = false;

        //头顶上射线，判断是否有墙体
        //y最高值，方向朝上，头顶检测距离，地面图层
        RaycastHit2D headCheck = Raycast(new Vector2(0f, coll.size.y), Vector2.up, headClearance, groundLayer);

        if (headCheck)//如果头上有物体
            isHeadBlocked = true;
        else isHeadBlocked = false;

        float direction = transform.localScale.x;//头顶前方射线判断，定义方向
        Vector2 grabDir = new Vector2(direction, 0f);//射线方向

        //头顶左右射线，判断头顶是否有墙体
        //（角色左上角/右上角 x 左右翻转，角色高度），射线方向，与墙的距离，平台图层
        RaycastHit2D blockedCheck = Raycast(new Vector2(footOffset * direction, playerHeight), grabDir, grabDistance, groundLayer);

        //眼睛射线，墙体检测
        //（角色左上角/右上角 x 左右翻转，眼睛高度），射线方向，与墙的距离，平台图层
        RaycastHit2D wallCheck = Raycast(new Vector2(footOffset * direction, eyeHeight), grabDir, grabDistance, groundLayer);

        //壁挂检测射线
        //（头顶向下有墙壁 x 左右翻转，角色高度），方向朝下，与墙的距离，平台图层
        RaycastHit2D ledgeCheck = Raycast(new Vector2(reachOffset * direction, playerHeight), Vector2.down, grabDistance, groundLayer);

        //不在地上，下降，壁挂检测有，墙体检测有，头顶无墙体
        if (!isOnGround && rb.velocity.y < 0f && ledgeCheck && wallCheck && !blockedCheck)
        {
            Vector3 pos = transform.position;//获得角色此时位置

            //墙体检测，目前x位置+（获得射线距离最近的碰撞体之间的长度=角色离墙面的距离） x 朝向
            pos.x += (wallCheck.distance - 0.05f) * direction;//留出一些手臂的位置

            //墙体检测，目前y位置 - 壁挂检测射线的距离
            pos.y -= ledgeCheck.distance;

            transform.position = pos;//重新给出位置

            rb.bodyType = RigidbodyType2D.Static;//角色刚体静止，固定
            isHanging = true;//悬挂
        }

    }

    void GroundMovement()//地上移动
    {
        if (isHanging)//悬挂后停止执行下面的函数
            return;

        if (crouchHeld && !isCrouch && isOnGround)//判断下蹲
            Crouch();
        //下蹲时没有按下蹲键，头顶没有东西
        else if (!crouchHeld && isCrouch && !isHeadBlocked)
            StandUp();
        else if (!isOnGround && isCrouch)//不在地面上下蹲
            StandUp();

        xVelocity = Input.GetAxis("Horizontal"); //介于-1f 1f， 不按键盘归0，判断键盘左右移动

        if (isCrouch)//移动之前，下蹲时速度减少
            xVelocity /= crouchSpeedDivisor;

        rb.velocity = new Vector2(xVelocity * speed, rb.velocity.y);//x轴移动

        FlipDirection();
    }

    void MidAirMovement()//跳跃
    {
        if (isHanging)
        {
            if (jumpPressed)//悬挂时单次跳跃
            {
                rb.bodyType = RigidbodyType2D.Dynamic;//把角色刚体调整回来
                rb.velocity = new Vector2(rb.velocity.x, hangingJumpForce);//施加向上的跳跃力
                isHanging = false;//解除悬挂
            }

            if (crouchPressed)////悬挂时单次下蹲
            {
                rb.bodyType = RigidbodyType2D.Dynamic;
                isHanging = false;
            }
        }

        //如果不在地面，不在跳跃中，头顶没被挡 按跳跃
        if (jumpPressed && isOnGround && !isJump && !isHeadBlocked)
        {
            if (isCrouch)
            {
                StandUp();
                rb.AddForce(new Vector2(0f, crouchJumpBoost), ForceMode2D.Impulse);
            }

            isOnGround = false;
            isJump = true;

            jumpTime = Time.time + jumpHoldDuration;//计算跳跃时间

            rb.AddForce(new Vector2(0f, jumpForce), ForceMode2D.Impulse);//添加二维向量的力

            AudioManager.PlayJumpAudio();
        }

        else if (isJump)
        {
            if (jumpHeld)//持续跳跃
                rb.AddForce(new Vector2(0f, jumpHoldForce), ForceMode2D.Impulse);
            if (jumpTime < Time.time)//实时时间会渐渐增长
                isJump = false;
        }
    }

    void FlipDirection()//翻转朝向
    {
        if (xVelocity < 0)//速度小于0向左朝向，if语句只有一行代码可不写大括号
            transform.localScale = new Vector3(-1, 1, 1);

        if (xVelocity > 0)//速度大于0向右朝向
            transform.localScale = new Vector3(1, 1, 1);
    }

    void Crouch()//下蹲
    {
        isCrouch = true;
        coll.size = colliderCrouchSize;
        coll.offset = colliderCrouchOffset;
    }

    void StandUp()//下蹲起立
    {
        isCrouch = false;
        coll.size = colliderStandSize;
        coll.offset = colliderStandOffset;
    }

    //method overloading + DrawRay 方法重载+画射线
    RaycastHit2D Raycast(Vector2 offset, Vector2 rayDiraction, float length, LayerMask layer)//位移，方向，距离，图层
    {
        Vector2 pos = transform.position;//获得游戏角色的位置

        RaycastHit2D hit = Physics2D.Raycast(pos + offset, rayDiraction, length, layer);

        Color color = hit ? Color.red : Color.green;//碰撞红色，否则绿色

        Debug.DrawRay(pos + offset, rayDiraction * length, color);//画射线

        return hit;//返回值
    }

}
