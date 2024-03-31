
using TMPro;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class ReaderContentPanelBehaviour : UdonSharpBehaviour
{
	public const float DESTROY_MARGIN_WORLD_SPACE = 5f;

	[SerializeField] public TextMeshProUGUI chapter_text;
	[SerializeField] public TextMeshProUGUI content_text;

	private RectTransform rect;
	private RectTransform parent;
	private readonly Vector3[] corners_a = new Vector3[4];
	private readonly Vector3[] corners_b = new Vector3[4];

	public int book_index = 0;
	public int chapter_index = 0;

	public void Init(string text, int book, int chapter)
	{
		book_index = book;
		chapter_index = chapter;

		chapter_text.text = $"Chapter {chapter + 1}";
		content_text.text = text;
	}

	void Start()
	{
		rect = (RectTransform)transform;
		parent = (RectTransform)rect.parent.parent; // Gets the viewport transform
	}

	// void Update()
	// {
	// 	CheckCorners();
	// }

	// void CheckCorners()
	// {
	// 	rect.GetWorldCorners(corners_a);
	// 	parent.GetWorldCorners(corners_b);

	// 	if (corners_a[0].y <= corners_b[1].y + DESTROY_MARGIN_WORLD_SPACE && corners_a[1].y >= corners_b[0].y - DESTROY_MARGIN_WORLD_SPACE) return;

	// 	// Destroy(gameObject);
	// }
}
