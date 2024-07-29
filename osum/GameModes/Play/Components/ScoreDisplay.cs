﻿using System;
using System.Collections.Generic;
using System.Text;
using osu_common;
using osum.GameModes;
using osum.Graphics.Sprites;
using OpenTK;
using osum.Helpers;
using osum.Graphics.Skins;
using OpenTK.Graphics;

namespace osum.GameModes.Play.Components
{
    class ScoreDisplay : GameComponent
    {
        protected readonly pSpriteText s_Score;
        protected readonly pSpriteText s_Accuracy;
        private int displayScore;
        private double displayAccuracy;
        internal int currentScore;
        internal double currentAccuracy;
        internal Vector2 leftOfDisplay;
        protected Vector2 textMeasure;
        protected float scale;

        internal ScoreDisplay() : this(Vector2.Zero, true, 1, true, true)
        {
        }

        internal ScoreDisplay(Vector2 position, bool alignRight, float scale, bool showScore, bool showAccuracy)
        {
            this.spriteManager = spriteManager;

            this.scale = scale;

            float vpos = position.Y;

            textMeasure = Vector2.Zero;

            if (showScore)
            {
                s_Score =
                    new pSpriteText("00000000", "score",-2,
                        alignRight ? FieldTypes.StandardSnapRight : FieldTypes.Standard, alignRight ? OriginTypes.TopRight : OriginTypes.TopLeft, ClockTypes.Game,
                        new Vector2(0, 0), 0.95F, true, Color4.White);
                s_Score.TextConstantSpacing = true;
                textMeasure = s_Score.MeasureText() * 0.625f * scale;
                s_Score.Position = new Vector2(position.X, vpos);
                s_Score.ScaleScalar = scale;

                vpos += textMeasure.Y;
            }

            if (showAccuracy)
            {
                s_Accuracy =
                        new pSpriteText("00.00%", "score", -2,
                            alignRight ? FieldTypes.StandardSnapRight : FieldTypes.Standard, alignRight ? OriginTypes.TopRight : OriginTypes.TopLeft, ClockTypes.Game,
                            new Vector2(0, 0), 0.95F, true, Color4.White);
                s_Accuracy.ScaleScalar = scale * (showScore ? 0.6f : 1);
                s_Accuracy.TextConstantSpacing = true;
                s_Accuracy.Position = new Vector2(position.X, vpos);

                leftOfDisplay =
                s_Accuracy.Position +
                                new Vector2(s_Accuracy.MeasureText().X * 0.625f * s_Accuracy.ScaleScalar + 24,
                                    s_Accuracy.MeasureText().Y * 0.625f * s_Accuracy.ScaleScalar / 2);
            }

            spriteManager.Add(s_Score);
            spriteManager.Add(s_Accuracy);
        }

        internal void Draw()
        {
            //if (GameBase.SixtyFramesPerSecondFrame)
            {
                if (s_Accuracy != null)
                {
                    if (displayAccuracy - currentAccuracy <= -0.005)
                    {
                        displayAccuracy = Math.Round(displayAccuracy + Math.Max(0.01, (currentAccuracy - displayAccuracy) / 5), 2);
                        s_Accuracy.Text = String.Format("{0:00.00}%", displayAccuracy);
                    }
                    else if (displayAccuracy - currentAccuracy >= 0.005)
                    {
                        displayAccuracy = Math.Round(displayAccuracy - Math.Max(0.01, (displayAccuracy - currentAccuracy) / 5), 2);
                        s_Accuracy.Text = String.Format("{0:00.00}%", displayAccuracy);
                    }
                }

                if (s_Score != null)
                {
                    if (displayScore < currentScore)
                    {
                        displayScore += Math.Max(1, (currentScore - displayScore) / 10);
                        s_Score.Text = String.Format("{0:00000000}", displayScore);
                    }
                }
            }

            spriteManager.Draw();
        }

        internal virtual void Update(int score)
        {
            currentScore = score;

            spriteManager.Update();
        }

        internal void SetAccuracy(float accuracy)
        {
            currentAccuracy = Math.Round(accuracy,2);
        }

        internal void Hide()
        {
            if (s_Score != null)
                s_Score.FadeOut(0);
            if (s_Accuracy!=null)
                s_Accuracy.FadeOut(0);
            
        }

        internal void SetScore(int score)
        {
            currentScore = score;
        }
    }
}
