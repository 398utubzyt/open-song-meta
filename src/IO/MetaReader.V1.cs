// This file is a part of the Open Song Meta project.
// Copyright (c) 398utubzyt and Open Song Meta contributors.
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System.Collections.Generic;

namespace Opsm.IO
{
    partial class MetaReader
    {
        private static class V1
        {
            public static MetaFile ReadMetaFile(MetaReader reader)
            {
                string name = reader._reader.ReadString();

                string? title = null;
                string? artist = null;
                string? album = null;
                TimedTrack<Tempo> tempos = null!;
                TimedTrack<TimeSignature> timeSignatures = null!;
                TimedTrack<KeySignature> keySignatures = null!;
                Dictionary<string, ExtensionBlock> blocks = [];

                while (reader._reader.TryReadBlockId(out BlockId id))
                {
                    switch (id)
                    {
                        case BlockId.Title:
                            title = reader._reader.ReadString();
                            break;
                        case BlockId.Artist:
                            artist = reader._reader.ReadString();
                            break;
                        case BlockId.Album:
                            album = reader._reader.ReadString();
                            break;
                        case BlockId.Tempo:
                            tempos = reader.ReadTempoTrack();
                            break;
                        case BlockId.TimeSignatures:
                            timeSignatures = reader.ReadTimeSignatureTrack();
                            break;
                        case BlockId.KeySignatures:
                            keySignatures = reader.ReadKeySignatureTrack();
                            break;
                        case BlockId.Extension:
                            string bname = reader._reader.ReadString();
                            blocks.Add(bname, new(bname, reader._reader.ReadBytes(reader._reader.Read7BitEncodedInt())));
                            break;
                    }
                }

                tempos ??= new(0);
                timeSignatures ??= new(0);
                keySignatures ??= new(0);

                return new(1, name, title, artist, album, tempos, timeSignatures, keySignatures, blocks);
            }
        }
    }
}
