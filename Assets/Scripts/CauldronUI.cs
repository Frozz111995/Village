using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CauldronUI : MonoBehaviour
{
    public static CauldronUI Instance;

    [Header("Панель")]
    public GameObject Panel;

    [Header("Ингредиенты")]
    public Transform IngredientsContainer;
    public GameObject IngredientButtonPrefab;

    [Header("Слоты")]
    public Image[] SlotImages;
    public TMP_Text[] SlotLabels;

    [Header("Кнопки")]
    public Button CookButton;
    public Button CloseButton;

    private List<ResourceType> _selectedIngredients = new();
    private Color _emptySlotColor = new Color(0.5f, 0.5f, 0.5f, 0.5f);
    private Color _filledSlotColor = new Color(0.3f, 0.7f, 0.3f, 1f);

    void Awake() => Instance = this;

    void Start()
    {
        CookButton.onClick.AddListener(OnCook);
        CloseButton.onClick.AddListener(OnClose);
        Panel.SetActive(false);
    }

    public void Open()
    {
        Panel.SetActive(true);
        _selectedIngredients.Clear();
        RefreshSlots();
        RefreshIngredients();
    }

    void RefreshIngredients()
    {
        foreach (Transform child in IngredientsContainer)
            Destroy(child.gameObject);

        var resources = GameManager.Instance.Resources;

        foreach (ResourceType type in System.Enum.GetValues(typeof(ResourceType)))
        {
            if (type == ResourceType.Wood) continue;
            if (resources[type] <= 0) continue;

            var go = Instantiate(IngredientButtonPrefab, IngredientsContainer);
            var label = go.GetComponentInChildren<TMP_Text>();
            label.text = $"{TypeToName(type)} x{resources[type]}";

            var btn = go.GetComponent<Button>();
            var capturedType = type;
            btn.onClick.AddListener(() => OnIngredientClicked(capturedType));
        }
    }

    void OnIngredientClicked(ResourceType type)
    {
        if (_selectedIngredients.Count >= 3) return;
        if (_selectedIngredients.Contains(type)) return;

        _selectedIngredients.Add(type);
        RefreshSlots();
    }

    void RefreshSlots()
    {
        for (int i = 0; i < SlotImages.Length; i++)
        {
            if (i < _selectedIngredients.Count)
            {
                SlotImages[i].color = _filledSlotColor;
                SlotLabels[i].text = TypeToName(_selectedIngredients[i]);
            }
            else
            {
                SlotImages[i].color = _emptySlotColor;
                SlotLabels[i].text = "—";
            }
        }

        CookButton.interactable = _selectedIngredients.Count >= 2;
    }

    void OnCook()
    {
        if (_selectedIngredients.Count < 2) return;

        var gm = GameManager.Instance;

        // Проверяем ингредиенты
        foreach (var ing in _selectedIngredients)
        {
            if (gm.Resources[ing] <= 0)
            {
                Debug.Log($"Не хватает: {ing}");
                return;
            }
        }

        // Списываем ингредиенты
        foreach (var ing in _selectedIngredients)
            gm.Resources[ing]--;

        // Ищем рецепт
        var recipe = RecipeBook.Instance.FindRecipe(_selectedIngredients);

        if (recipe != null)
        {
            gm.Portions += recipe.Portions;
            gm.ActiveBuff = recipe.BuffType;
            Debug.Log($"Приготовлено: {recipe.Name} +{recipe.Portions} порций. {recipe.Buff}");
        }
        else
        {
            gm.Portions += 1;
            Debug.Log("Похлёбка. +1 порция");
        }

        _selectedIngredients.Clear();
        RefreshSlots();
        RefreshIngredients();
        UIManager.Instance?.RefreshHUD();
    }

    void OnClose() => Panel.SetActive(false);

    string TypeToName(ResourceType type) => type switch
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