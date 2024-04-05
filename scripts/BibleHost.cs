
using TMPro;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class BibleHost : UdonSharpBehaviour
{
	[SerializeField] public string[] translation_names;
	[SerializeField] public string[] translation_abbrs;
	[SerializeField] public TextAsset[] translation_docs;

	[SerializeField] private BibleReader _reader;
	public BibleReader reader => _reader;

	[SerializeField] private TextMeshProUGUI _chapter_text;
	[SerializeField] private TextMeshProUGUI _book_text;

	[SerializeField] private int _chapter_index = 0;
	public int chapter_index
	{
		get => _chapter_index;
		set
		{
			if (value == _chapter_index) return;
			_chapter_index = value;

			_chapter_text.text = $"{reader.CHAPTER_LOCALS[_chapter_index] + 1}";
			_book_text.text = $"{reader.BOOK_ABBRS[book_index]}";
		}
	}
	public int book_index => reader.CHAPTER_BOOKS[_chapter_index];

	[SerializeField] private GameObject[] _window_object_list;
	[SerializeField] private EBibleWindow _active_window_index;
	public EBibleWindow active_window_index
	{
		get => _active_window_index;
		set
		{
			if (value == _active_window_index) return;
			active_window.SetActive(false);
			_active_window_index = value;
			active_window.SetActive(true);
		}
	}
	public GameObject active_window => _window_object_list[(int)_active_window_index];

	/** <<============================================================>> **/

	void Start()
	{
		foreach (var obj in _window_object_list)
			obj.SetActive(false);
		_window_object_list[(int)_active_window_index].SetActive(true);
	}
}

public enum EBibleWindow
{
	Reader,
	TransSelector,
	BookSelector,
	ChapterSelector,
	Settings,
}
