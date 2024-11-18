// This file is a part of the Open Song Meta project.
// Copyright (c) 398utubzyt and Open Song Meta contributors.
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Runtime.CompilerServices;

namespace Opsm
{
    public enum BlockId : byte
    {
        None = 0,
        Title = 1,
        Artist = 2,
        Album = 3,
        Tempo = 4,
        TimeSignatures = 5,
        KeySignatures = 6,

        Reserved = 127,
        Dummy = 254,
        Extension = 255,
    }

    public readonly struct MetaFile
    {
        private readonly int _version;
        private readonly string _name;
        private readonly string? _title;
        private readonly string? _artist;
        private readonly string? _album;
        private readonly TimedTrack<Tempo> _tempos;
        private readonly TimedTrack<TimeSignature> _tsigs;
        private readonly TimedTrack<KeySignature> _ksigs;
        private readonly Dictionary<string, ExtensionBlock> _data;

#pragma warning disable IDE0290 // Use primary constructor
        public MetaFile(int version, string name, string? title, string? artist, string? album,
#pragma warning restore IDE0290 // Use primary constructor
            TimedTrack<Tempo> tempos, TimedTrack<TimeSignature> tsigs,  TimedTrack<KeySignature> ksigs,
            Dictionary<string, ExtensionBlock> data)
        {
            _version = version;
            _name = name;
            _title = title;
            _artist = artist;
            _album = album;
            _tempos = tempos;
            _tsigs = tsigs;
            _ksigs = ksigs;
            _data = data;
        }

        public readonly int ApiVersion => _version;
        public readonly string Name => _name;

        public readonly string? Title => _title;
        public readonly string? Artist => _artist;
        public readonly string? Album => _album;

        public readonly bool HasTempo => _tempos.Length > 0;
        public readonly TimedTrack<Tempo>.ReadOnly Tempos => _tempos.AsReadOnly();
        public readonly bool HasTimeSignature => _tsigs.Length > 0;
        public readonly TimedTrack<TimeSignature>.ReadOnly TimeSignatures => _tsigs.AsReadOnly();
        public readonly bool HasKeySignature => _ksigs.Length > 0;
        public readonly TimedTrack<KeySignature>.ReadOnly KeySignatures => _ksigs.AsReadOnly();

        public readonly Tempo InitialTempo => _tempos.FirstValue;
        public readonly TimeSignature InitialTimeSignature => _tsigs.FirstValue;
        public readonly KeySignature InitialKeySignature => _ksigs.FirstValue;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ExtensionBlock GetExtensionBlock(string name) => _data[name];
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool HasExtensionBlock(string name) => _data.ContainsKey(name);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryGetExtensionBlock(string name, out ExtensionBlock block) => _data.TryGetValue(name, out block);
        public bool TryCopyExtensionBlock(string name, Span<byte> destination)
        {
            bool result = _data.TryGetValue(name, out ExtensionBlock data);
            if (result)
            {
                data.CopyTo(destination);
            }
            return result;
        }
        public bool TryOpenExtensionBlock(string name, [MaybeNullWhen(false)] out Stream stream)
        {
            bool result = _data.TryGetValue(name, out ExtensionBlock data);
            stream = result ? data.Open() : null;
            return result;
        }
    }
}
