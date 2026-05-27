using UnityEngine;

public class Arrow : MonoBehaviour
{
    private GameObject _target;
    private float _speed = 600f;
    private RectTransform _rt;

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
            _rt.position += (Vector3)(Vector2.right * _speed * Time.deltaTime);
            CheckHitAnyEnemy();
            if (_rt.position.x > Screen.width + 100f)
                Destroy(gameObject);
            return;
        }

        Vector2 targetPos = (Vector2)_target.GetComponent<RectTransform>().position
                            + Vector2.up * _target.GetComponent<RectTransform>().rect.height / 2f;

        Vector2 dir = (targetPos - (Vector2)_rt.position).normalized;
        _rt.position += (Vector3)(dir * _speed * Time.deltaTime);

        CheckHitAnyEnemy(); // проверяем всех врагов при каждом движении

        float dist = Vector2.Distance(_rt.position, targetPos);
        if (dist < 20f)
        {
            _target.GetComponent<Enemy>()?.TakeDamage(1);
            Destroy(gameObject);
        }
    }
    
    void CheckHitAnyEnemy()
    {
        var enemies = FindObjectsByType<Enemy>(FindObjectsInactive.Exclude);
        foreach (var enemy in enemies)
        {
            float dist = Vector2.Distance(_rt.position, enemy.GetComponent<RectTransform>().position);
            if (dist < 30f)
            {
                enemy.TakeDamage(1);
                Destroy(gameObject);
                return;
            }
        }
    }
}