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

public enum VillagerBuff { None, HPBonus, ExpeditionBonus }

public class RecipeBook : MonoBehaviour
{
    public static RecipeBook Instance;

    private List<Recipe> _recipes = new()
    {
        new Recipe {
            Name = "Грибной суп",
            Ingredients = new[] { ResourceType.Berries, ResourceType.Mushrooms },
            Portions = 2,
            Buff = "+1 выносливость",
            BuffType = VillagerBuff.ExpeditionBonus
        },
        new Recipe {
            Name = "Жаркое",
            Ingredients = new[] { ResourceType.Meat, ResourceType.Roots },
            Portions = 3,
            Buff = "+1 HP жителям",
            BuffType = VillagerBuff.HPBonus
        },
        new Recipe {
            Name = "Уха",
            Ingredients = new[] { ResourceType.Fish, ResourceType.Herbs },
            Portions = 2,
            Buff = "+выносливость в вылазке",
            BuffType = VillagerBuff.ExpeditionBonus
        },
    };

    void Awake() => Instance = this;

    public Recipe FindRecipe(List<ResourceType> ingredients)
    {
        foreach (var recipe in _recipes)
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