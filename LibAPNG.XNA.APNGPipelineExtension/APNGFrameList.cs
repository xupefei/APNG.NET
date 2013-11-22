using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;

namespace LibAPNG.XNA.APNGPipelineExtension
{
    public struct Frame
    {
        public TextureContent Content;
        public TimeSpan DelayTime;
    }

    public struct APNGFrameList
    {
        private readonly List<Frame> _frames;
        public uint LoopCount;

        public APNGFrameList(uint loopCount)
        {
            LoopCount = loopCount;

            _frames = new List<Frame>();
        }

        public Frame[] Frames
        {
            get { return _frames.ToArray(); }
        }

        public void AddFrame(TimeSpan delayTime, TextureContent content)
        {
            _frames.Add(new Frame {DelayTime = delayTime, Content = content});
        }
    }
}