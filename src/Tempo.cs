// This file is a part of the Open Song Meta project.
// Copyright (c) 398utubzyt and Open Song Meta contributors.
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

namespace Osm
{
    public readonly struct Tempo(double bpm)
    {
        public readonly double BeatsPerMinute = bpm;
        public readonly double MinutesPerBeat => 60.0 / BeatsPerMinute;

        public override string ToString() => $"{BeatsPerMinute} BPM";
    }
}
