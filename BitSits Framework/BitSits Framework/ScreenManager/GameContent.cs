/*
 * Copyright (c) 2011 BitSits Games
 *  
 * Shubhajit Saha    http://bitsits.blogspot.com/
 * Maya Agarwal      http://bitsitsgames.blogspot.com/
 * 
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */

using System;
using System.Threading;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using GameDataLibrary;

namespace BitSits_Framework
{
    /// <summary>
    /// All the Contents of the Game is loaded and stored here
    /// so that all other screen can copy from here
    /// </summary>
    public class GameContent
    {
        public ContentManager content;

        //GenerateData g = new GenerateData();
        
        public Random random = new Random();

        public readonly float b2Scale = 30;

        // Textures
        public Texture2D blank, gradient;
        public Texture2D menuBackground, mainMenuTitle, credits, levelBackground, pause, levelUp, gameOver;

        public static readonly int MaxGoals = 8;
        public Texture2D[] goalSymbols = new Texture2D[MaxGoals];

        /// <summary>
        /// [goal][number]
        /// </summary>
        public Texture2D[][] goalBlocks = new Texture2D[MaxGoals][];
        public List<int> blockCount;

        public Texture2D lineEnd, lineStart, lineOver, lineCorner;
        public Vector2 lineOrigin;

        public Texture2D star;

        // Fonts
        public SpriteFont debugFont, gameFont;
        public int gameFontSize;

        public SoundEffect menuSelect, blockSelect;

        // Audio objects
        //public AudioEngine audioEngine;
        //public SoundBank soundBank;
        //public WaveBank waveBank;
        

        /// <summary>
        /// Load GameContents
        /// </summary>
        public GameContent(GameComponent screenManager)
        {
            content = screenManager.Game.Content;

            blank = content.Load<Texture2D>("Graphics/blank");
            gradient = content.Load<Texture2D>("Graphics/gradient");
            menuBackground = content.Load<Texture2D>("Graphics/menuBackground");
            credits = content.Load<Texture2D>("Graphics/credits");
            levelUp = content.Load<Texture2D>("Graphics/levelUp");
            gameOver = content.Load<Texture2D>("Graphics/gameOver");

            pause = content.Load<Texture2D>("Graphics/pause");

            levelBackground = content.Load<Texture2D>("Graphics/levelBackground");

            mainMenuTitle = content.Load<Texture2D>("Graphics/mainMenuTitle");

            blockCount = content.Load<List<int>>("Levels/blockCount");
            for (int i = 0; i < MaxGoals; i++)
            {
                goalBlocks[i] = new Texture2D[blockCount[i]];

                for (int j = 0; j < blockCount[i]; j++)
                    goalBlocks[i][j] = content.Load<Texture2D>("Graphics/block" + i + "_" + j);
            }

            lineEnd = content.Load<Texture2D>("Graphics/lineEnd");
            lineStart = content.Load<Texture2D>("Graphics/lineStart");
            lineOver = content.Load<Texture2D>("Graphics/lineOver");
            lineCorner = content.Load<Texture2D>("Graphics/lineCorner");

            lineOrigin = new Vector2(lineEnd.Width, lineEnd.Height) / 2;

            star = content.Load<Texture2D>("Graphics/star");

            debugFont = content.Load<SpriteFont>("Fonts/debugFont");

            gameFontSize = 72;
            gameFont = content.Load<SpriteFont>("Fonts/realmadrid2009_" + gameFontSize.ToString());
            gameFont.Spacing = 5;

            menuSelect = content.Load<SoundEffect>("Audio/menuSelect");
            blockSelect = content.Load<SoundEffect>("Audio/blockSelect");

            Song s = content.Load<Song>("Audio/PATROUX - LITTLE STORY");
            MediaPlayer.Play(s);

            MediaPlayer.IsRepeating = true;

#if DEBUG && WINDOWS
            MediaPlayer.Volume = .3f; SoundEffect.MasterVolume = .3f;
#else
            MediaPlayer.Volume = .5f; SoundEffect.MasterVolume = .5f;
#endif

            // Initialize audio objects.
            //audioEngine = new AudioEngine("Content/Audio/Audio.xgs");
            //soundBank = new SoundBank(audioEngine, "Content/Audio/Sound Bank.xsb");
            //waveBank = new WaveBank(audioEngine, "Content/Audio/Wave Bank.xwb");

            //soundBank.GetCue("music").Play();

#if !(DEBUG && WINDOWS)
            Thread.Sleep(1000);
#endif

            // once the load has finished, we use ResetElapsedTime to tell the game's
            // timing mechanism that we have just finished a very long frame, and that
            // it should not try to catch up.
            screenManager.Game.ResetElapsedTime();
        }


        /// <summary>
        /// Unload GameContents
        /// </summary>
        public void UnloadContent() { content.Unload(); }
    }
}
