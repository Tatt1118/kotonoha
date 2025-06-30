using UnityEngine;
using System.Collections.Generic;

public class PhoneInputModel
{
    private int _phase = 1;

    /// <summary>
    /// フェーズごとの電話番号→ストーリーIDマップ
    /// </summary>
    private Dictionary<int, Dictionary<string, string>> _phaseStoryMap = new()
    {
        [1] = new Dictionary<string, string>
        {
            {"2495", "1-2"},
            {"5750", "1-3"},
        },
        [2] = new Dictionary<string, string>
        {
            {"2495", "2-1"},
            {"5750", "2-2"},
        },
        [3] = new Dictionary<string, string>
        {
            {"2495", "3-1"},
            {"5750", "3-2"},
            {"0000", "3-3"},
        }
    };

    /// <summary>
    /// フェーズごとのストーリーIDの再生順（番号ではなくストーリーID）
    /// </summary>
    private Dictionary<int, List<string>> _phaseStoryOrder = new()
    {
        [1] = new List<string> { "1-2", "1-3" },
        [2] = new List<string> { "2-1", "2-2" },
        [3] = new List<string> { "3-1", "3-2", "3-3" },
    };

    /// <summary>
    /// 入力された番号が、現在のフェーズに存在するかどうか
    /// </summary>
    public bool NumberJudge(string input)
    {
        return _phaseStoryMap.TryGetValue(_phase, out var map) && map.ContainsKey(input);
    }

    /// <summary>
    /// 現在のフェーズで入力番号に対応するストーリーIDを取得
    /// </summary>
    public string GetStoryID(string inputNum)
    {
        return _phaseStoryMap.TryGetValue(_phase, out var map) && map.TryGetValue(inputNum, out var id) ? id : null;
    }

    /// <summary>
    /// 現在のフェーズのストーリーIDの順序リストを取得
    /// </summary>
    public List<string> GetStoryOrderInCurrentPhase()
    {
        return _phaseStoryOrder.TryGetValue(_phase, out var list) ? list : new List<string>();
    }

    /// <summary>
    /// 現在のフェーズに存在するストーリー数を取得
    /// </summary>
    public int GetStoryCountInCurrentPhase()
    {
        return _phaseStoryMap.TryGetValue(_phase, out var map) ? map.Count : 0;
    }

    /// <summary>
    /// フェーズを1つ進める
    /// </summary>
    public void AdvancePhase()
    {
        if (_phaseStoryMap.ContainsKey(_phase + 1))
        {
            _phase++;
        }
    }

    /// <summary>
    /// 現在のフェーズ番号を取得
    /// </summary>
    public int GetCurrentPhase()
    {
        return _phase;
    }
}
