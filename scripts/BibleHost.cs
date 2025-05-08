
using TMPro;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class BibleHost : UdonSharpBehaviour
{
	#region Constants

	internal const int LUT_REF_LENGTH = 8;
	internal const int LUT_REF_PREFIX_LENGTH = 9;
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

	[SerializeField] private TabSelector _tab_selector;
	[SerializeField] private ButtonGrid _chapter_selector;

	[Header("Settings")]

	private int _chapter_index = 0;
	[UdonSynced] private int _chapter_index_SYNC = 0;
	public int chapter_index
	{
		get => _chapter_index;
		set
		{
			if (value == _chapter_index_SYNC) return;
			_chapter_index_SYNC = value;
			UpdateChapterIndex();

			Networking.SetOwner(Networking.LocalPlayer, gameObject);
			RequestSerialization();
		}
	}
	private void UpdateChapterIndex()
	{
		if (_chapter_index == _chapter_index_SYNC) return;
		_chapter_index = _chapter_index_SYNC;

		_chapter_text.text = $"{chapter_locals[_chapter_index] + 1}";
		_book_text.text = $"{book_names_short[book_index]}";
	}
	public int book_index => chapter_books[_chapter_index];

	public EBibleWindow active_window_index
	{
		get => (EBibleWindow)_tab_selector.current_index;
		set => _tab_selector.current_index = (int)value;
	}
	private string book_lut;
	private string address_lut;
	private string _content_lut;
	internal string content_lut { get => _content_lut; private set => _content_lut = value; }

	/// <summary>
	/// Total number books in the translation.
	///</summary>
	internal int max_book_count = 66;

	/// <summary>
	/// Total number of chapters in the translation. Used to simplify reference indeces and quicken verse loading.
	///</summary>
	internal int max_chapter_count = 1189;

	// Full names of each book, e.g. Genesis, Exodus, Leviticus
	internal string[] book_names;
	// Abbreviated names of each book, e.g. GEN, EXO, LEV
	internal string[] book_names_short;
	// The length (in chapters) of each book.
	internal int[] book_lengths;
	// The head chapter address (in the global chapter index) of each book.
	internal int[] book_heads;

	// The chapter number each chapter is, relative to the book it's in.
	internal int[] chapter_locals;
	// The book each chapter is located in (this array has a lot of repitition).
	internal int[] chapter_books;
	// The number of verses in each chapter.
	internal int[] chapter_lengths;
	// I forgor
	internal int[] chapter_heads;

	internal int current_book => chapter_books[chapter_index];

	internal int current_book_head => book_heads[current_book];
	internal int current_book_length => book_lengths[current_book];

	#endregion

	void Start()
	{
		trans_default.UpdateHost();
	}

	public void Despawn()
	{
		chapter_index = 0;
		active_window_index = EBibleWindow.BookSelector;
	}

	public override void OnDeserialization()
	{
		UpdateChapterIndex();
	}

	public void Init(string name, string abbr, TextAsset books, TextAsset address, TextAsset content)
	{
		max_book_count = 66;
		max_chapter_count = 1189;

		book_names = new string[max_book_count];
		book_names_short = new string[max_book_count];
		book_lengths = new int[max_book_count];
		book_heads = new int[max_book_count];

		chapter_locals = new int[max_chapter_count];
		chapter_books = new int[max_chapter_count];
		chapter_lengths = new int[max_chapter_count];
		chapter_heads = new int[max_chapter_count];

		book_lut = books.text;
		address_lut = address.text;
		content_lut = content.text;

#if UNITY_EDITOR
		/**	Validate files
		*/
		if (address_lut[LUT_REF_LENGTH] == '\r')
		{
			Debug.LogError($"The address document '{address.name}' uses CRLF line endings, please change to LF.");
			return;
		}
		if (address_lut.Substring(address_lut.Length - LUT_REF_LENGTH, LUT_REF_LENGTH) != string.Empty.PadRight(LUT_REF_LENGTH, '0'))
		{
			Debug.LogError($"The address document '{address.name}' must end with a terminating string of zeroes ('{string.Empty.PadRight(LUT_REF_LENGTH, '0')}')");
			return;
		}
#endif
		/**	Make MAX_CHAPTER_COUNT a variable and set it here depending on the address doc?
		*/

		/**	Set indeces for each book.
		*/
		var head = 0;
		for (var i = 0; i < max_book_count; i++)
		{
			var line_start = BibleUtils.NthIndexOf(book_lut, SEP, i - 1);
			var line_end = BibleUtils.NthIndexOf(book_lut, SEP, i) - 1;

			var line = book_lut.Substring(line_start, line_end - line_start);

			var name_tail = BibleUtils.NthIndexOf(line, ',', 0);
			var abbr_tail = BibleUtils.NthIndexOf(line, ',', 1);

			book_names[i] = line.Substring(0, name_tail - 1);
			book_names_short[i] = line.Substring(name_tail, abbr_tail - name_tail - 1);
			book_lengths[i] = int.Parse(line.Substring(abbr_tail));
			book_heads[i] = head;

			head += book_lengths[i];
		}

		/**	Set indeces for each chapter.
		*/
		var b = 0;
		var l = 0;
		var v = 0;
		var h = 0;
		for (var i = 0; i < chapter_heads.Length; i++)
		{
			var v_address = GetAddress(v);
			var j = 0;
			do j++; while (AddressesShareChapter(v_address, GetAddress(v + j)));

			chapter_books[i] = b;
			chapter_locals[i] = l;
			chapter_lengths[i] = j;
			chapter_heads[i] = h;

			// Debug.Log($"[{i}] book={b}, local={l}, length={j}, head={h}");

			h = BibleUtils.NthIndexOf(content_lut, SEP, chapter_lengths[i] - 1, h);
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
		_chapter_selector.ResetChildren();


		UpdateChapterIndex();
		reader.Init();
	}

	public void OnClose()
	{
		Destroy(close_object);
	}

	/** <<============================================================>> **/

	public int GetChapterFromLocals(int book, int chapter) => book_heads[book] + chapter;
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
