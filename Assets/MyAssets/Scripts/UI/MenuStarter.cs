using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class MenuStarter : MonoBehaviour
{
    [SerializeField] private GameObject firstSelected;

    private IEnumerator Start()
    {
        yield return null;

        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(firstSelected);
    }
}