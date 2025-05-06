
using TMPro;
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon;

public class Setting_Privacy : Setting_Dropdown
{
    [SerializeField] private GameObject canvas_object;
    [SerializeField] private GameObject mesh_object;
    [SerializeField] private GameObject prop_object;

	public override void Refresh()
	{
        base.Refresh();

        var definitely_allow = deed.is_owner_or_unclaimed;

        canvas_object.GetComponent<Canvas>().enabled = definitely_allow || value >= 1;
        canvas_object.GetComponent<GraphicRaycaster>().enabled = definitely_allow || value >= 2;
        canvas_object.GetComponent<Collider>().enabled = definitely_allow || value >= 2;

        if (mesh_object != null)
            mesh_object.SetActive(definitely_allow || value >= 1);

        if (prop_object != null)
            prop_object.GetComponent<Collider>().enabled = definitely_allow || value >= 2;
	}
}
