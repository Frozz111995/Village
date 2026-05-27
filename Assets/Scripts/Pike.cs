using UnityEngine;
using UnityEngine.UI;

public class Pike : MonoBehaviour
{
    private int _hp = 2;
    public bool IsAlive => _hp > 0;

    public void TakeDamage()
    {
        _hp--;
        if (_hp <= 0)
        {
            GetComponent<Image>().color = new Color(0.3f, 0.3f, 0.3f, 0.5f);
            Destroy(gameObject, 0.5f);
        }
    }
}