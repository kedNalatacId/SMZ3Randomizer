﻿using System;
using System.IO;
using System.Collections.Generic;
using static Randomizer.CLI.FileHelper;

namespace Randomizer.CLI.FileData {
    static class Rom {
        static Lazy<byte[]> BaseRom;

        public static byte[] CombineSMZ3Rom(Stream smRom, Stream z3Rom) {
            int pos;
            var combined = new byte[0x600000];

            /* SM hi bank */
            pos = 0;
            for (var i = 0; i < 0x40; i++) {
                smRom.Read(combined, pos + 0x8000, 0x8000);
                pos += 0x10000;
            }

            /* SM lo bank */
            pos = 0;
            for (var i = 0; i < 0x40; i++) {
                smRom.Read(combined, pos, 0x8000);
                pos += 0x10000;
            }

            /* Z3 hi bank*/
            pos = 0x400000;
            for (var i = 0; i < 0x20; i++) {
                z3Rom.Read(combined, pos + 0x8000, 0x8000);
                pos += 0x10000;
            }

            return combined;
        }

        public static void ApplyIps(byte[] rom, Stream ips) {
            const int header = 5;
            const int footer = 3;
            ips.Seek(header, SeekOrigin.Begin);
            while (ips.Position + footer < ips.Length) {
                var offset = (ips.ReadByte() << 16) | (ips.ReadByte() << 8) | ips.ReadByte();
                var size = (ips.ReadByte() << 8) | ips.ReadByte();
                if (size > 0) {
                    ips.Read(rom, offset, size);
                } else {
                    var rleSize = (ips.ReadByte() << 8) | ips.ReadByte();
                    var rleByte = (byte)ips.ReadByte();
                    Array.Fill(rom, rleByte, offset, rleSize);
                }
            }
        }

        public static void ApplySeed(byte[] rom, IDictionary<int, byte[]> patch) {
            foreach (var (offset, bytes) in patch)
                bytes.CopyTo(rom, offset);
        }

        public static byte[] ConstructBaseRom(string smFile, string z3File, string base_ips) {
            if (BaseRom != null)
                return (byte[]) BaseRom.Value.Clone();

            BaseRom = new Lazy<byte[]> (() => {
                using var sm = File.OpenRead(smFile);
                using var z3 = File.OpenRead(z3File);
                var rom = FileData.Rom.CombineSMZ3Rom(sm, z3);
                using var ips = OpenReadInnerStream(base_ips);
                FileData.Rom.ApplyIps(rom, ips);
                return rom;
            });

            return (byte[]) BaseRom.Value.Clone();
        }
    }
}
