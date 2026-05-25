using System.Collections.Generic;
using UnityEngine;

public enum VillagerTask { Idle, Wood, Food, Guard }

public class VillagerManager : MonoBehaviour
{
    public static VillagerManager Instance;

    public List<Villager> Villagers = new();
    private string[] _names = { "Арне", "Берта", "Гунар", "Дагни", "Эйрик", "Фрея" };

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