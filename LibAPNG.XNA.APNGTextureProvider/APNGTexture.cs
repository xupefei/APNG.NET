using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace LibAPNG.XNA.APNGTextureProvider
{
    public class APNGTexture : IDisposable
    {
        private readonly List<Frame> _frames;
        private int _currentIndex;
        private int _playedCount;
        private TimeSpan _waitTime = TimeSpan.Zero;

        public APNGTexture(uint loopCount)
        {
            LoopCount = loopCount;

            _frames = new List<Frame>();
        }

        public bool IsDisposed { get; private set; }

        public uint LoopCount { get; private set; }

        public Texture2D CurrentFrame
        {
            get { return _frames[_currentIndex].Texture; }
        }

        public Frame[] Frames
        {
            get { return _frames.ToArray(); }
        }

        public void Dispose()
        {
            if (!IsDisposed)
            {
                IsDisposed = true;

                foreach (Frame frame in _frames)
                {
                    if (!frame.Texture.IsDisposed)
                        frame.Texture.Dispose();
                }
            }
        }

        public void Update(GameTime gameTime)
        {
            if (_frames.Count == 1)
            {
                _currentIndex = 0;
            }
            else
            {
                if (LoopCount != 0)
                {
                    if (_playedCount >= LoopCount)
                    {
                        _currentIndex = 0;
                    }
                    else
                    {
                        NextFrame(gameTime);
                    }
                }
                else
                {
                    NextFrame(gameTime);
                }
            }
        }

        private void NextFrame(GameTime gameTime)
        {
            if (_waitTime < _frames[_currentIndex].DelayTime)
            {
                _waitTime += gameTime.ElapsedGameTime;
                return;
            }

            _waitTime = TimeSpan.Zero;

            if (++_currentIndex == _frames.Count)
            {
                _currentIndex = 0;
                _playedCount++;
            }
        }

        internal void AddFrame(TimeSpan delayTime, Texture2D texture)
        {
            _frames.Add(new Frame {DelayTime = delayTime, Texture = texture});
        }

        ~APNGTexture()
        {
            Dispose();
        }
    }
}