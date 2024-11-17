// This file is a part of the Open Song Meta project.
// Copyright (c) 398utubzyt and Open Song Meta contributors.
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

namespace Opsm
{
    public readonly struct TimedValue<T>(double t, T value)
    {
        public readonly double Time = t;
        public readonly T Value = value;

        public override string ToString() => $"{{{Time}, {Value}}}";
    }
}
