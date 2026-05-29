using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private NightBattleManager _manager;
    private RectTransform _rt;
    private Animator _animator;
    private int _hp = 4;
    private bool _attacking = false;
    private GameObject _targetPike = null;
    private float _attackTimer = 0f;
    private float _attackInterval = 1f;
    private bool _initialized = false;
    private bool _pikeHitReceived = false;

    public void Init(NightBattleManager manager)
    {
        _manager = manager;
        _rt = GetComponent<RectTransform>();
        _animator = GetComponent<Animator>();
        _animator?.SetBool("Walk", true);
        _initialized = true;
    }

    void Update()
    {
        if (!_initialized) return;
        
        if (GameManager.Instance.IsGameOver)
        {
            return;
        }

        if (_attacking)
        {
            AttackTarget();
            return;
        }

        _rt.anchoredPosition += Vector2.left * 80f * Time.deltaTime;

        CheckPikeCollision();

        if (_rt.position.x <= _manager.WallBoundary.GetWallX(_rt.position.y))
        {
            SetAttacking(true);
        }
    }

    void CheckPikeCollision()
    {
        var pikes = _manager.GetPikes();

        foreach (var pike in pikes)
        {
            if (pike == null || !pike.activeSelf) continue;

            float dist = Vector2.Distance(_rt.position, pike.GetComponent<RectTransform>().position);

            if (dist < 60f)
            {
                if (!_pikeHitReceived)
                {
                    _pikeHitReceived = true;
                    pike.GetComponent<Pike>()?.DealDamageTo(this);
                }

                _targetPike = pike;
                SetAttacking(true);
                return;
            }
        }
    }

    void SetAttacking(bool value)
    {
        _attacking = value;
        _animator?.SetBool("Walk", !value);
    }

    void AttackTarget()
    {
        _attackTimer += Time.deltaTime;
        if (_attackTimer < _attackInterval) return;
        _attackTimer = 0f;

        _animator?.SetTrigger("Attack");
        StartCoroutine(DealDamageWithDelay(0.5f));
    }
    
    IEnumerator DealDamageWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (_targetPike != null)
        {
            var pike = _targetPike.GetComponent<Pike>();
            if (pike != null)
            {
                pike.TakeDamage();
                if (!pike.IsAlive)
                {
                    _targetPike = null;
                    SetAttacking(false);
                }
            }
        }
        else
        {
            _manager.OnEnemyReachedWall();
        }
    }

    public void TakeDamage(int dmg)
    {
        _hp -= dmg;
        if (_hp <= 0)
            _manager.OnEnemyDefeated(gameObject);
        Debug.Log(this + " got damage, hp left  " + _hp);
    }
}