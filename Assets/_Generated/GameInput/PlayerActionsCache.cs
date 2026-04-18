using System.Threading;

using Cysharp.Threading.Tasks;

using UnityEngine;

namespace Sackrany.GameInput.Caches
{
    public class PlayerActionsCache : InputActionsCache
    {
        SackranyInputScheme.PlayerActions _actions;

        readonly ToggleableBool _attack;
        readonly ToggleableBool _interact;
        readonly ToggleableBool _crouch;
        readonly ToggleableBool _jump;
        readonly ToggleableBool _previous;
        readonly ToggleableBool _next;
        readonly ToggleableBool _sprint;
        readonly ToggleableBool _inventory;

        public bool Attack => _attack.Value;
        public bool AttackJustPressed => _attack.JustPressed;
        public ToggleableBool AttackMode => _attack;

        public bool Interact => _interact.Value;
        public bool InteractJustPressed => _interact.JustPressed;
        public ToggleableBool InteractMode => _interact;

        public bool Crouch => _crouch.Value;
        public bool CrouchJustPressed => _crouch.JustPressed;
        public ToggleableBool CrouchMode => _crouch;

        public bool Jump => _jump.Value;
        public bool JumpJustPressed => _jump.JustPressed;
        public ToggleableBool JumpMode => _jump;

        public bool Previous => _previous.Value;
        public bool PreviousJustPressed => _previous.JustPressed;
        public ToggleableBool PreviousMode => _previous;

        public bool Next => _next.Value;
        public bool NextJustPressed => _next.JustPressed;
        public ToggleableBool NextMode => _next;

        public bool Sprint => _sprint.Value;
        public bool SprintJustPressed => _sprint.JustPressed;
        public ToggleableBool SprintMode => _sprint;

        public bool Inventory => _inventory.Value;
        public bool InventoryJustPressed => _inventory.JustPressed;
        public ToggleableBool InventoryMode => _inventory;

        public UnityEngine.Vector2 Move { get; private set; }
        public UnityEngine.Vector2 Look { get; private set; }

        public PlayerActionsCache(SackranyInputScheme input, CancellationToken token)
        {
            _actions = input.Player;

            _attack = Register(_actions.Attack);
            _interact = Register(_actions.Interact);
            _crouch = Register(_actions.Crouch);
            _jump = Register(_actions.Jump);
            _previous = Register(_actions.Previous);
            _next = Register(_actions.Next);
            _sprint = Register(_actions.Sprint);
            _inventory = Register(_actions.Inventory);

            Update(token).Forget();
        }

        async UniTaskVoid Update(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                await UniTask.Yield(PlayerLoopTiming.LastUpdate, token);

                ResetAll();

                Move = _actions.Move.ReadValue<UnityEngine.Vector2>();
                Look = _actions.Look.ReadValue<UnityEngine.Vector2>();
            }
        }
    }
}
