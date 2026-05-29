using System.Collections.Generic;
using UnityEngine;

public class CauldronSelectionState : MonoBehaviour
{
    public static CauldronSelectionState Instance;

    private List<ResourceType> _selected = new();
    private List<ResourceType> _pending = new();
    private bool _recipeAnimating = false;

    public IReadOnlyList<ResourceType> Selected => _selected;
    public IReadOnlyList<ResourceType> Pending => _pending;
    public bool IsLocked => _recipeAnimating;

    void Awake() => Instance = this;

    public bool CanAdd(ResourceType type)
    {
        if (_recipeAnimating) return false;
        if (_selected.Count + _pending.Count >= 3) return false;
        if (_selected.Contains(type)) return false;
        if (_pending.Contains(type)) return false;
        return true;
    }

    public void Reserve(ResourceType type) => _pending.Add(type);

    public void Confirm(ResourceType type)
    {
        _pending.Remove(type);
        _selected.Add(type);
    }

    public void LockForRecipe() => _recipeAnimating = true;
    public void Unlock() => _recipeAnimating = false;

    public void Clear()
    {
        _selected.Clear();
        _pending.Clear();
        _recipeAnimating = false;
    }

    public int Count => _selected.Count + _pending.Count;
}