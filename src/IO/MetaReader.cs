// This file is a part of the Open Song Meta project.
// Copyright (c) 398utubzyt and Open Song Meta contributors.
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using Opsm.Importers;

using System;
using System.Collections.Generic;
using System.IO;

namespace Opsm.IO
{
    public sealed class MetaReader : IDisposable
    {
        public const int CurrentVersion = 1;
        public const int Magic = 0x4D53504F;

        private readonly BinaryReader _reader;
        private readonly bool _keepAlive;

        public BinaryReader Reader => _reader;
        public Stream Stream => _reader.BaseStream;

        public MetaReader(Stream stream, bool keepAlive = false)
        {
            _reader = new BinaryReader(stream, System.Text.Encoding.UTF8, keepAlive);
            _keepAlive = false;
        }

        public MetaReader(BinaryReader reader, bool keepAlive = false)
        {
            _reader = reader;
            _keepAlive = keepAlive;
        }

        public Tempo ReadTempo()
        {
            return new(_reader.ReadDouble());
        }

        public TimeSignature ReadTimeSignature()
        {
            return new(_reader.ReadInt16(), _reader.ReadInt16());
        }

        public KeySignature ReadKeySignature()
        {
            return new((WesternNote)_reader.ReadByte(), (WesternMode)_reader.ReadByte());
        }

        public TimedValue<Tempo> ReadTimedTempo()
        {
            return new(_reader.ReadDouble(), ReadTempo());
        }

        public TimedValue<TimeSignature> ReadTimedTimeSignature()
        {
            return new(_reader.ReadDouble(), ReadTimeSignature());
        }

        public TimedValue<KeySignature> ReadTimedKeySignature()
        {
            return new(_reader.ReadDouble(), ReadKeySignature());
        }

        private static void ThrowIfNotEqual(int a, int b)
        {
            if (a != b)
                throw new IOException();
        }

        public TimedTrack<Tempo> ReadTempoTrack()
        {
            TimedTrack<Tempo> result = new(_reader.ReadInt16());

            int i;
            for (i = 0; i < result.Length; ++i)
                result.Append(ReadTimedTempo());

            ThrowIfNotEqual(result.Length, i);
            return result;
        }

        public TimedTrack<TimeSignature> ReadTimeSignatureTrack()
        {
            TimedTrack<TimeSignature> result = new(_reader.ReadInt16());

            int i;
            for (i = 0; i < result.Length; ++i)
                result.Append(ReadTimedTimeSignature());

            ThrowIfNotEqual(result.Length, i);
            return result;
        }

        public TimedTrack<KeySignature> ReadKeySignatureTrack()
        {
            TimedTrack<KeySignature> result = new(_reader.ReadInt16());

            int i;
            for (i = 0; i < result.Length; ++i)
                result.Append(ReadTimedKeySignature());

            ThrowIfNotEqual(result.Length, i);
            return result;
        }

        private static void ThrowIfLessThan(int a, int b)
        {
            if (a < b)
                throw new IOException();
        }

        public MetaFile ReadMetaFile()
        {
            ThrowIfNotEqual(_reader.ReadInt32(), Magic);

            int version = _reader.ReadInt32();
            ThrowIfLessThan(version, 1);

            // Temporary since there's only one version.
            ThrowIfNotEqual(version, CurrentVersion);

            string name = _reader.ReadString();

            string? title = null;
            string? artist = null;
            string? album = null;
            TimedTrack<Tempo> tempos = null!;
            TimedTrack<TimeSignature> timeSignatures = null!;
            TimedTrack<KeySignature> keySignatures = null!;
            Dictionary<string, ExtensionBlock> blocks = [];

            while (_reader.TryReadBlockId(out BlockId id))
            {
                switch (id)
                {
                    case BlockId.Title:
                        title = _reader.ReadString();
                        break;
                    case BlockId.Artist:
                        artist = _reader.ReadString();
                        break;
                    case BlockId.Album:
                        album = _reader.ReadString();
                        break;
                    case BlockId.Tempo:
                        tempos = ReadTempoTrack();
                        break;
                    case BlockId.TimeSignatures:
                        timeSignatures = ReadTimeSignatureTrack();
                        break;
                    case BlockId.KeySignatures:
                        keySignatures = ReadKeySignatureTrack();
                        break;
                    case BlockId.Extension:
                        string bname = _reader.ReadString();
                        blocks.Add(bname, new(bname, _reader.ReadBytes(_reader.Read7BitEncodedInt())));
                        break;
                }
            }

            tempos ??= new(0);
            timeSignatures ??= new(0);
            keySignatures ??= new(0);

            return new(version, name, title, artist, album, tempos, timeSignatures, keySignatures, blocks);
        }

        public bool ImportMetaFile(IMetaImporter importer, out MetaFile meta)
        {
            bool result = importer.Populate(this);

            if (result)
                result = importer.Import(out meta);
            else
                meta = default;

            return result;
        }

#pragma warning disable IDE0060 // Remove unused parameter
        private void Free(bool manual)
#pragma warning restore IDE0060 // Remove unused parameter
        {
            if (!_keepAlive)
                _reader.Close();
        }

        public void Dispose()
        {
            Free(true);
            GC.SuppressFinalize(this);
        }

        ~MetaReader()
        {
            Free(false);
        }

        public static bool Import(string path, out MetaFile file, IMetaImporter? importer = null)
        {
            bool result;
            file = default;

            result = File.Exists(path);
            if (result)
            {
                if (importer == null)
                {
                    try
                    {
                        using MetaReader reader = new(new FileStream(path, FileMode.Open, FileAccess.Read));
                        file = reader.ReadMetaFile();
                    } catch
                    {
                        result = false;
                    }
                } else
                {
                    result = importer.ValidatePath(path);
                    if (result)
                    {
                        MetaReader reader;
                        using (reader = new(new FileStream(path, FileMode.Open, FileAccess.Read)))
                        {
                            result = importer.Populate(reader);
                        }

                        if (result)
                        {
                            result = importer.Import(out file);
                        }
                    }
                }
            }

            return result;
        }
    }
}
