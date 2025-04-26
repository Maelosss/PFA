using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    public int life;

    [Header("MOVE :")]
    public float _speed = 5;

    [Header("JUMP :")]
    public float _jumpForce = 5;
    public int _nbJump = 1;
    private int _currentJump;

    private Animator _animator;
    private Rigidbody2D _rigidbody2D;
    private SpriteRenderer _spriteRenderer;
    public LayerMask ground;
    public float coyoteTimer = 0.5f;
    public float _fallTime;
    public float jumpBufferedTime;
    private float jumpBufferedCounter;
    public bool isCoyote;

    [Header("CORNER CORRECTION")]
    public Transform cornerCheckTopLeft;
    public Transform cornerCheckTopRight;
    public float cornerCheckDistance = 0.1f;
    public float cornerPushAmount = 0.05f;

    [Header("GRAVITY CONTROL")]
    public float gravityMultiplier = 2f;
    public float lowJumpMultiplier = 2.5f;

    private void Awake()
    {
        if (!TryGetComponent(out _animator))
            Debug.LogError("Animator manquant sur " + gameObject.name);

        if (!TryGetComponent(out _rigidbody2D))
            Debug.LogError("Rigidbody2D manquant sur " + gameObject.name);

        if (!TryGetComponent(out _spriteRenderer))
            Debug.LogError("SpriteRenderer manquant sur " + gameObject.name);
    }

    void Start()
    {
        _currentJump = 0;
    }

    void Update()
    {
        // Déplacement
        float moveInput = Input.GetAxis("Horizontal");
        if (moveInput != 0)
        {
            _animator.SetBool("IsMoving", true);
            transform.position += moveInput * Time.deltaTime * _speed * Vector3.right;
        }
        else
        {
            _animator.SetBool("IsMoving", false);
        }

        _spriteRenderer.flipX = moveInput < 0;

        // Jump buffering
        if (Input.GetKeyDown(KeyCode.Space))
        {
            jumpBufferedCounter = jumpBufferedTime;
        }
        else
        {
            jumpBufferedCounter -= Time.deltaTime;
        }

        // Coyote time
        if (!IsGrounded())
        {
            _fallTime += Time.deltaTime;
        }
        else
        {
            _fallTime = 0;
        }

        // Saut
        if (CanJump())
        {
            Jump();
            _currentJump++;
            jumpBufferedCounter = 0;
        }

        if (IsGrounded() && _rigidbody2D.linearVelocity.y <= 0)
        {
            _animator.SetBool("IsJumping", false);
        }
        // Si le joueur tombe sous un certain niveau, on relance la scène
        if (transform.position.y < -10f)
        {
            Debug.Log("Le joueur est tombé dans le vide !");
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }


        HandleCornerCorrection();
        HandleGravity();
    }

    private bool CanJump()
    {
        if (isCoyote)
        {
            return (!IsGrounded() && _fallTime <= coyoteTimer && _currentJump < _nbJump)
                || (IsGrounded() && jumpBufferedCounter > 0 && _currentJump < _nbJump);
        }
        else
        {
            return (_currentJump < _nbJump && jumpBufferedCounter > 0);
        }
    }

    private void Jump()
    {
        _rigidbody2D.linearVelocity = new Vector2(_rigidbody2D.linearVelocity.x, _jumpForce);
        _animator.SetBool("IsJumping", true);
    }

    public void GetDamage(int damage)
    {
        life -= Mathf.Abs(damage);
        Debug.Log(life);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        _currentJump = 0;
        _fallTime = 0;
        _animator.SetBool("IsJumping", false);
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (!isCoyote)
            _currentJump++;
    }

    private bool IsGrounded()
    {
        return Physics2D.Raycast(transform.position, Vector2.down, 0.65f, ground);
    }

    private void HandleCornerCorrection()
    {
        RaycastHit2D hitRight = Physics2D.Raycast(cornerCheckTopRight.position, Vector2.right, cornerCheckDistance, ground);
        RaycastHit2D hitLeft = Physics2D.Raycast(cornerCheckTopLeft.position, Vector2.left, cornerCheckDistance, ground);

        bool isBlockedRight = hitRight.collider != null;
        bool isBlockedLeft = hitLeft.collider != null;

        if (isBlockedLeft && !isBlockedRight)
        {
            transform.position += new Vector3(cornerPushAmount, 0, 0);
        }
        else if (isBlockedRight && !isBlockedLeft)
        {
            transform.position += new Vector3(-cornerPushAmount, 0, 0);
        }
    }

    private void HandleGravity()
    {
        if (_rigidbody2D.linearVelocity.y < 0)
        {
            _rigidbody2D.gravityScale = gravityMultiplier;
        }
        else if (_rigidbody2D.linearVelocity.y > 0 && !Input.GetKey(KeyCode.Space))
        {
            _rigidbody2D.gravityScale = lowJumpMultiplier;
        }
        else
        {
            _rigidbody2D.gravityScale = 1f;
        }
    }
}
