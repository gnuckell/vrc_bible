
using TMPro;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class Setting_Privacy_Dropdown : UdonSharpBehaviour
{
    [SerializeField] private Bible owner;
    [SerializeField] private Setting_Privacy privacy;
    [SerializeField] public TMP_Dropdown dropdown;

    private int initial_value;
	private int previous_value;
    public int value => dropdown.value;

    void Start()
    {
        initial_value = dropdown.value;
        previous_value = initial_value;
    }

    public void Despawn()
    {
        dropdown.SetValueWithoutNotify(initial_value);
        previous_value = initial_value;
        privacy.Despawn();
    }

    public override void OnDeserialization()
    {
        dropdown.SetValueWithoutNotify(privacy.level);
		previous_value = dropdown.value;
    }

    public void OnValueChanged()
    {
		if (owner.is_owner)
		{
	        owner.ClaimLocal();
			previous_value = dropdown.value;
		}
		else
		{
			dropdown.SetValueWithoutNotify(previous_value);
		}
    }

    public void Sync()
    {
        Networking.SetOwner(Networking.LocalPlayer, gameObject);
        RequestSerialization();
    }
}
