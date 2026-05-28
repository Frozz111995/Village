using System.Collections.Generic;
using UnityEngine;

public enum VillagerTask
{
    Idle,
    Garden,    // огород — овощи
    Woodpile,  // поленница — дерево
    Pigs,      // свиньи — мясо (долгий цикл)
    Foraging,  // за забором — случайный сбор
    Kitchen    // кухня — 1 порция из ничего
}

public class VillagerManager : MonoBehaviour
{
    public static VillagerManager Instance;

    public List<Villager> Villagers = new();
    private string[] _names = { "Арне", "Берта", "Гунар", "Дагни", "Эйрик", "Фрея" };

    private Dictionary<Villager, int> _pigDays = new();

    void Awake()
    {
        Instance = this;
        foreach (var name in _names)
            Villagers.Add(new Villager(name));
    }

    public void ProcessEndOfDay(int portions)
    {
        int food = portions;

        foreach (var v in Villagers)
        {
            if (!v.IsAlive) continue;

            if (food > 0)
            {
                v.Feed();
                food--;
            }
            else
            {
                v.Starve();
            }
        }

        Villagers.RemoveAll(v => !v.IsAlive);
        Debug.Log($"Живых жителей: {Villagers.Count}");
    }

    public int ProcessKitchen()
    {
        int portions = 0;
        foreach (var v in Villagers)
            if (v.IsAlive && v.Task == VillagerTask.Kitchen)
                portions++;
        return portions;
    }

    public CollectedResources ProcessTasks()
    {
        var result = new CollectedResources();
        bool productionBonus = GameManager.Instance.ActiveBuff == VillagerBuff.ProductionBonus;
        int bonus = productionBonus ? 1 : 0;

        foreach (var v in Villagers)
        {
            if (!v.IsAlive) continue;

            switch (v.Task)
            {
                case VillagerTask.Garden:
                    result.Vegetables += 1 + bonus;
                    break;

                case VillagerTask.Woodpile:
                    result.Wood += 1 + bonus;
                    break;

                case VillagerTask.Pigs:
                    if (!_pigDays.ContainsKey(v)) _pigDays[v] = 0;
                    _pigDays[v]++;
                    if (_pigDays[v] >= 3)
                    {
                        result.Meat += 2 + bonus;
                        _pigDays[v] = 0;
                        Debug.Log($"{v.Name} вырастил свинью — мясо +{2 + bonus}");
                    }
                    break;

                case VillagerTask.Foraging:
                    ProcessForaging(result, bonus);
                    break;
            }
        }

        return result;
    }

    void ProcessForaging(CollectedResources result, int bonus)
    {
        int roll = Random.Range(0, 4);
        switch (roll)
        {
            case 0: result.Berries += 1 + bonus; break;
            case 1: result.Mushrooms += 1 + bonus; break;
            case 2: result.Herbs += 1 + bonus; break;
            case 3: result.Wood += 1 + bonus; break;
        }
    }

    public void AddSurvivor()
    {
        var name = $"Странник {Villagers.Count + 1}";
        Villagers.Add(new Villager(name));
        Debug.Log($"Новый житель: {name}");
    }

    public int CollectResources(VillagerTask task)
    {
        int count = 0;
        foreach (var v in Villagers)
            if (v.IsAlive && v.Task == task) count++;
        return count;
    }
}

public class CollectedResources
{
    public int Wood;
    public int Vegetables;
    public int Meat;
    public int Berries;
    public int Mushrooms;
    public int Herbs;
}