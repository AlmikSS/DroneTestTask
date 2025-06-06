using TMPro;
using UnityEngine;

public class ScoreShower : MonoBehaviour
{
    [SerializeField] private Team _team;
    [SerializeField] private Base _base;
    [SerializeField] private TMP_Text _text;

    private void LateUpdate()
    {
        if (_text == null || _base == null) return;
        
        _text.SetText($"{_team}: {_base.ResCount}");
    }
}