
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class Setting_FontFamily : Setting_Dropdown
{
    [SerializeField] private GameObject[] font_prefabs;
    [SerializeField] private BibleReaderContent bibleReaderContent;

	public override void Refresh()
	{
		base.Refresh();

        if (bibleReaderContent.pref_content != font_prefabs[value])
        {
            bibleReaderContent.pref_content = font_prefabs[value];
            bibleReaderContent.ResetContent();
        }
	}
}
