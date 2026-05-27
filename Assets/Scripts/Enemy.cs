using UnityEngine;

public class Enemy : MonoBehaviour
{
    private NightBattleManager _manager;
    private RectTransform _rt;
    private int _hp = 3;
    private bool _attacking = false;
    private GameObject _targetPike = null;
    private float _attackTimer = 0f;
    private float _attackInterval = 1f;
    private bool _initialized = false;
    
    public void Init(NightBattleManager manager)
    {
        _manager = manager;
        _rt = GetComponent<RectTransform>();
        _initialized = true;
    }

    void Update()
    {
        if (!_initialized) return;
        
        if (_attacking)
        {
            AttackTarget();
            return;
        }

        _rt.anchoredPosition += Vector2.left * 80f * Time.deltaTime;

        CheckPikeCollision();

        if (_rt.position.x <= _manager.WallBoundary.GetWallX(_rt.position.y))
        {
            _attacking = true;
            _manager.OnEnemyReachedWall();
        }
    }

    void CheckPikeCollision()
    {
        foreach (var pike in _manager.GetPikes())
        {
            if (pike == null) continue;
            float dist = Vector2.Distance(_rt.position, pike.GetComponent<RectTransform>().position);
            if (dist < 40f)
            {
                _targetPike = pike;
                _attacking = true;
                return;
            }
        }
    }

    void AttackTarget()
    {
        _attackTimer += Time.deltaTime;
        if (_attackTimer < _attackInterval) return;
        _attackTimer = 0f;

        if (_targetPike != null)
        {
            var pike = _targetPike.GetComponent<Pike>();
            if (pike != null)
            {
                pike.TakeDamage();
                if (!pike.IsAlive)
                {
                    _targetPike = null;
                    _attacking = false;
                }
            }
        }
    }

    public void TakeDamage(int dmg)
    {
        _hp -= dmg;
        if (_hp <= 0)
            _manager.OnEnemyDefeated(gameObject);
    }
}