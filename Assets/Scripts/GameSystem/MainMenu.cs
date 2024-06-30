using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Assets.Scripts.GameSystem
{
    public class MainMenu : MonoBehaviour
    {
        private const string Score = "Score";
        private const string BestScore = "BestScore";

        [SerializeField] private Button _play;
        [SerializeField] private Button _mute;
        [SerializeField] private TMP_Text _bestScoreCount;
        [SerializeField] private AudioSource _menuTheme;
        [SerializeField] private AudioSource _gameTheme;
        [SerializeField] private Sprite _onImage;
        [SerializeField] private Sprite _offImage;
        [SerializeField] private GameObject _menuWindow;
        [SerializeField] private BattleSystem _battleSystem;

        private bool _isOn = true;
        private int _score;
        private int _bestScore;

        private void Start()
        {
            if (PlayerPrefs.HasKey(Score))
            {
                _score = PlayerPrefs.GetInt(Score);
                _bestScore = PlayerPrefs.GetInt(BestScore);

                if (_bestScore < _score)
                {
                    _bestScore = _score;
                    PlayerPrefs.SetInt(BestScore, _bestScore);
                }
            }

            _bestScoreCount.text = _bestScore.ToString();
            _menuTheme.Play();
        }

        private void OnEnable()
        {
            _play.onClick.AddListener(EnterGame);
            _mute.onClick.AddListener(Mute);
        }       

        private void Mute()
        {
            if (!_isOn)
            {
                AudioListener.volume = 1f;
                _isOn = true;
                _mute.image.sprite = _onImage;
            }
            else if (_isOn)
            {
                AudioListener.volume = 0f;
                _isOn = false;
                _mute.image.sprite = _offImage;
            }
        }

        private void EnterGame()
        {
            _menuWindow.SetActive(false);
            _menuTheme.Stop();
            _gameTheme.Play();
            _battleSystem.Spawn();
        }
    }
}