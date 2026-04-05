using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text.Json;

namespace nxStealer.utils
{
    internal class NxTheme
    {
        // SARCアーカイブから抽出した info.json  
        private string infoJson;

        // 初期化
        private JsonDocument doc;

        // 解凍したデータ
        private byte[] decompressed;

        public NxTheme(string filePath)
        {
            // ファイルを読み込み  
            byte[] fileData = File.ReadAllBytes(filePath);
            
            // Yaz0解凍  
            decompressed = Yaz0.Decompress(fileData);

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
            if (doc.RootElement.TryGetProperty("Version", out JsonElement version))
            {
                return version.GetInt32();
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

        // 設定されている画像を取得
        public Image nxImage()
        {
            // PNG, JPG形式の画像をサーチ
            string[] imageFiles = { "image.jpg", "image.png" };

            foreach (string fileName in imageFiles)
            {
                try
                {
                    byte[] imageData = Sarc.ExtractFileBytes(decompressed, fileName);
                    if (imageData != null && imageData.Length > 0)
                    {
                        using (var ms = new MemoryStream(imageData))
                        {
                            using (var original = Image.FromStream(ms))
                            {
                                return new Bitmap(original);
                            }
                        }
                    }
                }
                catch
                {
                    continue;
                }
            }
            return null;
        }
    }
}