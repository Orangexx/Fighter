using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//非输入、非攻击的一些角色判定，例如是否在地上
public class CharacterTriggers : MonoBehaviour {

    [SerializeField] private LayerMask mGroundLayer;
    [SerializeField] private SpriteRenderer mSpriteRenderer;

    public Bool IsGrounded;

    private void Awake()
    {
        IsGrounded = new Bool(true);
    }

    void Update()
    {
        SetIsGrounded();
    }

    void SetIsGrounded()
    {
        float mDistToGround = mSpriteRenderer.sprite.bounds.size.y / 2 - 0.1f;
        IsGrounded.BOOL = Physics2D.Raycast(transform.position, Vector2.down, mDistToGround, mGroundLayer);
    }

}
