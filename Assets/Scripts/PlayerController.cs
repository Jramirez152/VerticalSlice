using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;

    [Header("Dash")]
    public float dashSpeed = 14f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 1f;

    [Header("Attack")]
    public float attackDuration = 0.4f;

    [Header("Push")]
    public float pushDuration = 0.2f;
    public float pushCooldown = 2f;
    public float pushForce = 12f;
    public float pushConeAngle = 60f;
    public float pushRadius = 3f;

    public PlayerState currentState = PlayerState.Idle;

    private Rigidbody _rb;
    private Animator _animator;
    private float _stateTimer;
    private float _dashCooldownTimer;
    private float _pushCooldownTimer;
    private Vector3 _dashDirection;
    private bool _attackHitFired;
    private bool _pushFired;
    private bool _isKnockedBack;
    private Camera _cam;

    public float DashCooldownRemaining => _dashCooldownTimer;
    public float PushCooldownRemaining => _pushCooldownTimer;

    void Start()
    {
        _animator = GetComponentInChildren<Animator>();
    }

    void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _cam = Camera.main;
    }

    void Update()
    {
        if (_isKnockedBack) return;

        if (_dashCooldownTimer > 0f)
            _dashCooldownTimer -= Time.deltaTime;

        if (_pushCooldownTimer > 0f)
            _pushCooldownTimer -= Time.deltaTime;

        switch (currentState)
        {
            case PlayerState.Idle: UpdateIdle(); break;
            case PlayerState.Move: UpdateMove(); break;
            case PlayerState.Attack: UpdateAttack(); break;
            case PlayerState.Dash: UpdateDash(); break;
            case PlayerState.Push: UpdatePush(); break;
        }
    }

    // State Updates

    void UpdateIdle()
    {
        FaceMouseCursor();
        if (!_isKnockedBack)
            _rb.velocity = Vector3.zero;

        if (GetMoveInput().magnitude > 0.1f) { EnterMove(); return; }
        if (Input.GetMouseButtonDown(0)) { EnterAttack(); return; }
        if (Input.GetKeyDown(KeyCode.Space) && _dashCooldownTimer <= 0f) { EnterDash(); return; }
        if (Input.GetMouseButtonDown(1) && _pushCooldownTimer <= 0f) { EnterPush(); return; }
    }

    void UpdateMove()
    {
        Vector3 input = GetMoveInput();
        _rb.velocity = new Vector3(input.x * moveSpeed, _rb.velocity.y, input.z * moveSpeed);
        FaceMovementDirection(input);

        if (input.magnitude < 0.1f) { EnterIdle(); return; }
        if (Input.GetMouseButtonDown(0)) { EnterAttack(); return; }
        if (Input.GetKeyDown(KeyCode.Space) && _dashCooldownTimer <= 0f) { EnterDash(); return; }
        if (Input.GetMouseButtonDown(1) && _pushCooldownTimer <= 0f) { EnterPush(); return; }
    }

    void UpdateAttack()
    {
        FaceMouseCursor();
        Vector3 input = GetMoveInput();
        _rb.velocity = new Vector3(input.x * moveSpeed, _rb.velocity.y, input.z * moveSpeed);

        _stateTimer -= Time.deltaTime;

        if (!_attackHitFired && _stateTimer <= attackDuration * 0.5f)
        {
            DoAttackHit();
            _attackHitFired = true;
        }

        if (_stateTimer <= 0f)
        {
            _attackHitFired = false;
            EnterIdle();
        }
    }

    void UpdateDash()
    {
        FaceMovementDirection(_dashDirection);
        _rb.velocity = new Vector3(
            _dashDirection.x * dashSpeed,
            _rb.velocity.y,
            _dashDirection.z * dashSpeed
        );

        _stateTimer -= Time.deltaTime;
        if (_stateTimer <= 0f) EnterIdle();
    }

    void UpdatePush()
    {
        FaceMouseCursor();
        _rb.velocity = Vector3.zero;

        _stateTimer -= Time.deltaTime;

        if (!_pushFired && _stateTimer <= pushDuration * 0.5f)
        {
            DoPush();
            _pushFired = true;
        }

        if (_stateTimer <= 0f)
        {
            _pushFired = false;
            EnterIdle();
        }
    }

    // State Transitions 

    void EnterIdle()
    {
        currentState = PlayerState.Idle;
        PlayAnimation("Idle");
        _animator.speed = 1f;
        if (!_isKnockedBack)
            _rb.velocity = Vector3.zero;
    }

    void EnterMove()
    {
        currentState = PlayerState.Move;
        PlayAnimation("Running");
    }

    void EnterAttack()
    {
        currentState = PlayerState.Attack;
        PlayAnimation("Punching");
        _stateTimer = attackDuration;
        _rb.velocity = Vector3.zero;
    }

    void EnterDash()
    {
        currentState = PlayerState.Dash;
        PlayAnimation("Stand to Roll");
        _animator.speed = 2f;
        _stateTimer = dashDuration;
        _dashCooldownTimer = dashCooldown;

        Vector3 input = GetMoveInput();
        _dashDirection = input.magnitude > 0.1f ? input : transform.forward;
    }

    void EnterPush()
    {
        currentState = PlayerState.Push;
        PlayAnimation("Idle");
        _stateTimer = pushDuration;
        _pushCooldownTimer = pushCooldown;
        _pushFired = false;
        _rb.velocity = Vector3.zero;
    }

    // Animation

    void PlayAnimation(string animName)
    {
        if (_animator == null)
            _animator = GetComponentInChildren<Animator>();
        if (_animator != null)
            _animator.CrossFade(animName, 0.1f);
        Debug.Log($"Playing animation: {animName}");
    }

    // Combat

    void DoAttackHit()
    {
        Collider[] hits = Physics.OverlapSphere(
            transform.position + transform.forward * 1f,
            1f
        );

        foreach (var col in hits)
        {
            if (col.TryGetComponent<EnemyController>(out var enemy))
                enemy.TakeDamage(25f);
        }
    }

    void DoPush()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, pushRadius);

        foreach (var col in hits)
        {
            if (col.TryGetComponent<EnemyController>(out var enemy))
            {
                Vector3 dirToEnemy = (col.transform.position - transform.position).normalized;
                float angle = Vector3.Angle(transform.forward, dirToEnemy);

                if (angle <= pushConeAngle * 0.5f)
                    enemy.Knockback(transform.forward, pushForce);
            }
        }
    }

    // Knockback

    public void ApplyKnockback(Vector3 direction, float force)
    {
        StartCoroutine(KnockbackCoroutine(direction, force));
    }

    IEnumerator KnockbackCoroutine(Vector3 direction, float force)
    {
        _isKnockedBack = true;
        currentState = PlayerState.Idle;
        _rb.velocity = direction * force;

        yield return new WaitForSeconds(0.3f);

        _rb.velocity = Vector3.zero;
        _isKnockedBack = false;
    }

    // Rotation

    void FaceMouseCursor()
    {
        Ray ray = _cam.ScreenPointToRay(Input.mousePosition);
        Plane ground = new Plane(Vector3.up, transform.position);
        if (ground.Raycast(ray, out float dist))
        {
            Vector3 target = ray.GetPoint(dist);
            Vector3 dir = (target - transform.position);
            dir.y = 0f;
            if (dir.magnitude > 0.1f)
                transform.rotation = Quaternion.Slerp(
                    transform.rotation,
                    Quaternion.LookRotation(dir),
                    15f * Time.deltaTime
                );
        }
    }

    void FaceMovementDirection(Vector3 moveInput)
    {
        if (moveInput.magnitude > 0.1f)
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                Quaternion.LookRotation(moveInput),
                15f * Time.deltaTime
            );
    }

    // Helpers

    Vector3 GetMoveInput()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        return new Vector3(h, 0f, v).normalized;
    }

    // Gizmos

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + transform.forward * 1f, 1f);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, pushRadius);
    }
}