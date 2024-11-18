// This file is a part of the Open Song Meta project.
// Copyright (c) 398utubzyt and Open Song Meta contributors.
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;

namespace Opsm.IO
{
    public static class BinaryIOExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Write(this BinaryWriter self, BlockId id)
        {
            self.Write((byte)id);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool WriteAlignDummy(this BinaryWriter self, int alignment)
        {
            long alignedOff = alignment - (self.BaseStream.Position % alignment);
            if (alignedOff == alignment)
                return false;

            while (--alignedOff > 0)
            {
                Write(self, BlockId.Dummy);
            }

            return true;
        }
        public static void WriteOptional(this BinaryWriter self, BlockId id, string? str)
        {
            if (str != null)
            {
                Write(self, id);
                self.Write(str!);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static BlockId ReadBlockId(this BinaryReader self)
        {
            return (BlockId)self.ReadByte();
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryReadBlockId(this BinaryReader self, out BlockId id)
        {
            id = ReadBlockId(self);
            return id != BlockId.None && id != BlockId.Reserved;
        }
    }
}
