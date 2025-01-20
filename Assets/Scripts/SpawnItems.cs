using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnItems : MonoBehaviour
{
    private Transform _itemParent;

    [SerializeField] private string _key;
    private int _savedItemIndex;

    private void Start()
    {
        _itemParent = GetComponent<Transform>();

        for (int i = 0; i < _itemParent.childCount; i++)
            _itemParent.GetChild(i).gameObject.SetActive(false);

        _savedItemIndex = PlayerPrefs.HasKey(_key) ? PlayerPrefs.GetInt(_key) : 0;

        if (_savedItemIndex < _itemParent.childCount)
            _itemParent.GetChild(_savedItemIndex).gameObject.SetActive(true);
    }
}
