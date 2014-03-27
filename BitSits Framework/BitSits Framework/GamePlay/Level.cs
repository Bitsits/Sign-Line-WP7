using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using System.IO;
using GameDataLibrary;

namespace BitSits_Framework
{
    class Level : IDisposable
    {
        #region Fields

        public int Score { get; private set; }

        public bool IsLevelUp { get; private set; }
        public bool ReloadLevel { get; private set; }
        int levelIndex;

        GameContent gameContent;

        public int minScore, starScore;

        List<Block> blocks = new List<Block>();
        public int[] blockNumber = new int[GameContent.MaxGoals];

        List<List<Block>> lines = new List<List<Block>>();
        int lastLineIndex = -1;

        #endregion

        #region Initialization


        public Level(ScreenManager screenManager, int levelIndex)
        {
            this.gameContent = screenManager.GameContent;
            this.levelIndex = levelIndex;

            for (int i = 0; i < GameContent.MaxGoals; i++)
                blockNumber[i] = gameContent.random.Next(gameContent.blockCount[i]);

            LoadBlocks(levelIndex);
        }


        private void LoadBlocks(int levelIndex)
        {
            // Load the level and ensure all of the lines are the same length.
            int width;
            List<string> lines = new List<string>();
            LevelData ld = gameContent.content.Load<LevelData>("Levels/level" + levelIndex.ToString("00"));

            minScore = ld.MinScore;
            starScore = ld.StarScore;

            lines = ld.BlockData;

            width = lines[0].Length;
            // Loop over every tile position,
            for (int y = 0; y < lines.Count; ++y)
            {
                if (lines[y].Length != width)
                    throw new Exception(String.Format(
                        "The length of line {0} is different from all preceeding lines.", lines.Count));

                for (int x = 0; x < lines[0].Length; ++x)
                {
                    char c = lines[y][x];

                    if (c != '.')
                        blocks.Add(new Block(gameContent, (MDGoal)(c - '0'), blockNumber[(c - '0')],
                            new Vector2(x, y) * Tile.Width));
                }
            }
        }


        public void Dispose() { }


        #endregion

        #region Update and HandleInput


        public void Update(GameTime gameTime)
        {
            for (int i = 0; i < blocks.Count; i++)
            {
                blocks[i].IsColor = false; blocks[i].Score = 0;
            }

            Score = 0;
            for (int i = 0; i < lines.Count; i++)
            {
                for (int j = 0; j < lines[i].Count; j++)
                {
                    lines[i][j].IsColor = true;

                    if (j == 0 || j == lines[i].Count - 1)
                    {
                        lines[i][j].Score += 1; Score += 1;
                    }
                    else
                    {
                        lines[i][j].Score += 2; Score += 2;
                    }
                }
            }

            if (lastLineIndex == -1 && Score >= minScore)
            {
                IsLevelUp = true;
                for (int i = 0; i < blocks.Count; i++) IsLevelUp &= blocks[i].IsColor;
            }
        }


        public void HandleInput(InputState input, int playerIndex)
        {
            Vector2 mousePos = Vector2.Zero;

#if WINDOWS
            mousePos = new Vector2(input.CurrentMouseState.X, input.CurrentMouseState.Y); 
#endif
#if WINDOWS_PHONE
            if (input.TouchState.Count > 0) mousePos = input.TouchState[0].Position;
#endif

            mousePos /= Camera2D.PhoneScale;
            Point mousePoint = new Point((int)mousePos.X, (int)mousePos.Y);

            if (input.IsMouseLeftButtonClick())
            {
                for (int i = 0; i < blocks.Count; i++)
                {
                    if (blocks[i].Bounds.Contains(mousePoint))
                    {
                        // start a new line
                        if (!blocks[i].IsColor)
                        {
                            lines.Add(new List<Block>());
                            lastLineIndex = lines.Count - 1;
                            lines[lastLineIndex].Add(blocks[i]);

                            gameContent.blockSelect.Play();

                            break;
                        }
                        else
                        {
                            // break the line
                            for (int j = 0; j < lines.Count; j++)
                            {
                                if (lines[j].Contains(blocks[i]))
                                {
                                    int lastIndex = lines[j].LastIndexOf(blocks[i]);
                                    lines[j].RemoveRange(lastIndex + 1, lines[j].Count - lastIndex - 1);

                                    lastLineIndex = j;

                                    gameContent.blockSelect.Play();

                                    break;
                                }
                            }
                        }
                    }
                }
            }
#if WINDOWS
            else if (input.CurrentMouseState.LeftButton == ButtonState.Pressed && lastLineIndex != -1)
#endif
#if WINDOWS_PHONE
            //else if (TouchPanel.IsGestureAvailable && TouchPanel.ReadGesture().GestureType == GestureType.FreeDrag
            //  && lastLineIndex != -1)
            else if (input.TouchState.Count > 0 && input.TouchState[0].State == TouchLocationState.Moved
                && lastLineIndex != -1)
#endif
            {
                for (int i = 0; i < blocks.Count; i++)
                {
                    if (blocks[i] != CurrentBlock && blocks[i].Bounds.Contains(mousePoint))
                    {
                        // adding non colored blocks
                        if (!blocks[i].IsColor)
                        {
                            if (IsCorresponding(blocks[i], CurrentBlock)
                                && lines[lastLineIndex][0].Goal == blocks[i].Goal)
                            {
                                lines[lastLineIndex].Add(blocks[i]);

                                gameContent.blockSelect.Play();

                                break;
                            }
                            //else lastLineIndex = -1;
                        }
                        else
                        {
                            if (lines[lastLineIndex].Contains(blocks[i]))
                            {
                                // remove when drag back
                                if (blocks[i] == lines[lastLineIndex][lines[lastLineIndex].Count - 2])
                                {
                                    int blockLastIndex = lines[lastLineIndex].LastIndexOf(CurrentBlock);
                                    lines[lastLineIndex].RemoveAt(blockLastIndex);

                                    gameContent.blockSelect.Play();
                                }
                                else
                                {
                                    // for cross blocks
                                    int blockIndex = lines[lastLineIndex].IndexOf(blocks[i]);
                                    int blockLastIndex = lines[lastLineIndex].LastIndexOf(blocks[i]);

                                    if ((blockIndex == 0 ? true : CurrentBlock != lines[lastLineIndex][blockIndex - 1])
                                        && CurrentBlock != lines[lastLineIndex][blockIndex + 1]
                                        && (blockLastIndex == 0 ? true : CurrentBlock != lines[lastLineIndex][blockLastIndex - 1])
                                        && CurrentBlock != lines[lastLineIndex][blockLastIndex + 1])
                                    {
                                        if (IsCorresponding(blocks[i], CurrentBlock)
                                            && lines[lastLineIndex][0].Goal == blocks[i].Goal)
                                        {
                                            lines[lastLineIndex].Add(blocks[i]);

                                            gameContent.blockSelect.Play();

                                            break;
                                        }
                                        //else lastLineIndex = -1;
                                    }
                                }
                            }
                        }
                    }
                }

                /* add block both sides, line merging */
            }
#if WINDOWS
            if (input.LastMouseState.LeftButton == ButtonState.Pressed
                && input.CurrentMouseState.LeftButton == ButtonState.Released)
#endif
#if WINDOWS_PHONE
            if (input.TouchState.Count > 0 && input.TouchState[0].State == TouchLocationState.Released)
#endif
            {
                // Remove Single block lines
                if (lastLineIndex != -1 && lines[lastLineIndex].Count == 1)
                    lines.RemoveAt(lastLineIndex);

                lastLineIndex = -1;
            }
        }


        Block CurrentBlock
        {
            get
            {
                if (lastLineIndex == -1) return null;
                else
                    return lines[lastLineIndex][lines[lastLineIndex].Count - 1];
            }
        }


        bool IsCorresponding(Block blockA, Block blockB)
        {
            Point a = blockA.Bounds.Center, b = blockB.Bounds.Center;

            // at diagonal
            if (Math.Abs(a.X - b.X) == Tile.Width && Math.Abs(a.Y - b.Y) == Tile.Width)
                return false;

            // horizontal
            if (Math.Abs(a.X - b.X) == Tile.Width && a.Y == b.Y) return true;
            else
                // vertical
                if (a.X == b.X && Math.Abs(a.Y - b.Y) == Tile.Width) return true;

            return false;
        }


        #endregion

        #region Draw


        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(gameContent.levelBackground, Vector2.Zero, Color.White);

            for (int i = 0; i < blocks.Count; i++) blocks[i].Draw(spriteBatch);

            float lineAlpha = 0.2f;
            for (int i = 0; i < lines.Count; i++)
            {
                if (lines[i].Count >= 2)
                {
                    Point a = lines[i][1].Bounds.Center;
                    Vector2 pos = new Vector2(lines[i][0].Bounds.Center.X,
                            lines[i][0].Bounds.Center.Y);

                    float theta = (float)Math.Atan2(a.Y - pos.Y, a.X - pos.X);

                    spriteBatch.Draw(gameContent.lineStart, pos, null, Color.Black * lineAlpha,
                                 theta, gameContent.lineOrigin, 1, SpriteEffects.None, 1);

                    a = lines[i][lines[i].Count - 2].Bounds.Center;
                    pos = new Vector2(lines[i][lines[i].Count - 1].Bounds.Center.X,
                            lines[i][lines[i].Count - 1].Bounds.Center.Y);

                    theta = (float)Math.Atan2(pos.Y - a.Y, pos.X - a.X);

                    spriteBatch.Draw(gameContent.lineEnd, pos, null, Color.Black * lineAlpha,
                                 theta, gameContent.lineOrigin, 1, SpriteEffects.None, 1);
                }
                

                for (int j = 1; j < lines[i].Count - 1; j++)
                {
                    Point a = lines[i][j - 1].Bounds.Center, b = lines[i][j + 1].Bounds.Center;
                    Vector2 pos = new Vector2(lines[i][j].Bounds.Center.X,
                        lines[i][j].Bounds.Center.Y);

                    if (a.X == b.X || a.Y == b.Y)
                    {
                        float theta = (float)Math.Atan2(b.Y - a.Y, b.X - a.X);

                        spriteBatch.Draw(gameContent.lineOver, pos, null, Color.Black * lineAlpha,
                            theta, gameContent.lineOrigin, 1, SpriteEffects.None, 1);
                    }
                    else
                    {
                        float theta = (float)Math.Atan2((b.Y + a.Y) / 2 - pos.Y, (b.X + a.X) / 2 - pos.X);

                        if (theta < 0) theta += 2 * (float)Math.PI;

                        spriteBatch.Draw(gameContent.lineCorner, pos, null, Color.Black * lineAlpha,
                             theta - (float)Math.PI / 4, gameContent.lineOrigin, 1, SpriteEffects.None, 1);
                    }
                }
            }
        }


        #endregion
    }
}
