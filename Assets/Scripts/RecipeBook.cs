using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Recipe
{
    public string Name;
    public ResourceType[] Ingredients;
    public int Portions;
    public string Buff;
    public VillagerBuff BuffType;
}

public enum VillagerBuff { None, ExpeditionBonus, CombatBonus, ProductionBonus }

public class RecipeBook : MonoBehaviour
{
    public static RecipeBook Instance;

    public List<Recipe> Recipes = new()
    {
        new Recipe {
            Name = "Грибной суп",
            Ingredients = new[] { ResourceType.Berries, ResourceType.Mushrooms },
            Portions = 1,
            Buff = "+1 действие в лесу",
            BuffType = VillagerBuff.ExpeditionBonus
        },
        new Recipe {
            Name = "Жаркое",
            Ingredients = new[] { ResourceType.Meat, ResourceType.Roots },
            Portions = 1,
            Buff = "Солдаты стреляют быстрее",
            BuffType = VillagerBuff.CombatBonus
        },
        new Recipe {
            Name = "Уха",
            Ingredients = new[] { ResourceType.Fish, ResourceType.Herbs },
            Portions = 1,
            Buff = "+1 ресурс жителям",
            BuffType = VillagerBuff.ProductionBonus
        },
        new Recipe {
            Name = "Овощной суп",
            Ingredients = new[] { ResourceType.Vegetables, ResourceType.Herbs },
            Portions = 2,
            Buff = "+1 порция",
            BuffType = VillagerBuff.None
        },
    };

    void Awake() => Instance = this;

    public Recipe FindRecipe(List<ResourceType> ingredients)
    {
        foreach (var recipe in Recipes)
        {
            if (recipe.Ingredients.Length != ingredients.Count) continue;

            bool match = true;
            foreach (var ing in recipe.Ingredients)
            {
                if (!ingredients.Contains(ing)) { match = false; break; }
            }
            if (match) return recipe;
        }
        return null;
    }
}