using UnityEngine;
using UnityEngine.UI;

public class WorkZone : MonoBehaviour
{
    public VillagerTask Task;
    public Animator Animator;
    public Material OutlineMaterial;
    public Image OutlineImage;
    private Villager _assignedVillager;
    private Image _image;
    private Material _mat;

    void Start()
    {
        _image = GetComponent<Image>();
        if (OutlineMaterial != null)
        {
            _mat = Instantiate(OutlineMaterial);
            if (_image != null) _image.material = _mat;
        }
        GetComponent<Button>().onClick.AddListener(OnTap);
        Refresh();
    }
    
    public void ResetZone()
    {
        _assignedVillager = null;
        Refresh();
    }

    void OnTap()
    {
        if (GameManager.Instance.Phase != GamePhase.Morning) return;

        var vm = VillagerManager.Instance;

        if (_assignedVillager != null && _assignedVillager.IsAlive)
        {
            _assignedVillager.AssignTask(VillagerTask.Idle);
            _assignedVillager = null;
        }
        else
        {
            var idle = vm.Villagers.Find(v => v.IsAlive && v.Task == VillagerTask.Idle);
            if (idle != null)
            {
                idle.AssignTask(Task);
                _assignedVillager = idle;
            }
        }

        Refresh();
        UIManager.Instance?.RefreshHUD();
    }

    public void Refresh()
    {
        bool isActive = _assignedVillager != null && _assignedVillager.IsAlive;

        if (!isActive) _assignedVillager = null;

        if (_image != null) _image.material = isActive ? null : _mat;
        if (OutlineImage != null) OutlineImage.material = isActive ? null : _mat;
        if (Animator != null) Animator.SetBool("Active", isActive);
    }
}