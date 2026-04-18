using System.Threading;

using Cysharp.Threading.Tasks;

using UnityEngine;

namespace Sackrany.GameInput.Caches
{
    public class UIActionsCache : InputActionsCache
    {
        SackranyInputScheme.UIActions _actions;

        readonly ToggleableBool _submit;
        readonly ToggleableBool _cancel;
        readonly ToggleableBool _click;
        readonly ToggleableBool _rightClick;
        readonly ToggleableBool _middleClick;

        public bool Submit => _submit.Value;
        public bool SubmitJustPressed => _submit.JustPressed;
        public ToggleableBool SubmitMode => _submit;

        public bool Cancel => _cancel.Value;
        public bool CancelJustPressed => _cancel.JustPressed;
        public ToggleableBool CancelMode => _cancel;

        public bool Click => _click.Value;
        public bool ClickJustPressed => _click.JustPressed;
        public ToggleableBool ClickMode => _click;

        public bool RightClick => _rightClick.Value;
        public bool RightClickJustPressed => _rightClick.JustPressed;
        public ToggleableBool RightClickMode => _rightClick;

        public bool MiddleClick => _middleClick.Value;
        public bool MiddleClickJustPressed => _middleClick.JustPressed;
        public ToggleableBool MiddleClickMode => _middleClick;

        public UnityEngine.Vector2 Navigate { get; private set; }
        public UnityEngine.Vector2 Point { get; private set; }
        public UnityEngine.Vector2 ScrollWheel { get; private set; }
        public UnityEngine.Vector3 TrackedDevicePosition { get; private set; }
        public UnityEngine.Quaternion TrackedDeviceOrientation { get; private set; }

        public UIActionsCache(SackranyInputScheme input, CancellationToken token)
        {
            _actions = input.UI;

            _submit = Register(_actions.Submit);
            _cancel = Register(_actions.Cancel);
            _click = Register(_actions.Click);
            _rightClick = Register(_actions.RightClick);
            _middleClick = Register(_actions.MiddleClick);

            Update(token).Forget();
        }

        async UniTaskVoid Update(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                await UniTask.Yield(PlayerLoopTiming.LastUpdate, token);

                ResetAll();

                Navigate = _actions.Navigate.ReadValue<UnityEngine.Vector2>();
                Point = _actions.Point.ReadValue<UnityEngine.Vector2>();
                ScrollWheel = _actions.ScrollWheel.ReadValue<UnityEngine.Vector2>();
                TrackedDevicePosition = _actions.TrackedDevicePosition.ReadValue<UnityEngine.Vector3>();
                TrackedDeviceOrientation = _actions.TrackedDeviceOrientation.ReadValue<UnityEngine.Quaternion>();
            }
        }
    }
}
