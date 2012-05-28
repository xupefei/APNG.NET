using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;

namespace APNG
{
    public class APNG
    {
        private MemoryStreamEx ms;

        private List<Frame> frames = new List<Frame>();
        private Frame defaultImage = new Frame();

        /// <summary>
        /// Indicate whether the file is a simple PNG.
        /// </summary>
        public bool IsSimplePNG { get; private set; }

        /// <summary>
        /// Indicate whether the default image is part of the animation
        /// </summary>
        public bool DefaultImageIsAnimeated { get; private set; }

        /// <summary>
        /// Gets the base image.
        /// If IsSimplePNG = True, returns the only image;
        /// if False, returns the default image
        /// </summary>
        public Frame DefaultImage
        {
            get { return defaultImage; }
        }

        /// <summary>
        /// Gets the frame array.
        /// If IsSimplePNG = True, returns empty
        /// </summary>
        public Frame[] Frames
        {
            get { return frames.ToArray(); }
        }

        /// <summary>
        /// Gets the IHDR Chunk
        /// </summary>
        public IHDRChunk IHDRChunk { get; private set; }

        /// <summary>
        /// Gets the acTL Chunk
        /// </summary>
        public acTLChunk acTLChunk { get; private set; }

        public APNG(string fileName)
            : this(File.ReadAllBytes(fileName))
        {
        }

        public APNG(byte[] fileBytes)
        {
            ms = new MemoryStreamEx(fileBytes);

            // check file signature.
            if (!Helper.IsBytesEqual(ms.ReadBytes(Frame.Signature.Length), Frame.Signature))
                throw new Exception("File signature incorrect.");

            // Read IHDR chunk.
            this.IHDRChunk = new IHDRChunk(ms);
            if (this.IHDRChunk.ChunkType != "IHDR")
                throw new Exception("IHDR chunk must located before any other chunks.");

            // Now let's loop in chunks
            Chunk chunk;
            Frame frame = null;
            bool isIDATAlreadyParsed = false;
            do
            {
                if (ms.Position == ms.Length)
                    throw new Exception("IEND chunk expected.");

                chunk = new Chunk(ms);

                switch (chunk.ChunkType)
                {
                    case "IHDR":
                        throw new Exception("Only single IHDR is allowed.");
                        break;

                    case "acTL":
                        if (this.IsSimplePNG)
                            throw new Exception("acTL chunk must located before any IDAT and fdAT");

                        this.acTLChunk = new acTLChunk(chunk);
                        break;

                    case "IDAT":
                        // To be an APNG, acTL must located before any IDAT and fdAT.
                        if (this.acTLChunk == null)
                            this.IsSimplePNG = true;

                        // Only default image has IDAT.
                        this.defaultImage.IHDRChunk = this.IHDRChunk;
                        this.defaultImage.AddIDATChunk(new IDATChunk(chunk));
                        isIDATAlreadyParsed = true;
                        break;

                    case "fcTL":
                        // Simple PNG should ignore this.
                        if (this.IsSimplePNG)
                            continue;

                        if (frame != null && frame.IDATChunks.Count == 0)
                            throw new Exception("One frame must have only one fcTL chunk.");

                        // IDAT already parsed means this fcTL is used by FRAME IMAGE.
                        if (isIDATAlreadyParsed)
                        {
                            // register current frame object and build a new frame object
                            // for next use
                            if (frame != null)
                                this.frames.Add(frame);
                            frame = new Frame
                                {
                                    IHDRChunk = this.IHDRChunk,
                                    fcTLChunk = new fcTLChunk(chunk)
                                };
                        }
                        // Otherwise this fcTL is used by the DEFAULT IMAGE.
                        else
                        {
                            this.defaultImage.fcTLChunk = new fcTLChunk(chunk);
                        }
                        break;
                    case "fdAT":
                        // Simple PNG should ignore this.
                        if (this.IsSimplePNG)
                            continue;

                        // fdAT is only used by frame image.
                        if (frame == null || frame.fcTLChunk == null)
                            throw new Exception("fcTL chunk expected.");

                        frame.AddIDATChunk(new fdATChunk(chunk).ToIDATChunk());
                        break;

                    case "IEND":
                        // register last frame object
                        if (frame != null)
                            this.frames.Add(frame);

                        if (this.DefaultImage.IDATChunks.Count != 0)
                            this.DefaultImage.IENDChunk = new IENDChunk(chunk);
                        foreach (var f in frames)
                        {
                            f.IENDChunk = new IENDChunk(chunk);
                        }
                        break;

                    default:
                        //TODO: Handle other chunks.
                        break;
                }
            }
            while (chunk.ChunkType != "IEND");

            // We have one more thing to do:
            // If the default image if part of the animation,
            // we should insert it into frames list.
            if (this.defaultImage.fcTLChunk != null)
            {
                this.frames.Insert(0, this.defaultImage);
                this.DefaultImageIsAnimeated = true;
            }
        }
    }
}