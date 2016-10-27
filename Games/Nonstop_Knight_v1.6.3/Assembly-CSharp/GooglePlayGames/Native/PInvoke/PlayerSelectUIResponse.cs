﻿namespace GooglePlayGames.Native.PInvoke
{
    using GooglePlayGames.Native.Cwrapper;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using System.Text;

    internal class PlayerSelectUIResponse : BaseReferenceHolder, IEnumerable, IEnumerable<string>
    {
        internal PlayerSelectUIResponse(IntPtr selfPointer) : base(selfPointer)
        {
        }

        protected override void CallDispose(HandleRef selfPointer)
        {
            TurnBasedMultiplayerManager.TurnBasedMultiplayerManager_PlayerSelectUIResponse_Dispose(selfPointer);
        }

        internal static PlayerSelectUIResponse FromPointer(IntPtr pointer)
        {
            if (PInvokeUtilities.IsNull(pointer))
            {
                return null;
            }
            return new PlayerSelectUIResponse(pointer);
        }

        public IEnumerator<string> GetEnumerator()
        {
            return PInvokeUtilities.ToEnumerator<string>(TurnBasedMultiplayerManager.TurnBasedMultiplayerManager_PlayerSelectUIResponse_GetPlayerIds_Length(base.SelfPtr()), new Func<UIntPtr, string>(this.PlayerIdAtIndex));
        }

        internal uint MaximumAutomatchingPlayers()
        {
            return TurnBasedMultiplayerManager.TurnBasedMultiplayerManager_PlayerSelectUIResponse_GetMaximumAutomatchingPlayers(base.SelfPtr());
        }

        internal uint MinimumAutomatchingPlayers()
        {
            return TurnBasedMultiplayerManager.TurnBasedMultiplayerManager_PlayerSelectUIResponse_GetMinimumAutomatchingPlayers(base.SelfPtr());
        }

        private string PlayerIdAtIndex(UIntPtr index)
        {
            <PlayerIdAtIndex>c__AnonStorey2E2 storeye = new <PlayerIdAtIndex>c__AnonStorey2E2();
            storeye.index = index;
            storeye.<>f__this = this;
            return PInvokeUtilities.OutParamsToString(new PInvokeUtilities.OutStringMethod(storeye.<>m__16F));
        }

        internal GooglePlayGames.Native.Cwrapper.CommonErrorStatus.UIStatus Status()
        {
            return TurnBasedMultiplayerManager.TurnBasedMultiplayerManager_PlayerSelectUIResponse_GetStatus(base.SelfPtr());
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        [CompilerGenerated]
        private sealed class <PlayerIdAtIndex>c__AnonStorey2E2
        {
            internal PlayerSelectUIResponse <>f__this;
            internal UIntPtr index;

            internal UIntPtr <>m__16F(StringBuilder out_string, UIntPtr size)
            {
                return TurnBasedMultiplayerManager.TurnBasedMultiplayerManager_PlayerSelectUIResponse_GetPlayerIds_GetElement(this.<>f__this.SelfPtr(), this.index, out_string, size);
            }
        }
    }
}

