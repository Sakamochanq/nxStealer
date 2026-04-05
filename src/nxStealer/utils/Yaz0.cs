using System;

namespace nxStealer.utils
{
    internal class Yaz0
    {
        // Yaz0圧縮形式を解凍する
        public static byte[] Decompress(byte[] data)
        {
            // Yaz0の有無を確認
            if (data.Length < 16 ||
                data[0] != 0x59 || data[1] != 0x61 ||
                data[2] != 0x7A || data[3] != 0x30)
            {
                throw new Exception("Invalid Yaz0 format");
            }

            // 解凍後のサイズを取得（OFFSET4-8)
            int decompressedSize = (data[4] << 24) | (data[5] << 16) |
                                   (data[6] << 8) | data[7];

            byte[] Yaz0 = new byte[decompressedSize];
            int srcPos = 16;
            int dstPos = 0;

            byte codeByte = 0;
            int codeBytePos = 0;

            while (dstPos < decompressedSize)
            {
                // 8ビットごとに制御バイトを読む
                if (codeBytePos == 0)
                {
                    codeByte = data[srcPos++];
                    codeBytePos = 8;
                }

                if ((codeByte & 0x80) != 0)
                {
                    // 非圧縮データ
                    Yaz0[dstPos++] = data[srcPos++];
                }
                else
                {
                    // 圧縮データ（ビットが0の場合）
                    // 2バイトから距離と長さを取得
                    byte b1 = data[srcPos++];
                    byte b2 = data[srcPos++];

                    // 距離の計算（過去のデータを参照する位置）
                    int dist = ((b1 & 0x0F) << 8) | b2;
                    int copyPos = dstPos - (dist + 1);

                    // 長さの計算（コピーするバイト数）
                    int length = (b1 >> 4);
                    if (length == 0)
                    {
                        // 長さが0の場合は次のバイトから長さを読む
                        length = data[srcPos++] + 0x12;
                    }
                    else
                    {
                        length += 2;
                    }

                    // 過去のデータをコピー
                    for (int i = 0; i < length; i++)
                    {
                        Yaz0[dstPos++] = Yaz0[copyPos++];
                    }
                }

                // 次のビットへ
                codeByte <<= 1;
                codeBytePos--;
            }

            return Yaz0;
        }
    }
}
