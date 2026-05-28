using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CauldronUI : MonoBehaviour
{
    public static CauldronUI Instance;

    [Header("Панель")]
    public GameObject Panel;
    public GameObject IconSpawnRoot;
    [Header("Ингредиенты")]
    public Transform IngredientsContainer;
    public GameObject IngredientButtonPrefab;

    [Header("Иконки ингредиентов")]
    public IngredientData[] IngredientDataList;

    [Header("Слоты")]
    public Image[] SlotImages;

    [Header("Кнопки")]
    public Button CookButton;
    public Button CloseButton;
    public Button CauldronButton;

    [Header("Котёл")]
    public RectTransform CauldronRect;

    private List<ResourceType> _selectedIngredients = new();
    private Dictionary<ResourceType, Sprite> _icons = new();
    private List<GameObject> _flyingIcons = new();
    private Color _emptySlotColor = new Color(1f, 1f, 1f, 0.2f);
    private Color _filledSlotColor = new Color(1f, 1f, 1f, 1f);

    void Awake()
    {
        Instance = this;
        foreach (var data in IngredientDataList)
            _icons[data.Type] = data.Icon;
    }

    void Start()
    {
        CookButton.onClick.AddListener(OnCook);
        CloseButton.onClick.AddListener(OnClose);
        CauldronButton.onClick.AddListener(Open);
        Panel.SetActive(false);
    }

    public void Open()
    {
        foreach (var icon in _flyingIcons)
            if (icon != null) Destroy(icon);
        _flyingIcons.Clear();

        Panel.SetActive(true);
        _selectedIngredients.Clear();
        RefreshSlots();
        RefreshIngredients();
        RefreshRecipeButtons();
    }

    void RefreshRecipeButtons()
    {
        foreach (var btn in FindObjectsByType<RecipeButton>(FindObjectsInactive.Exclude))
            btn.Refresh();
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

            var tag = go.AddComponent<IngredientTag>();
            tag.Type = type;

            var icon = go.transform.Find("Icon")?.GetComponent<Image>();
            if (icon != null && _icons.TryGetValue(type, out var sprite))
                icon.sprite = sprite;

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

        var btn = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject;
        if (btn != null)
        {
            var rect = btn.GetComponent<RectTransform>();
            var flying = IngredientFlyAnimation.Play(rect, CauldronRect, IconSpawnRoot.transform, Vector2.zero, () =>
            {
                _selectedIngredients.Add(type);
                RefreshSlots();
            });
            if (flying != null) _flyingIcons.Add(flying);
        }
        else
        {
            _selectedIngredients.Add(type);
            RefreshSlots();
        }
    }

    void RefreshSlots()
    {
        for (int i = 0; i < SlotImages.Length; i++)
        {
            if (i < _selectedIngredients.Count)
            {
                SlotImages[i].color = _filledSlotColor;
                if (_icons.TryGetValue(_selectedIngredients[i], out var sprite))
                    SlotImages[i].sprite = sprite;
            }
            else
            {
                SlotImages[i].color = _emptySlotColor;
                SlotImages[i].sprite = null;
            }
        }

        CookButton.interactable = _selectedIngredients.Count >= 2;
    }

    void OnCook()
    {
        if (_selectedIngredients.Count < 2) return;

        var gm = GameManager.Instance;

        foreach (var ing in _selectedIngredients)
        {
            if (gm.Resources[ing] <= 0)
            {
                Debug.Log($"Не хватает: {ing}");
                return;
            }
        }

        foreach (var ing in _selectedIngredients)
            gm.Resources[ing]--;

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
        RefreshRecipeButtons();
        foreach (var icon in _flyingIcons)
            if (icon != null) Destroy(icon);
        _flyingIcons.Clear(); 
        UIManager.Instance?.RefreshHUD();
    }

    public void SelectRecipe(Recipe recipe)
    {
        _selectedIngredients.Clear();
        foreach (var ing in recipe.Ingredients)
            _selectedIngredients.Add(ing);
        RefreshSlots();
    }

    public void SelectRecipeWithAnimation(Recipe recipe)
    {
        _selectedIngredients.Clear();
        RefreshSlots();
        StartCoroutine(AddIngredientsOneByOne(recipe));
    }

    IEnumerator AddIngredientsOneByOne(Recipe recipe)
    {
        foreach (var ing in recipe.Ingredients)
        {
            foreach (Transform child in IngredientsContainer)
            {
                var tag = child.GetComponent<IngredientTag>();
                if (tag == null || tag.Type != ing) continue;

                var rect = child.GetComponent<RectTransform>();
                bool done = false;

                var flying = IngredientFlyAnimation.Play(rect, CauldronRect, IconSpawnRoot.transform, Vector2.zero, () =>
                {
                    _selectedIngredients.Add(ing);
                    RefreshSlots();
                    done = true;
                });
                if (flying != null) _flyingIcons.Add(flying);

                yield return new WaitUntil(() => done);
                break;
            }
        }
    }

    void OnClose()
    {
        foreach (var icon in _flyingIcons)
            if (icon != null) Destroy(icon);
        _flyingIcons.Clear();

        Panel.SetActive(false);
    }

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