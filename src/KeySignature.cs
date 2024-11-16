// This file is a part of the Open Song Meta project.
// Copyright (c) 398utubzyt and Open Song Meta contributors.
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

namespace Osm
{
    public readonly struct KeySignature(WesternNote root, WesternMode mode)
    {
        public readonly WesternNote Root = root;
        public readonly WesternMode Mode = mode;

        public override string ToString() => Root.KeyToString(Mode);
        public string ToString(bool preferFlat = true) => Root.KeyToString(Mode, preferFlat);
    }
}
