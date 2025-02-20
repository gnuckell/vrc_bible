
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon;

public class SyncedScrollRect : UdonSharpBehaviour
{
	private const float SCROLL_ALPHA_SPEED = 5f;

    [SerializeField] private ScrollRect _scroll_rect;
    [SerializeField] private BibleReaderContent _bible_reader_content;

	[UdonSynced] private float _scroll_value_SYNC;

    void Update()
    {
		if (Networking.IsOwner(gameObject))
		{
			_scroll_value_SYNC = _scroll_rect.verticalNormalizedPosition;
		}
		else
		{
			_scroll_rect.verticalNormalizedPosition = Mathf.Lerp(_scroll_rect.verticalNormalizedPosition, _scroll_value_SYNC, SCROLL_ALPHA_SPEED * Time.deltaTime);
		}
    }

    public void OnScrollInitializePotentialDrag()
    {
		Networking.SetOwner(Networking.LocalPlayer, gameObject);
    }

	public void OnScrollValueChanged()
	{
        if (!Networking.IsOwner(gameObject)) return;

		if (_scroll_rect.verticalNormalizedPosition > 1f) OnScrollPastHead();
		else if (_scroll_rect.verticalNormalizedPosition < 0f) OnScrollPastTail();
	}

    public void OnScrollPastHead() {
        if (_bible_reader_content == null) return;
        _bible_reader_content.CreateContentAtHead();
        _bible_reader_content.head_chapter_SYNC = _bible_reader_content.content_head.chapter_index;
        _bible_reader_content.Sync();
    }

    public void OnScrollPastTail() {
        if (_bible_reader_content == null) return;
        _bible_reader_content.CreateContentAtTail();
        _bible_reader_content.tail_chapter_SYNC = _bible_reader_content.content_tail.chapter_index;
        _bible_reader_content.Sync();
    }
}
