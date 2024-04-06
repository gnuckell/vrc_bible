
using UnityEngine;

internal static class BibleUtils
{
	internal static int GetChildIndex(this Transform a, Transform b)
	{
		for (int i = 0; i < a.childCount; i++)
			if (a.GetChild(i) == b) return i;
		return -1;
	}

	internal static int NthIndexOf(in string sample, char c, int n, int startIndex = 0)
	{
		if (n < 0) return 0;
		var result = startIndex;
		for (var i = 0; i <= n; i++)
			result = sample.IndexOf(c, result) + 1;
		return result;
	}

	internal static int CountOfChar(in string sample, char c, int end)
	{
		var result = 0;
		var start_index = 0;
		while (start_index < end)
		{
			start_index = sample.IndexOf(c, start_index) + 1;
			result++;
		}
		return result;
	}
}