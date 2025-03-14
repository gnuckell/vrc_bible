﻿
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class TabSelector : UdonSharpBehaviour
{
    private int init_value;

    [SerializeField] private GameObject[] _tab_object_list;
    [SerializeField] private int _current_index;
    [UdonSynced] private int _current_index_SYNC;
    public int current_index
    {
        get => _current_index_SYNC;
        set
        {
            value = Mathf.Clamp(value, 0, _tab_object_list.Length - 1);
            if (_current_index_SYNC == value) return;
            _current_index_SYNC = value;
            UpdateCurrentIndex();

            Networking.SetOwner(Networking.LocalPlayer, gameObject);
            RequestSerialization();
        }
    }
    private void UpdateCurrentIndex()
    {
        if (_current_index_SYNC == _current_index) return;
        current_object.SetActive(false);
        _current_index = _current_index_SYNC;
        current_object.SetActive(true);
    }
    public GameObject current_object => _tab_object_list[_current_index];

    private void Start()
    {
        init_value = _current_index;
        _current_index = _current_index_SYNC;
		foreach (var obj in _tab_object_list)
            obj.SetActive(obj == _tab_object_list[_current_index_SYNC]);
    }

    public void Despawn()
    {
        current_index = init_value;
    }

    public override void OnDeserialization()
    {
        UpdateCurrentIndex();
    }

}
