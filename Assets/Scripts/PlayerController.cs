
using UnityEngine;
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

    // Current state
    public PlayerState currentState = PlayerState.Idle;

    private Rigidbody _rb;
    private float _stateTimer;
    private float _dashCooldownTimer;
    private Vector3 _dashDirection;
    private bool _attackHitFired;
    private Camera _cam;

    void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _cam = Camera.main;
    }

    void Update()
    {
        if (_dashCooldownTimer > 0f)
            _dashCooldownTimer -= Time.deltaTime;

        switch (currentState)
        {
            case PlayerState.Idle: UpdateIdle(); break;
            case PlayerState.Move: UpdateMove(); break;
            case PlayerState.Attack: UpdateAttack(); break;
            case PlayerState.Dash: UpdateDash(); break;
        }

        FaceMouseCursor();
    }

    //State Updates

    void UpdateIdle()
    {
        _rb.velocity = Vector3.zero;

        if (GetMoveInput().magnitude > 0.1f) { EnterMove(); return; }
        if (Input.GetMouseButtonDown(0)) { EnterAttack(); return; }
        if (Input.GetKeyDown(KeyCode.Space) && _dashCooldownTimer <= 0f) { EnterDash(); return; }
    }

    void UpdateMove()
    {
        Vector3 input = GetMoveInput();
        _rb.velocity = new Vector3(input.x * moveSpeed, _rb.velocity.y, input.z * moveSpeed);

        if (input.magnitude < 0.1f) { EnterIdle(); return; }
        if (Input.GetMouseButtonDown(0)) { EnterAttack(); return; }
        if (Input.GetKeyDown(KeyCode.Space) && _dashCooldownTimer <= 0f) { EnterDash(); return; }
    }

    void UpdateAttack()
    {
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
        _rb.velocity = new Vector3(
            _dashDirection.x * dashSpeed,
            _rb.velocity.y,
            _dashDirection.z * dashSpeed
        );

        _stateTimer -= Time.deltaTime;
        if (_stateTimer <= 0f) EnterIdle();
    }
    public float DashCooldownRemaining => _dashCooldownTimer; 
    //State Transitions

    void EnterIdle()
    {
        currentState = PlayerState.Idle;
        _rb.velocity = Vector3.zero;
    }

    void EnterMove()
    {
        currentState = PlayerState.Move;
    }

    void EnterAttack()
    {
        currentState = PlayerState.Attack;
        _stateTimer = attackDuration;
        _rb.velocity = Vector3.zero;
    }

    void EnterDash()
    {
        currentState = PlayerState.Dash;
        _stateTimer = dashDuration;
        _dashCooldownTimer = dashCooldown;

        Vector3 input = GetMoveInput();
        _dashDirection = input.magnitude > 0.1f ? input : transform.forward;
    }

    //Combat

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

    //Helpers

    Vector3 GetMoveInput()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        return new Vector3(h, 0f, v).normalized;
    }

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
                transform.rotation = Quaternion.LookRotation(dir);
        }
    }

    //Gizmos

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + transform.forward * 1f, 1f);
    }
}