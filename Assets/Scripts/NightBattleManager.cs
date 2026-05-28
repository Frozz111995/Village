using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NightBattleManager : MonoBehaviour
{
    public static NightBattleManager Instance;

    [Header("Солдаты и колья")]
    public List<GameObject> Soldiers;
    public List<GameObject> Pikes;

    [Header("Префабы")]
    public GameObject EnemyPrefab;
    public GameObject ArrowPrefab;

    [Header("Зоны")]
    public RectTransform SoldierArea;
    public RectTransform EnemyArea;
    public WallBoundary WallBoundary;

    [Header("Настройки")]
    public float ArrowSpeed = 300f;
    public float ShootInterval = 1.5f;
    public int EnemiesPerWave = 3;
    public float ShootRange = 400f;

    private float _baseShootInterval;
    private List<GameObject> _soldiers = new();
    private List<GameObject> _enemies = new();
    private List<GameObject> _arrows = new();

    private bool _battleActive = false;
    private int _enemiesDefeated = 0;
    private int _totalEnemies = 0;

    void Awake()
    {
        Instance = this;
        _baseShootInterval = ShootInterval;
    }

    public void StartBattle()
    {
        _battleActive = true;
        _enemiesDefeated = 0;
        _totalEnemies = EnemiesPerWave + GameManager.Instance.Day;

        // CombatBonus — солдаты стреляют вдвое быстрее
        ShootInterval = GameManager.Instance.ActiveBuff == VillagerBuff.CombatBonus
            ? _baseShootInterval * 0.5f
            : _baseShootInterval;

        UIManager.Instance.EndDayButton.interactable = false;

        ActivateSoldiers();
        ActivatePikes();
        StartCoroutine(SpawnEnemies());
        StartCoroutine(AutoShoot());
    }

    void ActivateSoldiers()
    {
        foreach (var s in Soldiers) s.SetActive(false);
        _soldiers.Clear();

        int count = Mathf.Min(VillagerManager.Instance.Villagers.Count, Soldiers.Count);

        List<GameObject> shuffled = new List<GameObject>(Soldiers);
        for (int i = shuffled.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (shuffled[i], shuffled[j]) = (shuffled[j], shuffled[i]);
        }

        for (int i = 0; i < count; i++)
        {
            shuffled[i].SetActive(true);
            _soldiers.Add(shuffled[i]);
        }
    }

    void ActivatePikes()
    {
        Pikes.RemoveAll(p => p == null);
        foreach (var p in Pikes)
        {
            p.SetActive(false);
            // сбрасываем цвет
            p.GetComponent<Image>().color = Color.white;
            p.GetComponent<Pike>().ResetHP();
        }

        int count = Mathf.Min(GameManager.Instance.StakeCount, Pikes.Count);

        List<GameObject> shuffled = new List<GameObject>(Pikes);
        for (int i = shuffled.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (shuffled[i], shuffled[j]) = (shuffled[j], shuffled[i]);
        }

        for (int i = 0; i < count; i++)
            shuffled[i].SetActive(true);
    }

    IEnumerator SpawnEnemies()
    {
        float height = EnemyArea.rect.height;
        float rightEdge = EnemyArea.rect.width / 2;

        for (int i = 0; i < _totalEnemies; i++)
        {
            var go = Instantiate(EnemyPrefab, EnemyArea);
            var rt = go.GetComponent<RectTransform>();
            rt.anchoredPosition = new Vector2(rightEdge, Random.Range(-height / 2, height / 2));

            var enemy = go.GetComponent<Enemy>();
            if (enemy != null) enemy.Init(this);

            _enemies.Add(go);

            yield return new WaitForSeconds(Random.Range(1.5f, 3f));
        }
    }

    IEnumerator AutoShoot()
    {
        while (_battleActive)
        {
            foreach (var soldier in _soldiers)
            {
                if (soldier == null || !soldier.activeSelf) continue;

                var target = GetNearestEnemy();
                if (target != null)
                {
                    float dist = Vector2.Distance(
                        soldier.GetComponent<RectTransform>().position,
                        target.GetComponent<RectTransform>().position
                    );

                    if (dist <= ShootRange)
                        ShootArrow(soldier, target);
                }

                yield return new WaitForSeconds(ShootInterval);
            }
        }
    }

    void ShootArrow(GameObject soldier, GameObject target)
    {
        var animator = soldier.GetComponent<Animator>();
        if (animator != null)
            animator.SetTrigger("Attack");

        StartCoroutine(ShootWithDelay(soldier, target, 0.4f));
    }

    IEnumerator ShootWithDelay(GameObject soldier, GameObject target, float delay)
    {
        yield return new WaitForSeconds(delay);

        if (soldier == null || target == null) yield break;

        var arrow = Instantiate(ArrowPrefab, SoldierArea.parent);
        arrow.GetComponent<RectTransform>().position = soldier.GetComponent<RectTransform>().position;
        arrow.GetComponent<Arrow>().Init(target, ArrowSpeed);
        _arrows.Add(arrow);
    }

    GameObject GetNearestEnemy()
    {
        GameObject nearest = null;
        float minX = float.MaxValue;

        foreach (var e in _enemies)
        {
            if (e == null) continue;
            float x = e.GetComponent<RectTransform>().anchoredPosition.x;
            if (x < minX) { minX = x; nearest = e; }
        }

        return nearest;
    }

    GameObject GetNearestSoldierTo(GameObject target)
    {
        if (_soldiers.Count == 0) return null;
        GameObject nearest = null;
        float minDist = float.MaxValue;

        foreach (var s in _soldiers)
        {
            if (s == null) continue;
            float dist = Vector2.Distance(
                s.GetComponent<RectTransform>().position,
                target.GetComponent<RectTransform>().position
            );
            if (dist < minDist) { minDist = dist; nearest = s; }
        }

        return nearest;
    }

    public void OnEnemyDefeated(GameObject enemy)
    {
        _enemies.Remove(enemy);
        Destroy(enemy);
        _enemiesDefeated++;

        if (_enemiesDefeated >= _totalEnemies && _enemies.Count == 0)
            EndBattle(true);
    }

    public void OnEnemyReachedWall()
    {
        if (GameManager.Instance.WallHP - 1 < 0)
            GameManager.Instance.WallHP = 0;
        else
            GameManager.Instance.WallHP--;
        
        UIManager.Instance.RefreshHUD();

        if (GameManager.Instance.WallHP <= 0)
            EndBattle(false);
    } 

    void EndBattle(bool victory)
    {
        _battleActive = false;
        ShootInterval = _baseShootInterval;
        StopAllCoroutines();

        foreach (var a in _arrows) if (a != null) Destroy(a);
        _arrows.Clear();

        int survivingPikes = 0;
        foreach (var p in Pikes)
            if (p != null && p.activeSelf) survivingPikes++;
        GameManager.Instance.StakeCount = survivingPikes;

        if (victory)
        {
            Debug.Log("Ночь пережита!");
            UIManager.Instance.EndDayButton.interactable = true;
        }
        else
        {
            GameManager.Instance.IsGameOver = true;
            GameOverUI.Instance.Show(GameManager.Instance.Day);
        }
    }

    public List<GameObject> GetPikes() => Pikes;
}