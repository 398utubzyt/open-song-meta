// This file is a part of the Open Song Meta project.
// Copyright (c) 398utubzyt and Open Song Meta contributors.
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

namespace Osm
{
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

#pragma warning disable IDE0290 // Use primary constructor
        public MetaFile(int version, string name, string? title, string? artist, string? album,
#pragma warning restore IDE0290 // Use primary constructor
            TimedTrack<Tempo> tempos, TimedTrack<TimeSignature> tsigs,  TimedTrack<KeySignature> ksigs)
        {
            _version = version;
            _name = name;
            _title = title;
            _artist = artist;
            _album = album;
            _tempos = tempos;
            _tsigs = tsigs;
            _ksigs = ksigs;
        }

        public readonly int ApiVersion => _version;
        public readonly string Name => _name;

        public readonly string? Title => _title;
        public readonly string? Artist => _artist;
        public readonly string? Album => _album;

        public readonly TimedTrack<Tempo>.ReadOnly Tempos => _tempos.AsReadOnly();
        public readonly TimedTrack<TimeSignature>.ReadOnly TimeSignatures => _tsigs.AsReadOnly();
        public readonly TimedTrack<KeySignature>.ReadOnly KeySignatures => _ksigs.AsReadOnly();

        public readonly Tempo InitialTempo => _tempos.FirstValue;
        public readonly TimeSignature InitialTimeSignature => _tsigs.FirstValue;
        public readonly KeySignature InitialKeySignature => _ksigs.FirstValue;
    }
}
