// This file is a part of the Open Song Meta project.
// Copyright (c) 398utubzyt and Open Song Meta contributors.
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Osm
{
    public enum WesternNote : byte
    {
        Unknown = 255,
        C = 0,
        Db,
        D,
        Eb,
        E,
        F,
        Gb,
        G,
        Ab,
        A,
        Bb,
        B,
    }

    public static partial class WesternUtils
    {
        private static readonly WesternNote[] _cof = [
                WesternNote.C, // 0
                WesternNote.G,
                WesternNote.D,
                WesternNote.A,
                WesternNote.E,
                WesternNote.B, // +5
                WesternNote.Gb, // -6
                WesternNote.Db,
                WesternNote.Ab,
                WesternNote.Eb,
                WesternNote.Bb,
                WesternNote.F, // -1
            ];

        public static WesternNote FromCircleOfFifths(int cw)
        {
            Debug.Assert(_cof.Length == 12);
            cw %= 12;
            if (cw < 0)
                cw += 12;
            return Unsafe.Add(ref MemoryMarshal.GetArrayDataReference(_cof), cw);
        }
    }

    public static class WesternNoteExtensions
    {
        private static readonly string[] _names =
        [
            "C", // Sharps
            "C#",
            "D",
            "D#",
            "E",
            "F",
            "F#",
            "G",
            "G#",
            "A",
            "A#",
            "B",
            "C", // Flats
            "Db",
            "D",
            "Eb",
            "E",
            "F",
            "Gb",
            "G",
            "Ab",
            "A",
            "Bb",
            "B"
        ];

        public static string GetName(this WesternNote note, bool preferFlat = true)
        {
            if (note == WesternNote.Unknown)
                return "Unknown";

            int index = (int)note;
            if (preferFlat)
                index += 12;
            return _names[index];
        }



        private static string ToString(WesternNote note, WesternMode mode, string unknown, bool preferFlat)
        {
            if (note == WesternNote.Unknown)
                return unknown;

            string noteName = GetName(note, preferFlat);
            string modeName = mode.GetName();

            string result;

            if (!string.IsNullOrWhiteSpace(modeName))
            {
                result = new('\0', noteName.Length + modeName.Length + 1);
                Span<char> r = MemoryMarshal.CreateSpan(ref Unsafe.AsRef(in result.GetPinnableReference()), result.Length);

                noteName.CopyTo(r);
                r[noteName.Length] = ' ';
                modeName.CopyTo(r[(noteName.Length + 1)..]);
            } else
                result = noteName;

            return result;
        }

        public static string KeyToString(this WesternNote note, WesternMode mode, bool preferFlat = true)
            => ToString(note, mode, "Unknown Key", preferFlat);
        public static string ModeToString(this WesternNote note, WesternMode mode, bool preferFlat = true)
            => ToString(note, mode, "Unknown Mode", preferFlat);
    }
}
