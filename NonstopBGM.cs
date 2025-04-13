using log4net;
using Microsoft.Xna.Framework;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using System.Reflection;
using Terraria;
using Terraria.ModLoader;

namespace NonstopBGM
{
	public class NonstopBGM : Mod
	{
        private record SessionState(bool HasFocus, float MusicVolume)
        {
            public bool PrevHasFocus { get; set; } = HasFocus;
            public float NormalMusicVolume { get; set; } = MusicVolume;
        };

		public static new ILog Logger { get; private set; }

        public static float VolumeRatio = 0.5f;

        private static SessionState State = null;

        public override void Load()
        {
			NonstopBGM.Logger = base.Logger;
            State = new SessionState(Main.hasFocus, Main.musicVolume);
            IL_Main.UpdateAudio += PatchGameFocus;
        }

		public override void Unload()
        {
            IL_Main.UpdateAudio -= PatchGameFocus;
            Main.musicVolume = State.NormalMusicVolume;
            State = null;
			NonstopBGM.Logger = null;
        }

		public static void PatchGameFocus(ILContext iL)
        {
			ILCursor cursor = new(iL);
            MethodInfo isActiveGetter = typeof(Game)
                    .GetProperty("IsActive")
                    .GetGetMethod();

            if (!cursor.TryGotoNext(MoveType.After,
            [
                x => x.MatchCall(isActiveGetter),
                x => x.MatchStloc(0),
            ]))
			{
				Logger.Error("Could not inject hook, no match found");
				return;
			}

            cursor.Index -= 1;
            cursor.Emit(OpCodes.Call, typeof(NonstopBGM).GetMethod("OnFocusUpdate"));
            cursor.Emit(OpCodes.Ldc_I4_1);
        }

        public static void OnFocusUpdate(bool hasFocus)
        {
            if (State == null || State.PrevHasFocus == hasFocus)
            {
                return;
            }
            State.PrevHasFocus = hasFocus;

            if (hasFocus)
            {
                Main.musicVolume = State.NormalMusicVolume;
            }
            else
            {
                State.NormalMusicVolume = Main.musicVolume;
                Main.musicVolume = State.NormalMusicVolume * VolumeRatio;
            }
        }
    }
}
