
using TMPro;
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon;

public class Setting_Privacy : UdonSharpBehaviour
{
    [SerializeField] private BibleOwner owner;
    [SerializeField] private Setting_Privacy_Dropdown dropdown;

    [SerializeField] private GameObject canvas_object;
    [SerializeField] private GameObject mesh_object;
    [SerializeField] private GameObject prop_object;

    [UdonSynced] private int _level_SYNC;
    public int level
    {
        get => _level_SYNC;
        set
        {
            _level_SYNC = value;
            Refresh();
            Networking.SetOwner(Networking.LocalPlayer, gameObject);
            RequestSerialization();

            // dropdown.Sync();
        }
    }

    void Start()
    {
        Sync();
    }

    public override void OnDeserialization()
    {
        Refresh();
    }

    public void Sync()
    {
        level = dropdown.value;
    }

    private void Refresh()
    {
        bool definitely_allow = owner.is_unclaimed_or_owner;

        canvas_object.GetComponent<Canvas>().enabled = definitely_allow || level >= 1;
        canvas_object.GetComponent<GraphicRaycaster>().enabled = definitely_allow || level >= 2;
        canvas_object.GetComponent<Collider>().enabled = definitely_allow || level >= 2;

        if (mesh_object != null)
            mesh_object.SetActive(definitely_allow || level >= 1);

        if (prop_object != null)
            prop_object.GetComponent<Collider>().enabled = definitely_allow || level >= 2;

        dropdown.dropdown.SetValueWithoutNotify(level);
    }
}
