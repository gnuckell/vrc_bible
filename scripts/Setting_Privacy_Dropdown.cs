
using TMPro;
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon;

public class Setting_Privacy_Dropdown : BibleDeedListener
{
    [SerializeField] private GameObject canvas_object;
    [SerializeField] private GameObject mesh_object;
    [SerializeField] private GameObject prop_object;

    [HideInInspector] public TMP_Dropdown dropdown;

    [UdonSynced] private int _level_SYNC;
    public int level
    {
        get => _level_SYNC;
        set {
            _level_SYNC = value;
            Sync();
        }
    }

    private int initial_value;

    protected override void Start()
    {
        dropdown = GetComponent<TMP_Dropdown>();
        initial_value = dropdown.value;

        if (Networking.IsMaster) level = initial_value;

        base.Start();
    }

	public override void Refresh()
	{
        var definitely_allow = deed.is_owner_or_unclaimed;

        canvas_object.GetComponent<Canvas>().enabled = definitely_allow || level >= 1;
        canvas_object.GetComponent<GraphicRaycaster>().enabled = definitely_allow || level >= 2;
        canvas_object.GetComponent<Collider>().enabled = definitely_allow || level >= 2;

        if (mesh_object != null)
            mesh_object.SetActive(definitely_allow || level >= 1);

        if (prop_object != null)
            prop_object.GetComponent<Collider>().enabled = definitely_allow || level >= 2;

        dropdown.interactable = deed.is_owner;
        dropdown.SetValueWithoutNotify(level);
	}

    public void OnValueChanged()
    {
        level = dropdown.value;
    }
}
