using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class RecipeButton : MonoBehaviour
{
    public int RecipeIndex;

    [Header("UI Text Fields")]
    [SerializeField] private TMP_Text _titleLabel;
    [SerializeField] private TMP_Text _ingredientsLabel;
    [SerializeField] private TMP_Text _buffLabel;

    private Recipe _recipe;
    private Button _button;

    void Awake()
    {
        _button = GetComponent<Button>();
    }

    void Start()
    {
        _recipe = RecipeBook.Instance.Recipes[RecipeIndex];
        _button.onClick.AddListener(OnClick);
        UpdateLabels();
    }

    void OnClick()
    {
        if (CauldronSelectionState.Instance.IsLocked) return;

        var gm = GameManager.Instance;
        bool hasIngredients = true;

        foreach (var ing in _recipe.Ingredients)
        {
            if (gm.Resources[ing] <= 0)
            {
                hasIngredients = false;
                break;
            }
        }

        if (!hasIngredients) return;

        CauldronUI.Instance.SelectRecipeWithAnimation(_recipe);
    }

    public void Refresh()
    {
        if (_button == null) _button = GetComponent<Button>();
        if (_recipe == null) _recipe = RecipeBook.Instance.Recipes[RecipeIndex];

        var gm = GameManager.Instance;
        bool hasIngredients = true;

        foreach (var ing in _recipe.Ingredients)
        {
            if (gm.Resources[ing] <= 0)
            {
                hasIngredients = false;
                break;
            }
        }

        _button.interactable = hasIngredients;
    }

    void UpdateLabels()
    {
        if (_titleLabel != null)
            _titleLabel.text = _recipe.Name;

        if (_ingredientsLabel != null)
        {
            List<string> ingredientNames = new List<string>();
            foreach (var ing in _recipe.Ingredients)
                ingredientNames.Add(IngToName(ing));
            _ingredientsLabel.text = $"Ингредиенты: {string.Join(" + ", ingredientNames)}";
        }

        if (_buffLabel != null)
        {
            if (!string.IsNullOrEmpty(_recipe.Buff))
            {
                _buffLabel.gameObject.SetActive(true);
                _buffLabel.text = $"Эффект: {_recipe.Buff}";
            }
            else
            {
                _buffLabel.gameObject.SetActive(false);
            }
        }
    }

    string IngToName(ResourceType type) => type switch
    {
        ResourceType.Berries    => "Ягоды",
        ResourceType.Mushrooms  => "Грибы",
        ResourceType.Fish       => "Рыба",
        ResourceType.Meat       => "Мясо",
        ResourceType.Roots      => "Коренья",
        ResourceType.Herbs      => "Травы",
        ResourceType.Vegetables => "Овощи",
        _                       => type.ToString()
    };
}