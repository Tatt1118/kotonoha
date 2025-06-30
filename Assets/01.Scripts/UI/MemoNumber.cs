using UnityEngine;

public class MemoNumber : MonoBehaviour
{
    [SerializeField] private GameObject memo;


    public void OpenMemo()
    {
        memo.SetActive(true);
    }

    public void CloseMemo()
    {
        memo.SetActive(false);
    }
}
