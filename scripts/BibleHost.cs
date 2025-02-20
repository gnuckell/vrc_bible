
using TMPro;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class BibleHost : UdonSharpBehaviour
{
	#region Constants

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

	#endregion
	#region Pinions

	/** <<============================================================>> **/

	[Header("References")]

	public GameObject close_object;

	public TransButton trans_default;

	[SerializeField] private BibleReaderContent _reader;
	public BibleReaderContent reader => _reader;

	[SerializeField] private TextMeshProUGUI _trans_text;
	[SerializeField] private TextMeshProUGUI _book_text;
	[SerializeField] private TextMeshProUGUI _chapter_text;

	[Header("Settings")]

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
	private EBibleWindow _active_window_index = EBibleWindow.BookSelector;
	[UdonSynced] private EBibleWindow _active_window_index_SYNC = EBibleWindow.BookSelector;
	public EBibleWindow active_window_index
	{
		get => _active_window_index_SYNC;
		set
		{
			if (value == _active_window_index_SYNC) return;
			_active_window_index_SYNC = value;
			UpdateActiveWindowIndex();

			Networking.SetOwner(Networking.LocalPlayer, gameObject);
			RequestSerialization();
		}
	}
	private void UpdateActiveWindowIndex()
	{
		if (_active_window_index_SYNC == _active_window_index) return;
		active_window.SetActive(false);
		_active_window_index = _active_window_index_SYNC;
		active_window.SetActive(true);
	}
	public GameObject active_window => _window_object_list[(int)_active_window_index];

	private string book_lut;
	private string address_lut;
	private string _content_lut;
	internal string content_lut { get => _content_lut; private set => _content_lut = value; }

	internal readonly string[] BOOK_NAMES = new string[MAX_BOOK_COUNT];
	internal readonly string[] BOOK_ABBRS = new string[MAX_BOOK_COUNT];
	internal readonly int[] BOOK_LENGTHS = new int[MAX_BOOK_COUNT];
	internal readonly int[] BOOK_HEADS = new int[MAX_BOOK_COUNT];

	internal readonly int[] CHAPTER_LOCALS = new int[MAX_CHAPTER_COUNT];
	internal readonly int[] CHAPTER_BOOKS = new int[MAX_CHAPTER_COUNT];
	internal readonly int[] CHAPTER_LENGTHS = new int[MAX_CHAPTER_COUNT];
	internal readonly int[] CHAPTER_HEADS = new int[MAX_CHAPTER_COUNT];

	#endregion

	void Start()
	{
		trans_default.UpdateHost();
	}

	public override void OnDeserialization()
	{
		UpdateActiveWindowIndex();
	}

	public void Init(string name, string abbr, TextAsset books, TextAsset address, TextAsset content)
	{
		book_lut = books.text;
		address_lut = address.text;
		content_lut = content.text;

#if UNITY_EDITOR
		/**	Validate files
		*/
		if (address_lut[8] == '\r')
		{
			Debug.LogError($"The address document '{address.name}' uses CRLF line endings, please change to LF.");
			return;
		}
		if (address_lut.Substring(address_lut.Length - LUT_REF_LENGTH, LUT_REF_LENGTH) != "00000000")
		{
			Debug.LogError($"The address document '{address.name}' must end with a terminating string of zeroes ('00000000')");
			return;
		}
#endif
		/**	Make MAX_CHAPTER_COUNT a variable and set it here depending on the address doc?
		*/

		/**	Set indeces for each book.
		*/
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

		/**	Set indeces for each chapter.
		*/
		var b = 0;
		var l = 0;
		var v = 0;
		var h = 0;
		for (var i = 0; i < CHAPTER_HEADS.Length; i++)
		{
			var v_address = GetAddress(v);
			var j = 0;
			do j++; while (AddressesShareChapter(v_address, GetAddress(v + j)));

			CHAPTER_BOOKS[i] = b;
			CHAPTER_LOCALS[i] = l;
			CHAPTER_LENGTHS[i] = j;
			CHAPTER_HEADS[i] = h;

			// Debug.Log($"[{i}] book={b}, local={l}, length={j}, head={h}");

			h = BibleUtils.NthIndexOf(content_lut, SEP, CHAPTER_LENGTHS[i] - 1, h);
			v += j;
			if (AddressesShareBook(v_address, GetAddress(v)))
				l++;
			else
			{
				l = 0;
				b++;
			}
		}

		_trans_text.text = abbr;
		Refresh_chapter_index();
		reader.Init();
		foreach (var obj in _window_object_list)
			obj.SetActive(false);
		_window_object_list[(int)_active_window_index].SetActive(true);
	}

	public void OnClose()
	{
		Destroy(close_object);
	}

	/** <<============================================================>> **/

	public int GetChapterFromLocals(int book, int chapter) => BOOK_HEADS[book] + chapter;
	public string GetAddress(int line) => address_lut.Substring(line * (LUT_REF_LENGTH + 1), LUT_REF_LENGTH);

	private bool AddressesShareBook(string a, string b) => a.Substring(0, 2) == b.Substring(0, 2);
	private bool AddressesShareChapter(string a, string b) => a.Substring(0, 5) == b.Substring(0, 5);
}

public enum EBibleWindow
{
	Settings,
	TransSelector,
	BookSelector,
	ChapterSelector,
	Reader,
}
