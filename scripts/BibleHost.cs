
using TMPro;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class BibleHost : UdonSharpBehaviour
{
	/// <summary>
	/// Total number books in the Bible.
	///</summary>
	internal const int MAX_BOOK_COUNT = 66;

	/// <summary>
	/// Total number of chapters in the Bible. Used to simplify reference indeces and quicken verse loading.
	///</summary>
	internal const int MAX_CHAPTER_COUNT = 1189;

	internal const int LUT_REF_LENGTH = 8;
	internal const char SEP = '\n';

	/** <<============================================================>> **/

	[SerializeField] public string[] translation_names;
	[SerializeField] public string[] translation_abbrs;
	[SerializeField] public TextAsset[] translation_docs;
	[SerializeField] private TextAsset book_doc;
	[SerializeField] private TextAsset address_doc;

	/** <<============================================================>> **/

	[SerializeField] private BibleReader _reader;
	public BibleReader reader => _reader;

	[SerializeField] private TextMeshProUGUI _chapter_text;
	[SerializeField] private TextMeshProUGUI _book_text;
	[SerializeField] private TextMeshProUGUI _trans_text;

	[SerializeField] private int _trans_index = 0;
	public int trans_index
	{
		get => _trans_index;
		set
		{
			if (_trans_index == value) return;
			_trans_index = value;
			Refresh_trans_index();
		}
	}
	private void Refresh_trans_index()
	{
		_trans_text.text = $"{translation_abbrs[_trans_index]}";
		reader.SwitchTranslation(_trans_index);
	}

	[SerializeField] private int _chapter_index = 0;
	public int chapter_index
	{
		get => _chapter_index;
		set
		{
			if (value == _chapter_index) return;
			_chapter_index = value;
			Refresh_chapter_index();
		}
	}
	private void Refresh_chapter_index()
	{
		_chapter_text.text = $"{CHAPTER_LOCALS[_chapter_index] + 1}";
		_book_text.text = $"{BOOK_ABBRS[book_index]}";
	}
	public int book_index => CHAPTER_BOOKS[_chapter_index];

	[SerializeField] private GameObject[] _window_object_list;
	[SerializeField] private EBibleWindow _active_window_index = EBibleWindow.BookSelector;
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

	private string book_lut;
	private string address_lut;

	public readonly string[] BOOK_NAMES = new string[MAX_BOOK_COUNT];
	public readonly string[] BOOK_ABBRS = new string[MAX_BOOK_COUNT];
	public readonly int[] BOOK_LENGTHS = new int[MAX_BOOK_COUNT];
	public readonly int[] BOOK_HEADS = new int[MAX_BOOK_COUNT];

	public readonly int[] CHAPTER_LOCALS = new int[MAX_CHAPTER_COUNT];
	public readonly int[] CHAPTER_BOOKS = new int[MAX_CHAPTER_COUNT];
	public readonly int[] CHAPTER_LENGTHS = new int[MAX_CHAPTER_COUNT];

	/** <<============================================================>> **/

	void Start()
	{
		book_lut = book_doc.text;

		var head = 0;
		for (var i = 0; i < MAX_BOOK_COUNT; i++)
		{
			var line_start = BibleUtils.NthIndexOf(book_lut, SEP, i - 1);
			var line_end = BibleUtils.NthIndexOf(book_lut, SEP, i) - 1;

			var line = book_lut.Substring(line_start, line_end - line_start);

			var name_tail = BibleUtils.NthIndexOf(line, ',', 0);
			var abbr_tail = BibleUtils.NthIndexOf(line, ',', 1);

			BOOK_NAMES[i] = line.Substring(0, name_tail - 1);
			BOOK_ABBRS[i] = line.Substring(name_tail, abbr_tail - name_tail - 1);
			BOOK_LENGTHS[i] = int.Parse(line.Substring(abbr_tail));
			BOOK_HEADS[i] = head;

			head += BOOK_LENGTHS[i];
		}

		address_lut = address_doc.text;

		var b = 0;
		var l = 0;
		var v = 0;
		for (var i = 0; i < MAX_CHAPTER_COUNT; i++)
		{
			var v_address = GetAddress(v);
			var j = 0;
			do j++; while (AddressesShareChapter(v_address, GetAddress(v + j)));

			CHAPTER_LENGTHS[i] = j;
			CHAPTER_BOOKS[i] = b;
			CHAPTER_LOCALS[i] = l;

			v += j;
			if (AddressesShareBook(v_address, GetAddress(v)))
				l++;
			else
			{
				l = 0;
				b++;
			}
		}

		reader.Init();
		Refresh_trans_index();
		Refresh_chapter_index();
		foreach (var obj in _window_object_list)
			obj.SetActive(false);
		_window_object_list[(int)_active_window_index].SetActive(true);
	}

	public int GetChapterFromLocals(int book, int chapter) => BOOK_HEADS[book] + chapter;
	public string GetAddress(int line) => address_lut.Substring(line * (LUT_REF_LENGTH + 1), LUT_REF_LENGTH);

	private bool AddressesShareBook(string a, string b) => a.Substring(0, 2) == b.Substring(0, 2);
	private bool AddressesShareChapter(string a, string b) => a.Substring(0, 5) == b.Substring(0, 5);
}

public enum EBibleWindow
{
	Reader,
	TransSelector,
	BookSelector,
	ChapterSelector,
	Settings,
}
