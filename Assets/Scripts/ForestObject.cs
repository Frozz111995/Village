using UnityEngine;
using UnityEngine.UI;

public class ForestObject : MonoBehaviour
{
    public ResourceType Resource;
    public int Amount = 1;

    private Button _button;
    private Image _image;
    private Color _activeColor = new Color(1f, 1f, 1f, 1f);
    private Color _depleteColor = new Color(0.5f, 0.5f, 0.5f, 0.4f);
    public Material OutlineMaterial;
    public void Setup(ResourceType resource, int amount, Sprite sprite, Vector2 size, float outlineWidth)
    {
        Resource = resource;
        Amount = amount;

        _button = GetComponent<Button>();
        _image = GetComponent<Image>();
        _image.sprite = sprite;
        _image.preserveAspect = true;

        var mat = Instantiate(OutlineMaterial);
        mat.SetFloat("_OutlineWidth", outlineWidth);
        _image.material = mat;

        GetComponent<RectTransform>().sizeDelta = size;
        _button.onClick.AddListener(OnTap);
    }

    void OnTap()
    {
        if (!_button.interactable) return;
        if (ForestSpawner.Instance.ActionsLeft <= 0) return;

        GameManager.Instance.AddResource(Resource, Amount);
        ForestSpawner.Instance.UseAction();
        Deplete();

        if (ForestSpawner.Instance.TryFindSurvivor())
        {
            VillagerManager.Instance.AddSurvivor();
            Debug.Log("Найден выживший!");
        }
    }

    void Deplete()
    {
        _button.interactable = false;
        _image.raycastTarget = false;
        _image.color = _depleteColor;
    }

    public void Reset()
    {
        _button.interactable = true;
        _image.raycastTarget = true;
        _image.color = _activeColor;
    }
}