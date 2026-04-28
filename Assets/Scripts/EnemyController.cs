
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
    public float attackRange = 1.5f;
    public float attackCooldown = 1.5f;
    public float attackWindup = 0.5f;

    private NavMeshAgent _agent;
    private Transform _player;
    private PlayerHealth _playerHealth;
    private float _attackTimer;
    private bool _isWindingUp;

    private Renderer _renderer;
    private Color _defaultColor;

    void Start()
    {
        currentHealth = maxHealth;
        _agent = GetComponent<NavMeshAgent>();
        _renderer = GetComponent<Renderer>();

        if (_renderer != null)
            _defaultColor = _renderer.material.color;

        GameObject playerObj = GameObject.FindWithTag("Player");
        if (playerObj != null)
        {
            _player = playerObj.transform;
            _playerHealth = playerObj.GetComponent<PlayerHealth>();
        }
    }

    void Update()
    {
        if (_player == null) return;

        if (!_isWindingUp)
        {
            _agent.SetDestination(_player.position);

            _attackTimer -= Time.deltaTime;
            float dist = Vector3.Distance(transform.position, _player.position);

            if (dist <= attackRange && _attackTimer <= 0f)
            {
                _attackTimer = attackCooldown;
                StartCoroutine(AttackWindup());
            }
        }
    }

    IEnumerator AttackWindup()
    {
        _isWindingUp = true;
        _agent.isStopped = true;

        // Flash red
        if (_renderer != null)
            _renderer.material.color = Color.red;

        yield return new WaitForSeconds(attackWindup);

        _playerHealth?.TakeDamage(attackDamage);

        // Return to default color
        if (_renderer != null)
            _renderer.material.color = _defaultColor;

        _agent.isStopped = false;
        _isWindingUp = false;
    }

    public void TakeDamage(float amount)
    {
        // Flash white briefly to show damage
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

    IEnumerator DamageFlash()
    {
        _renderer.material.color = Color.white;
        yield return new WaitForSeconds(0.1f);
        _renderer.material.color = _defaultColor;
    }
}