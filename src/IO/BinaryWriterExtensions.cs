// This file is a part of the Open Song Meta project.
// Copyright (c) 398utubzyt and Open Song Meta contributors.
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System;
using System.Collections.Generic;
using System.IO;

namespace Osm.IO
{
    public static class BinaryIOExtensions
    {
        public static void WriteOptional(this BinaryWriter self, string? str)
        {
            bool isValid = str != null;
            self.Write(isValid ? (byte)1 : (byte)0);
            if (isValid)
            {
                self.Write((byte)1);
                self.Write(str!);
            } else
            {
                self.Write((byte)0);
            }
        }
        public static string? ReadStringOptional(this BinaryReader self)
        {
            return self.ReadByte() == 0 ? null : self.ReadString();
        }
    }
}
