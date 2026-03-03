using System;
using System.IO;
using System.Text.Json;
using System.Windows.Forms;

namespace nxtheme_analyzer.utils
{
    internal class NxTheme
    {
        // SARCアーカイブから抽出した info.json  
        private string infoJson;

        //初期化
        private JsonDocument doc;

        public NxTheme(string filePath)
        {
            // ファイルを読み込み  
            byte[] fileData = File.ReadAllBytes(filePath);
            // Yaz0解凍  
            byte[] decompressed = Yaz0.Decompress(fileData);

            // SARCアーカイブから info.json を抽出  
            infoJson = Sarc.Extract(decompressed);

            // JsonDocumentを初期化  
            doc = JsonDocument.Parse(infoJson);
        }

        // 作成者名
        public string Author()
        {
            if (doc.RootElement.TryGetProperty("Author", out JsonElement author))
            {
                return author.GetString();
            }
            return null;
        }


        // テーマ名
        public string Name()
        {
            if (doc.RootElement.TryGetProperty("ThemeName", out JsonElement themeName))
            {
                return themeName.GetString();
            }
            return null;
        }

        // NxThemeのバージョン
        public int Version()
        {
            if (doc.RootElement.TryGetProperty("Version", out JsonElement ver))
            {
                if (ver.ValueKind == JsonValueKind.Number && ver.TryGetInt32(out int version))
                {
                    return version;
                }
            }
            return 0;
        }

        // テーマの対象
        public string Target()
        {
            if (doc.RootElement.TryGetProperty("Target", out JsonElement target))
            {
                return target.GetString();
            }
            return null;
        }
    }
}