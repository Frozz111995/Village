using UnityEngine;

public class Arrow : MonoBehaviour
{
    [SerializeField] private float _rotationOffset = 0f;
    
    private GameObject _target;
    private float _speed = 600f;
    private RectTransform _rt;
    private Vector2 _lastDirection = Vector2.right;

    public void Init(GameObject target, float speed)
    {
        _target = target;
        _speed = speed;
        _rt = GetComponent<RectTransform>();
    }

    void Update()
    {
        if (_target == null)
        {
            _rt.position += (Vector3)(_lastDirection * _speed * Time.deltaTime);
            SetRotation(_lastDirection);
            CheckHitAnyEnemy();
            if (_rt.position.x > Screen.width + 100f)
                Destroy(gameObject);
            return;
        }

        Vector2 targetPos = (Vector2)_target.GetComponent<RectTransform>().position
                            + Vector2.up * _target.GetComponent<RectTransform>().rect.height / 2f;

        _lastDirection = (targetPos - (Vector2)_rt.position).normalized;
        _rt.position += (Vector3)(_lastDirection * _speed * Time.deltaTime);
        SetRotation(_lastDirection);

        CheckHitAnyEnemy();

        float dist = Vector2.Distance(_rt.position, targetPos);
        if (dist < 20f)
        {
            _target.GetComponent<Enemy>()?.TakeDamage(1);
            Destroy(gameObject);
        }
    }

    void SetRotation(Vector2 direction)
    {
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        _rt.rotation = Quaternion.Euler(0f, 0f, angle + _rotationOffset);
    }

    void CheckHitAnyEnemy()
    {
        var enemies = FindObjectsByType<Enemy>(FindObjectsInactive.Exclude);
        foreach (var enemy in enemies)
        {
            RectTransform enemyRt = enemy.GetComponent<RectTransform>();
            Vector2 enemyCenter = (Vector2)enemyRt.position + Vector2.up * enemyRt.rect.height / 2f;

            float dist = Vector2.Distance(_rt.position, enemyCenter);
            if (dist < 60f)
            {
                enemy.TakeDamage(1);
                Destroy(gameObject);
                return;
            }
        }
    }
}