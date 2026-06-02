using UnityEngine;
using UnityEngine.AI;
using System.Collections;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyController : MonoBehaviour
{
    [Header("Stats")]
    public float maxHealth = 50f;
    public float currentHealth;

    [Header("Combat")]
    public float attackDamage = 10f;
    public float attackRange = 1.2f;
    public float attackCooldown = 1.5f;
    public float attackWindup = 0.5f;

    [Header("Knockback")]
    public float knockbackDuration = 0.3f;

    [Header("Push")]
    public bool canPush = false;
    public float pushCooldown = 3f;
    public float pushForce = 10f;
    public float pushRange = 2f;
    public float pushWindup = 0.6f;

    private NavMeshAgent _agent;
    private Transform _player;
    private PlayerHealth _playerHealth;
    private PlayerController _playerController;
    private float _attackTimer;
    private float _pushTimer;
    private bool _isWindingUp;
    private bool _isKnockedBack;
    private Animator _animator;
    private string _currentAnim;

    private Renderer _renderer;
    private Color _defaultColor;

    void Start()
    {
        currentHealth = maxHealth;
        _agent = GetComponent<NavMeshAgent>();
        _renderer = GetComponentInChildren<Renderer>();
        _animator = GetComponentInChildren<Animator>();

        if (_renderer != null)
            _defaultColor = _renderer.material.color;

        GameObject playerObj = GameObject.FindWithTag("Player");
        if (playerObj != null)
        {
            _player = playerObj.transform;
            _playerHealth = playerObj.GetComponent<PlayerHealth>();
            _playerController = playerObj.GetComponent<PlayerController>();
        }

        PlayAnimation("Idle");
    }

    void Update()
    {
        if (_player == null || _isWindingUp || _isKnockedBack) return;

        _agent.SetDestination(_player.position);

        // Drive walk/idle animation based on movement
        if (_agent.velocity.magnitude > 0.1f)
            PlayAnimation("Running");
        else
            PlayAnimation("Idle");
        Debug.Log($"Agent velocity: {_agent.velocity.magnitude}");
        float dist = Vector3.Distance(transform.position, _player.position);

        // Regular attack
        _attackTimer -= Time.deltaTime;
        if (dist <= attackRange && _attackTimer <= 0f)
        {
            _attackTimer = attackCooldown;
            StartCoroutine(AttackWindup());
        }

        // Push attack for big enemy
        if (canPush)
        {
            _pushTimer -= Time.deltaTime;
            if (dist <= pushRange && _pushTimer <= 0f)
            {
                _pushTimer = pushCooldown;
                StartCoroutine(PushWindup());
            }
        }
    }

    IEnumerator AttackWindup()
    {
        _isWindingUp = true;
        _agent.isStopped = true;

        yield return new WaitForSeconds(0.1f);
        PlayAnimation("Punching");

        if (_renderer != null)
            _renderer.material.color = Color.red;

        yield return new WaitForSeconds(attackWindup);

        _playerHealth?.TakeDamage(attackDamage);

        if (_renderer != null)
            _renderer.material.color = _defaultColor;

        _agent.isStopped = false;
        _isWindingUp = false;
    }

    IEnumerator PushWindup()
    {
        _isWindingUp = true;
        _agent.isStopped = true;

        if (_renderer != null)
            _renderer.material.color = Color.magenta;

        yield return new WaitForSeconds(pushWindup);

        float dist = Vector3.Distance(transform.position, _player.position);
        if (dist <= pushRange)
        {
            Vector3 pushDir = (_player.position - transform.position).normalized;
            pushDir.y = 0f;
            _playerController?.ApplyKnockback(pushDir, pushForce);
        }

        if (_renderer != null)
            _renderer.material.color = _defaultColor;

        _agent.isStopped = false;
        _isWindingUp = false;
    }

    public void TakeDamage(float amount)
    {
        if (_renderer != null)
            StartCoroutine(DamageFlash());

        currentHealth -= amount;
        Debug.Log($"{gameObject.name} took {amount} damage. HP remaining: {currentHealth}");

        if (currentHealth <= 0f)
        {
            Debug.Log($"{gameObject.name} died.");
            Destroy(gameObject);
        }
    }

    public void Knockback(Vector3 direction, float force)
    {
        StartCoroutine(KnockbackCoroutine(direction, force));
    }

    IEnumerator KnockbackCoroutine(Vector3 direction, float force)
    {
        _isKnockedBack = true;
        _agent.enabled = false;

        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.AddForce(direction * force, ForceMode.Impulse);
        }

        yield return new WaitForSeconds(knockbackDuration);

        if (rb != null)
            rb.isKinematic = true;

        _agent.enabled = true;
        _isKnockedBack = false;
    }

    IEnumerator DamageFlash()
    {
        if (_renderer != null)
        {
            _renderer.material.color = Color.white;
            yield return new WaitForSeconds(0.1f);
            _renderer.material.color = _defaultColor;
        }
    }

    void PlayAnimation(string animName)
    {
        if (_animator == null)
            _animator = GetComponentInChildren<Animator>();

        Debug.Log($"Current: {_currentAnim}, Requested: {animName}, Same: {_currentAnim == animName}");

        if (_animator != null && _animator.isActiveAndEnabled && _currentAnim != animName)
        {
            _animator.CrossFade(animName, 0.1f, 0);
            _currentAnim = animName;
        }
    }
}