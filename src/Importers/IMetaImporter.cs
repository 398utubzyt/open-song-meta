// This file is a part of the Open Song Meta project.
// Copyright (c) 398utubzyt and Open Song Meta contributors.
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using Osm.IO;

using System;

namespace Osm.Importers
{
    public interface IMetaImporter : IDisposable
    {
        public string ImporterName { get; }
        public bool IsPopulated { get; }

        public bool ValidatePath(string path);
        public bool Populate(MetaReader reader);
        public bool Import(out MetaFile meta);
    }
}
