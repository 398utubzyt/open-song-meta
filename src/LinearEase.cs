// This file is a part of the Open Song Meta project.
// Copyright (c) 398utubzyt and Open Song Meta contributors.
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

namespace Osm
{
    public struct LinearEase : IBpmEase
    {
        public static double GetEase(double from, double to, double t)
        {
            return (from - to) * t;
        }

        public static double GetMagic(double from, double to, double t)
        {
            return 2.0 / ((1.0 / GetEase(from, to, t)) + (1.0 / from));
        }
    }
}
