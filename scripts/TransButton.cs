
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public sealed class TransButton : Button
{
	public override void OnClick()
	{
		host.reader.SwitchTranslation(index);
		host.active_window_index = EBibleWindow.Reader;
	}

	protected override string GetButtonText(int i)
	{
		return $"{host.translation_names[i]} <sub>{host.translation_abbrs[i]}</sub>";
	}
}
