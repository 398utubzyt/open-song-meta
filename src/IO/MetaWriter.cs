// This file is a part of the Open Song Meta project.
// Copyright (c) 398utubzyt and Open Song Meta contributors.
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System;
using System.IO;

namespace Opsm.IO
{
    public sealed class MetaWriter : IDisposable
    {
        public const int CurrentVersion = 1;
        public const int Magic = 0x4D53504F;
        public const int Alignment = 16;

        private readonly BinaryWriter _writer;
        private readonly bool _keepAlive;

        public BinaryWriter Writer => _writer;
        public Stream Stream => _writer.BaseStream;

        public MetaWriter(Stream stream, bool keepAlive = false)
        {
            _writer = new BinaryWriter(stream, System.Text.Encoding.UTF8, keepAlive);
            _keepAlive = false;
        }

        public MetaWriter(BinaryWriter writer, bool keepAlive = false)
        {
            _writer = writer;
            _keepAlive = keepAlive;
        }

        public void Write(Tempo tempo)
        {
            _writer.Write(tempo.BeatsPerMinute);
        }

        public void Write(TimeSignature timeSignature)
        {
            _writer.Write(timeSignature.Numerator);
            _writer.Write(timeSignature.Denominator);
        }

        public void Write(KeySignature keySignature)
        {
            _writer.Write((byte)keySignature.Root);
            _writer.Write((byte)keySignature.Mode);
        }

        public void Write(TimedValue<Tempo> tempo)
        {
            _writer.Write(tempo.Time);
            Write(tempo.Value);
        }

        public void Write(TimedValue<TimeSignature> timeSignature)
        {
            _writer.Write(timeSignature.Time);
            Write(timeSignature.Value);
        }

        public void Write(TimedValue<KeySignature> keySignature)
        {
            _writer.Write(keySignature.Time);
            Write(keySignature.Value);
        }

        public void Write(TimedTrack<Tempo>.ReadOnly tempo)
        {
            _writer.Write((short)tempo.Length);
            foreach (var t in tempo)
                Write(t);
        }

        public void Write(TimedTrack<TimeSignature>.ReadOnly timeSignature)
        {
            _writer.Write((short)timeSignature.Length);
            foreach (var t in timeSignature)
                Write(t);
        }

        public void Write(TimedTrack<KeySignature>.ReadOnly keySignature)
        {
            _writer.Write((short)keySignature.Length);
            foreach (var k in keySignature)
                Write(k);
        }

        public void Write(in MetaFile meta)
        {
            _writer.Write(Magic);
            _writer.Write(CurrentVersion);

            _writer.Write(meta.Name);

            _writer.WriteAlignDummy(Alignment);
            
            _writer.WriteOptional(BlockId.Title, meta.Title);
            _writer.WriteAlignDummy(Alignment);
            _writer.WriteOptional(BlockId.Artist, meta.Artist);
            _writer.WriteAlignDummy(Alignment);
            _writer.WriteOptional(BlockId.Album, meta.Album);
            _writer.WriteAlignDummy(Alignment);

            if (meta.HasTempo)
            {
                _writer.Write(BlockId.Tempo);
                Write(meta.Tempos);
                _writer.WriteAlignDummy(Alignment);
            }

            if (meta.HasTimeSignature)
            {
                _writer.Write(BlockId.TimeSignatures);
                Write(meta.TimeSignatures);
                _writer.WriteAlignDummy(Alignment);
            }

            if (meta.HasKeySignature)
            {
                _writer.Write(BlockId.KeySignatures);
                Write(meta.KeySignatures);
                _writer.WriteAlignDummy(Alignment);
            }

            // Signifies the end of the format.
            _writer.Write(BlockId.None);
            if (_writer.WriteAlignDummy(Alignment))
                _writer.Write(BlockId.Dummy);
        }

#pragma warning disable IDE0060 // Remove unused parameter
        private void Free(bool manual)
#pragma warning restore IDE0060 // Remove unused parameter
        {
            if (_keepAlive)
                _writer.Flush();
            else
                _writer.Close();
        }

        public void Dispose()
        {
            Free(true);
            GC.SuppressFinalize(this);
        }

        ~MetaWriter()
        {
            Free(false);
        }

        public static bool Export(string path, in MetaFile file)
        {
            bool result;
            try
            {
                using MetaWriter writer = new(new FileStream(path, FileMode.Create, FileAccess.Write));
                writer.Write(file);
                result = true;
            } catch
            {
                result = false;
            }
            return result;
        }
    }
}
