﻿namespace GooglePlayGames.Native.PInvoke
{
    using GooglePlayGames.Native.Cwrapper;
    using System;
    using System.Runtime.InteropServices;

    internal class NativeLeaderboard : BaseReferenceHolder
    {
        internal NativeLeaderboard(IntPtr selfPtr) : base(selfPtr)
        {
        }

        protected override void CallDispose(HandleRef selfPointer)
        {
            Leaderboard.Leaderboard_Dispose(selfPointer);
        }

        internal static NativeLeaderboard FromPointer(IntPtr pointer)
        {
            if (pointer.Equals(IntPtr.Zero))
            {
                return null;
            }
            return new NativeLeaderboard(pointer);
        }

        internal string Title()
        {
            return PInvokeUtilities.OutParamsToString(delegate (StringBuilder out_string, UIntPtr out_size) {
                return Leaderboard.Leaderboard_Name(base.SelfPtr(), out_string, out_size);
            });
        }
    }
}

