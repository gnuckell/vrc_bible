
using TMPro;
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon;

public class ScrollbarDebugger : UdonSharpBehaviour
{
	public GameObject pref_panel_create;
	public TextMeshProUGUI inst_text;
	public ScrollRect inst_scroll;
	public Transform inst_content;

	void Start()
	{

	}

	void Update()
	{
		// inst_text.text = inst_scroll.verticalScrollbar.value.ToString();
	}

	public override void Interact()
	{
		var obj = Instantiate(pref_panel_create, inst_content);
	}
}
