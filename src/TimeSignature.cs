// This file is a part of the Open Song Meta project.
// Copyright (c) 398utubzyt and Open Song Meta contributors.
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

namespace Opsm
{
    public readonly struct TimeSignature(short num, short den)
    {
        public readonly short Numerator = num;
        public readonly short Denominator = den;

        public static readonly TimeSignature Common = new(4, 4);
        public static readonly TimeSignature Cut = new(4, 2);

        public override string ToString() => $"{Numerator}/{Denominator}";
    }
}
