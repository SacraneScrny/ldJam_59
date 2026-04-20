using System;
using System.Threading;

using Cysharp.Threading.Tasks;

using Game.Logic.Level;

using R3;

using Sackrany.Extensions;

using SackranyUI.Core.Base;
using SackranyUI.Core.Entities;

using UnityEngine;

namespace Game.Logic.UI.Screen
{
    public class GameResultViewModel : ViewModel<GameResult>
    {
        [Bind("alpha")] public ReactiveProperty<float> _opacity = new ();
        [Bind("resultText")] public ReactiveProperty<string> _resultText = new ();
        [Bind("buttonText")] public ReactiveProperty<string> _buttonText = new ();
        CancellationTokenSource _cts = new();
        
        protected override void OnInitialized()
        {
            GameLevelManager.OnLevelFinished += OnLevelFinished;
        }
        void OnLevelFinished(bool isLose)
        {
            ShowWindow(isLose, _cts.Token).Forget();
        }

        async UniTask ShowWindow(bool isLose, CancellationToken token)
        {
            try {
                await UniTask.Delay(TimeSpan.FromSeconds(Template.Delay), cancellationToken: token);
                _resultText.Value = isLose ? "You lose, battery is out.." : "You Won!";
                _buttonText.Value = isLose ? "Restart" : "Next Level";
                _opacity.Value = 0;
                Open();

                float t = 0;
                while (t < 1 && !token.IsCancellationRequested)
                {
                    _opacity.Value = t;
                    t += Time.deltaTime * 2f;
                    await UniTask.Yield(token);
                }
                _opacity.Value = 1;
            }
            catch (Exception _) {}
        }

        [Bind("click")]
        public void OnClick()
        {
            GameLevelManager.NextLevel();
            Close();
        }
    }

    [Serializable]
    public class GameResult : ViewModelTemplate<GameResultViewModel>
    {
        public float Delay = 1;
    }
}