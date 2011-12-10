using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Audio;

namespace _2DActionGame
{
	public static class SoundControl
	{
		public static readonly float defVolume = .25f;
		public static Game1 game { get; private set; }
		public static ContentManager content { get; private set; }
		public static float volumeAll = defVolume;
		public static SoundEffect menuMusic, music;
		public static SoundEffectInstance menuMusicInstance, musicInstance;

		static SoundControl()
		{
		}
		public static void Initialize(Game1 game, ContentManager Content)
		{
			SoundControl.game = game;
			SoundControl.content = Content;
			menuMusic = content.Load<SoundEffect>("Audio\\BGM\\menu_new");
			menuMusicInstance = menuMusic.CreateInstance();

			menuMusicInstance.Volume = defVolume;
		}
		
		public static void IniMusic(string fileName)
		{
			music = content.Load<SoundEffect>(fileName);
			musicInstance = music.CreateInstance();
			musicInstance.Volume = defVolume;
		}

        public static void Play() { musicInstance.Volume = SoundControl.volumeAll; musicInstance.Play();}
		public static void Pause() { musicInstance.Pause(); }
		public static void Stop() { musicInstance.Stop(); }
		public static void Resume() { musicInstance.Resume(); }

	}
}
