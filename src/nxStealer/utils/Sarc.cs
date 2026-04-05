using System;
using System.Text;

namespace nxStealer.utils
{
    internal class Sarc
    {
        // SARCから info.json を抽出
        public static string Extract(byte[] data)
        {
            return ExtractFile(data, "info.json");
        }

        public static string ExtractFile(byte[] data, string targetFileName)
        {
            byte[] fileBytes = ExtractFileBytes(data, targetFileName);
            return Encoding.UTF8.GetString(fileBytes);
        }

        public static byte[] ExtractFileBytes(byte[] data, string targetFileName)
        {
            return ExtractFileInternal(data, targetFileName);
        }

        // SARCからファイルを抽出
        public static byte[] ExtractFileInternal(byte[] data, string targetFileName)
        {
            // SARCの有無を確認
            if (data.Length < 20 ||
                data[0] != 0x53 || data[1] != 0x41 ||
                data[2] != 0x52 || data[3] != 0x43)
            {
                throw new Exception("Invalid SARC format");
            }

            // データブロックの開始位置をSARCヘッダーから取得（offset 12-15）
            int dataBlockStart = data[12] | (data[13] << 8) | (data[14] << 16) | (data[15] << 24);

            // SFATをサーチ
            int sfatOffset = FindSignature(data, new byte[] { 0x53, 0x46, 0x41, 0x54 });
            if (sfatOffset == -1)
            {
                throw new Exception("SFAT not found");
            }

            // ファイル数を取得（SFATオフセット+6、リトルエンディアン、2バイト）
            int fileCount = data[sfatOffset + 6] | (data[sfatOffset + 7] << 8);

            // 各ファイルエントリの開始位置
            int entryStart = sfatOffset + 12;

            // SFNT（String Table）を探す
            int sfntOffset = FindSignature(data, new byte[] { 0x53, 0x46, 0x4E, 0x54 }, sfatOffset);
            if (sfntOffset == -1)
            {
                throw new Exception("SFNT not found");
            }

            // ファイル名とデータオフセットを取得
            for (int i = 0; i < fileCount; i++)
            {
                int entryPos = entryStart + (i * 16);

                // ファイル名オフセット
                int nameOffset = (data[entryPos + 4] |
                                 (data[entryPos + 5] << 8) |
                                 (data[entryPos + 6] << 16)) * 4;

                // データ開始オフセット
                int dataStart = data[entryPos + 8] | (data[entryPos + 9] << 8) |
                               (data[entryPos + 10] << 16) | (data[entryPos + 11] << 24);

                // データ終了オフセット
                int dataEnd = data[entryPos + 12] | (data[entryPos + 13] << 8) |
                             (data[entryPos + 14] << 16) | (data[entryPos + 15] << 24);

                // ファイル名を取得
                int namePos = sfntOffset + 8 + nameOffset;
                StringBuilder fileName = new StringBuilder();
                while (namePos < data.Length && data[namePos] != 0)
                {
                    fileName.Append((char)data[namePos++]);
                }

                // 対象ファイルが見つかったら内容を返す
                if (fileName.ToString() == targetFileName)
                {
                    // データブロックの開始位置 + 相対オフセット
                    int dataOffset = dataBlockStart + dataStart;
                    int dataSize = dataEnd - dataStart;

                    if (dataSize <= 0)
                    {
                        throw new Exception($"{targetFileName} has invalid size: {dataSize}. dataStart=0x{dataStart:X}, dataEnd=0x{dataEnd:X}");
                    }

                    if (dataOffset + dataSize <= data.Length)
                    {
                        //return Encoding.UTF8.GetString(data, dataOffset, dataSize);

                        byte[] result = new byte[dataSize];
                        Array.Copy(data, dataOffset, result, 0, dataSize);
                        return result;
                    }
                    else
                    {
                        throw new Exception($"{targetFileName} data out of bounds: offset=0x{dataOffset:X}, size={dataSize}, data.Length={data.Length}");
                    }
                }
            }

            throw new Exception($"{targetFileName} not found in SARC archive");
        }

        // バイト配列内で特定のシグネチャを検索
        private static int FindSignature(byte[] data, byte[] signature, int startOffset = 0)
        {
            for (int i = startOffset; i <= data.Length - signature.Length; i++)
            {
                bool match = true;
                for (int j = 0; j < signature.Length; j++)
                {
                    if (data[i + j] != signature[j])
                    {
                        match = false;
                        break;
                    }
                }
                if (match) return i;
            }
            return -1;
        }
    }
}
