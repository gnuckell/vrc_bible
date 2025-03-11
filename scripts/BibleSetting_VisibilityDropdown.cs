
using TMPro;
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon;

public class BibleSetting_VisibilityDropdown : UdonSharpBehaviour
{
    [SerializeField] private BibleOwner owner;
    [SerializeField] private TMP_Dropdown dropdown;

    [SerializeField] private GameObject canvas_object;
    [SerializeField] private GameObject mesh_object;
    [SerializeField] private GameObject prop_object;

    [UdonSynced] private int _dropdown_value_SYNC;

    void Start()
    {
        UpdatePrivacy();
    }

    public override void OnDeserialization()
    {
        dropdown.SetValueWithoutNotify(_dropdown_value_SYNC);
        UpdatePrivacy();
    }

    public void OnValueChanged()
    {
        _dropdown_value_SYNC = dropdown.value;

        owner.ClaimLocal();
        UpdatePrivacy();
        Networking.SetOwner(Networking.LocalPlayer, gameObject);
        RequestSerialization();
    }

    private void UpdatePrivacy()
    {
        bool definitely_allow = owner.is_unclaimed_or_owner;

        canvas_object.GetComponent<Canvas>().enabled = definitely_allow || _dropdown_value_SYNC >= 1;
        canvas_object.GetComponent<GraphicRaycaster>().enabled = definitely_allow || _dropdown_value_SYNC >= 2;
        canvas_object.GetComponent<Collider>().enabled = definitely_allow || _dropdown_value_SYNC >= 2;

        if (mesh_object != null)
            mesh_object.SetActive(definitely_allow || _dropdown_value_SYNC >= 1);

        if (prop_object != null)
            prop_object.GetComponent<Collider>().enabled = definitely_allow || _dropdown_value_SYNC >= 2;
    }
}
