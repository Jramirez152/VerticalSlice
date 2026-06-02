using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    public PlayerController playerController;
    private Animator _animator;
    private PlayerState _lastState;

    void Start()
    {
        _animator = GetComponent<Animator>();
        if (_animator != null)
        {
            _animator.Rebind();
            _animator.Play("Idle");
        }
        Debug.Log($"Animator: {_animator}, PlayerController: {playerController}");
    }

    void Update()
    {
        if (_animator == null || playerController == null) return;

        PlayerState state = playerController.currentState;

        Debug.Log($"Current state: {state}");

        if (state != _lastState)
        {
            
                Debug.Log($"State changed from {_lastState} to {state}");
               
                switch (state)
                {
                    case PlayerState.Idle: _animator.Play("Idle"); break;
                    case PlayerState.Move: _animator.Play("Running"); break;
                    case PlayerState.Attack: _animator.Play("Punching"); break;
                    case PlayerState.Dash: _animator.Play("Stand to Roll"); break;
                    case PlayerState.Push: _animator.Play("Idle"); break;
                }
                _lastState = state;
            
        }
    }
}