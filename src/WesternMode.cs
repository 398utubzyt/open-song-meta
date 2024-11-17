// This file is a part of the Open Song Meta project.
// Copyright (c) 398utubzyt and Open Song Meta contributors.
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System;

namespace Opsm
{
    public enum WesternMode : byte
    {
        Unknown,
        Ionian,
        Dorian,
        Phrygian,
        Lydian,
        Mixolydian,
        Aeolian,
        Locrian,

        Major = Ionian,
        Minor = Aeolian,
    }

    public static partial class WesternUtils
    {
        public static WesternMode ParseMode(string input)
        {
            return input.ToLowerInvariant() switch
            {
                "ionian" => WesternMode.Ionian,
                "dorian" => WesternMode.Dorian,
                "phrygian" => WesternMode.Phrygian,
                "lydian" => WesternMode.Lydian,
                "mixolydian" => WesternMode.Mixolydian,
                "aeolian" => WesternMode.Aeolian,
                "locrian" => WesternMode.Locrian,
                "major" => WesternMode.Major,
                "minor" => WesternMode.Minor,
                _ => WesternMode.Unknown,
            };
        }
    }

    public static class WesternModeExtensions
    {
        private static readonly string[] _names = [
            string.Empty,
            "Major",
            "Dorian",
            "Phyrgian",
            "Lydian",
            "Mixolydian",
            "Minor",
            "Locrian",
        ];

        public static string GetName(this WesternMode mode)
        {
            return _names[(int)mode];
        }
    }
}
