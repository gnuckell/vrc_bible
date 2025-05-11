
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon;

public class BibleReader : UdonSharpBehaviour
{
	[SerializeField] private Setting_FontFamily setting_font_family;
	[SerializeField] public GameObject pref_content;
    [SerializeField] private BibleHost _host;
	public BibleHost host => _host;
    [SerializeField] private ScrollRect _scroll_rect;

	private bool prox_is_awaiting_scroll_head;
	private float prox_preheight_scroll_head;

	[HideInInspector] public ChapterContent content_head;
	[HideInInspector] public ChapterContent content_tail;

	private ChapterContent _content_focused;
	private ChapterContent content_focused
	{
		get => _content_focused;
		set
		{
			_content_focused = value;
			host.chapter_index = _content_focused.chapter_index;
		}
	}

    [UdonSynced] [HideInInspector] public int head_chapter_SYNC = 0;
    [UdonSynced] [HideInInspector] public int tail_chapter_SYNC = 0;

    public void OnEnable()
    {
        // ResetContent();
        // Sync();
    }

    public void Init(GameObject fontOverride)
    {
		pref_content = fontOverride != null ? fontOverride : setting_font_family.current_font;

        ResetContent();
    }

	public void Despawn()
	{
		ResetContent();
	}

    void Update()
    {
		if (prox_is_awaiting_scroll_head)
			OnScrollPastHeadActually();

		var focus = CalculateFocusedChild();
		if (focus != content_focused) content_focused = focus;
    }

    public override void OnDeserialization()
    {
		if (head_chapter_SYNC <= content_head.chapter_index &&
			tail_chapter_SYNC >= content_tail.chapter_index)
		{
			SyncExpandHead();
			SyncExpandTail();
		}
		else // RESET
		{
			Clear();

			content_head = CreateChapterContent(head_chapter_SYNC);
			content_tail = content_head;

			SyncExpandTail();

			content_focused = content_head;
		}
    }

    public void Sync()
    {
        Networking.SetOwner(Networking.LocalPlayer, gameObject);
		head_chapter_SYNC = content_head.chapter_index;
		tail_chapter_SYNC = content_tail.chapter_index;
        RequestSerialization();
    }

	private void Clear()
	{
		var length = transform.childCount;
		for (var i = 0; i < length; i++)
			Destroy(transform.GetChild(i).gameObject);
	}

	public void ResetContent() => ResetContent(host.chapter_index);
	public void ResetContent(int chapt)
	{
		Clear();

		content_head = CreateChapterContent(chapt);
		content_tail = content_head;

		_scroll_rect.verticalNormalizedPosition = 1f;
		content_focused = content_head;

		if (Networking.IsOwner(gameObject)) Sync();
	}

    public void CreateContentAtHead()
	{
		var content = CreateChapterContent(content_head.chapter_index - 1);
		if (content == null) return;

		if (!prox_is_awaiting_scroll_head)
        {
		    _scroll_rect.enabled = false;
		    prox_preheight_scroll_head = GetCalculatedContentHeight();
        }
		prox_is_awaiting_scroll_head = true;

		content.transform.SetAsFirstSibling();
		content_head = content;
	}

	private void OnScrollPastHeadActually()
	{
		var post_height = GetCalculatedContentHeight();

		var scroll_y = prox_preheight_scroll_head / post_height;
		_scroll_rect.verticalNormalizedPosition = scroll_y;
		_scroll_rect.enabled = true;
		prox_is_awaiting_scroll_head = false;
	}

	public void CreateContentAtTail()
	{
		var content = CreateChapterContent(content_tail.chapter_index + 1);
		if (content == null) return;

		content.transform.SetAsLastSibling();
		content_tail = content;
	}

	private void SyncExpandHead()
	{
        while (content_head.chapter_index != head_chapter_SYNC)
        {
			CreateContentAtHead();
		}
	}

	private void SyncExpandTail()
	{
        while (content_tail.chapter_index != tail_chapter_SYNC)
        {
			CreateContentAtTail();
		}
	}


	private float GetCalculatedContentHeight()
	{
		return _scroll_rect.content.rect.height;
	}

	private ChapterContent CalculateFocusedChild()
	{
		for (int i = _scroll_rect.content.childCount - 1; i >= 0; i--)
		{
			var iChild = (RectTransform)_scroll_rect.content.GetChild(i);
			var iPosition = 1f + (iChild.anchoredPosition.y / _scroll_rect.content.rect.height);
			if (_scroll_rect.verticalNormalizedPosition < iPosition)
				return iChild.GetComponent<ChapterContent>();
		}
		return content_focused;
	}

	private ChapterContent CreateChapterContent(int chapter)
	{
		if (chapter < 0 || chapter >= host.max_chapter_count) return null;

		var obj = Instantiate(pref_content, transform);
		var panel = obj.GetComponent<ChapterContent>();

		panel.Init(this, chapter);
		return panel;
	}
}
