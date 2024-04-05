
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public sealed class ChapterSelector : ButtonGrid
{
	protected override void Start() { }

	private void OnEnable()
	{
		Reset(host.chapter_index);
	}

	public override void Reset(int index)
	{
		var book = host.reader.CHAPTER_BOOKS[index];
		max_buttons = host.reader.BOOK_LENGTHS[book];

		base.Reset(book);
	}

	protected override int GetButtonIndex(int index, int local) => host.reader.GetChapterFromLocals(index, local);
}
