
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class Setting_FontFamily : Setting_Dropdown
{
    [SerializeField] private GameObject[] font_prefabs;
    [SerializeField] private BibleReader bibleReaderContent;

    public GameObject current_font => font_prefabs[value];

	public override void Refresh()
	{
		base.Refresh();

        if (bibleReaderContent.pref_content != current_font)
        {
            bibleReaderContent.pref_content = current_font;
            bibleReaderContent.ResetContent();
        }
	}
}
