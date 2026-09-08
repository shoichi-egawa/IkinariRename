// HEIC画像処理用 (Magick.NET)
using ImageMagick;
// メタデータ読み取り＆KML出力用
using MetadataExtractor;
using MetadataExtractor.Formats.Exif;
using SharpKml.Base;
using SharpKml.Dom;
using SharpKml.Engine;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace IkinariRename
{
    public partial class Form1 : Form
    {
        private string selectedFolderPath = "";
        private bool isKmlExported = false;

        public Form1()
        {
            InitializeComponent();

            // ① ヘッダーの中央揃え設定
            dataGridView1.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            // ② フォントサイズの設定
            Column3.DefaultCellStyle.Font = new Font(dataGridView1.Font.FontFamily, 10F, FontStyle.Bold);

            Font largerFont = new Font(dataGridView1.Font.FontFamily, 11F, FontStyle.Regular);
            Column7.DefaultCellStyle.Font = largerFont;
            Column8.DefaultCellStyle.Font = largerFont;

            // ③ 新写真ファイル名（Column8）の ReadOnly を解除し手入力修正可能にする
            Column8.ReadOnly = false;

            // 初期表示は「中（150px）」サイズ
            SetThumbnailSize(150, 200);

            dataGridView1.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            dataGridView1.DataError += (s, e) => e.ThrowException = false;
            dataGridView1.CellPainting += DataGridView1_CellPainting;
        }

        // サムネイル表示サイズを変更する共通メソッド
        private void SetThumbnailSize(int rowHeight, int colWidth)
        {
            dataGridView1.RowTemplate.Height = rowHeight;
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                row.Height = rowHeight;
            }

            Column4.Width = colWidth;

            int gridWidth = Column1.Width + Column3.Width + colWidth + Column7.Width + colType.Width + Column8.Width;
            int targetWidth = gridWidth + 100;

            this.MaximumSize = new Size(targetWidth, 2000);
            this.Width = targetWidth;
        }

        // 「基準点名を入力」のグレー文字を折り返し描画する処理
        private void DataGridView1_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.ColumnIndex == Column7.Index && e.RowIndex >= 0 && string.IsNullOrEmpty(e.Value?.ToString()))
            {
                e.PaintBackground(e.CellBounds, true);
                Rectangle rect = new Rectangle(e.CellBounds.X + 4, e.CellBounds.Y + 4, e.CellBounds.Width - 8, e.CellBounds.Height - 8);

                TextFormatFlags flags = TextFormatFlags.Left | TextFormatFlags.Top | TextFormatFlags.WordBreak;
                TextRenderer.DrawText(e.Graphics, "基準点名を\n入力", e.CellStyle.Font, rect, Color.Gray, flags);

                e.Handled = true;
            }
        }

        // 写真読み込みおよび並び替え処理（JPG / JPEG / HEIC に対応）
        private void LoadAndDisplayPhotos()
        {
            if (string.IsNullOrEmpty(selectedFolderPath)) return;

            dataGridView1.Rows.Clear();

            // JPG, JPEG, HEIC ファイルを取得
            string[] files = System.IO.Directory.GetFiles(selectedFolderPath, "*.*", SearchOption.TopDirectoryOnly)
                             .Where(f => f.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase) ||
                                         f.EndsWith(".jpeg", StringComparison.OrdinalIgnoreCase) ||
                                         f.EndsWith(".heic", StringComparison.OrdinalIgnoreCase))
                             .ToArray();

            if (files.Length == 0) return;

            var photoList = new List<PhotoReadData>();

            foreach (string filePath in files)
            {
                DateTime dtOriginal = DateTime.MaxValue;
                string takeDateTimeStr = "日時不明";
                string latStr = "", lngStr = "";
                Image thumbnail = null;

                bool isHeic = filePath.EndsWith(".heic", StringComparison.OrdinalIgnoreCase);

                try
                {
                    if (isHeic)
                    {
                        // Magick.NET を用いて HEIC からサムネイルとEXIFメタデータを抽出
                        using (var image = new MagickImage(filePath))
                        {
                            // サムネイル生成（300x300）
                            using (var ms = new MemoryStream())
                            {
                                image.Format = MagickFormat.Jpeg;
                                image.Resize(300, 300);
                                image.Write(ms);
                                ms.Position = 0;
                                thumbnail = new Bitmap(ms);
                            }

                            // EXIFデータ解析
                            var profile = image.GetExifProfile();
                            if (profile != null)
                            {
                                var dateTag = profile.GetValue(ExifTag.DateTimeOriginal);
                                if (dateTag != null)
                                {
                                    string rawDateStr = dateTag.Value.ToString();
                                    var match = Regex.Match(rawDateStr, @"^(\d{4})[:/](\d{1,2})[:/](\d{1,2})\s+(\d{1,2}):(\d{1,2})");
                                    if (match.Success)
                                    {
                                        int y = int.Parse(match.Groups[1].Value);
                                        int m = int.Parse(match.Groups[2].Value);
                                        int d = int.Parse(match.Groups[3].Value);
                                        int hh = int.Parse(match.Groups[4].Value);
                                        int mm = int.Parse(match.Groups[5].Value);

                                        dtOriginal = new DateTime(y, m, d, hh, mm, 0);
                                        takeDateTimeStr = $"{y}/{m}/{d} {hh}:{mm:D2}";
                                    }
                                }
                            }
                        }

                        // GPS情報は MetadataExtractor で補完取得
                        var directories = ImageMetadataReader.ReadMetadata(filePath);
                        var gpsDir = directories.OfType<GpsDirectory>().FirstOrDefault();
                        if (gpsDir != null && gpsDir.GetGeoLocation() != null)
                        {
                            dynamic loc = gpsDir.GetGeoLocation();
                            latStr = Convert.ToDouble(loc.Latitude).ToString("F6");
                            lngStr = Convert.ToDouble(loc.Longitude).ToString("F6");
                        }
                    }
                    else
                    {
                        // 従来の JPG / JPEG 処理
                        var directories = ImageMetadataReader.ReadMetadata(filePath);

                        var subIfdDir = directories.OfType<ExifSubIfdDirectory>().FirstOrDefault();
                        if (subIfdDir != null && subIfdDir.ContainsTag(ExifDirectoryBase.TagDateTimeOriginal))
                        {
                            string rawDateStr = subIfdDir.GetDescription(ExifDirectoryBase.TagDateTimeOriginal);
                            var match = Regex.Match(rawDateStr, @"^(\d{4})[:/](\d{1,2})[:/](\d{1,2})\s+(\d{1,2}):(\d{1,2})");
                            if (match.Success)
                            {
                                int y = int.Parse(match.Groups[1].Value);
                                int m = int.Parse(match.Groups[2].Value);
                                int d = int.Parse(match.Groups[3].Value);
                                int hh = int.Parse(match.Groups[4].Value);
                                int mm = int.Parse(match.Groups[5].Value);

                                dtOriginal = new DateTime(y, m, d, hh, mm, 0);
                                takeDateTimeStr = $"{y}/{m}/{d} {hh}:{mm:D2}";
                            }
                        }

                        var gpsDir = directories.OfType<GpsDirectory>().FirstOrDefault();
                        if (gpsDir != null && gpsDir.GetGeoLocation() != null)
                        {
                            dynamic loc = gpsDir.GetGeoLocation();
                            latStr = Convert.ToDouble(loc.Latitude).ToString("F6");
                            lngStr = Convert.ToDouble(loc.Longitude).ToString("F6");
                        }

                        using (var img = Image.FromFile(filePath))
                        {
                            thumbnail = new Bitmap(img);
                        }
                    }
                }
                catch { }

                photoList.Add(new PhotoReadData
                {
                    FilePath = filePath,
                    FileName = Path.GetFileName(filePath),
                    TakeDateTime = dtOriginal,
                    TakeDateTimeStr = takeDateTimeStr,
                    Latitude = latStr,
                    Longitude = lngStr,
                    Thumbnail = thumbnail
                });
            }

            // ソート順の判定
            List<PhotoReadData> sortedPhotos;
            if (rdoSortName != null && rdoSortName.Checked)
            {
                sortedPhotos = photoList.OrderBy(p => p.FileName).ToList();
            }
            else
            {
                sortedPhotos = photoList.OrderBy(p => p.TakeDateTime).ThenBy(p => p.FileName).ToList();
            }

            // DataGridViewへ追加
            int index = 1;
            foreach (var photo in sortedPhotos)
            {
                string photoInfoText = $"No.{index:D2}\n\n{photo.TakeDateTimeStr}\n緯度: {photo.Latitude}\n経度: {photo.Longitude}";

                int rowIndex = dataGridView1.Rows.Add(false, photoInfoText, photo.Thumbnail, "", "近景", "");

                dataGridView1.Rows[rowIndex].Tag = new PhotoTagInfo
                {
                    FilePath = photo.FilePath,
                    Latitude = photo.Latitude,
                    Longitude = photo.Longitude,
                    TempNo = $"No.{index:D2}"
                };
                index++;
            }
        }

        // ① 写真フォルダ選択ボタン
        private void button1_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog fbd = new FolderBrowserDialog())
            {
                if (fbd.ShowDialog() == DialogResult.OK)
                {
                    selectedFolderPath = fbd.SelectedPath;
                    isKmlExported = false;
                    LoadAndDisplayPhotos();
                }
            }
        }

        // ソート用ラジオボタン切り替え時イベント
        private void rdoSortDate_CheckedChanged(object sender, EventArgs e) => HandleSortChange(sender);
        private void rdoSort_CheckedChanged(object sender, EventArgs e) => HandleSortChange(sender);

        private void HandleSortChange(object sender)
        {
            RadioButton rdo = sender as RadioButton;
            if (rdo != null && rdo.Checked)
            {
                LoadAndDisplayPhotos();

                if (isKmlExported)
                {
                    DialogResult res = MessageBox.Show(
                        "ソート順（仮番号）が変更されました。\nGoogleアース出力を再実行しますか？",
                        "確認",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question
                    );

                    if (res == DialogResult.Yes)
                    {
                        btnExportKml_Click(null, null);
                    }
                }
            }
        }

        // サムネイル表示サイズ切替ボタンイベント（小・中・大）
        private void btnSizeSmall_Click(object sender, EventArgs e) => SetThumbnailSize(75, 100);
        private void btnSizeMedium_Click(object sender, EventArgs e) => SetThumbnailSize(150, 200);
        private void btnSizeLarge_Click(object sender, EventArgs e) => SetThumbnailSize(250, 330);

        // ② 入力時の新ファイル名自動計算 & 自動チェック連動
        private void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            Dictionary<string, int> counts = new Dictionary<string, int>();

            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if (row.IsNewRow) continue;

                string pointName = Convert.ToString(row.Cells[Column7.Index].Value)?.Trim() ?? "";
                string photoType = Convert.ToString(row.Cells[colType.Index].Value)?.Trim() ?? "";

                if (!string.IsNullOrEmpty(pointName))
                {
                    string baseName = !string.IsNullOrEmpty(photoType) ? $"{pointName}_{photoType}" : pointName;

                    if (!counts.ContainsKey(baseName))
                    {
                        counts[baseName] = 1;
                        row.Cells[Column8.Index].Value = baseName;
                    }
                    else
                    {
                        counts[baseName]++;
                        row.Cells[Column8.Index].Value = $"{baseName}-{counts[baseName]}";
                    }

                    if (e.ColumnIndex == Column7.Index || e.ColumnIndex == colType.Index)
                    {
                        if (e.RowIndex == row.Index)
                        {
                            row.Cells[Column1.Index].Value = true;
                        }
                    }
                }
                else
                {
                    if (e.ColumnIndex == Column7.Index && e.RowIndex == row.Index)
                    {
                        row.Cells[Column8.Index].Value = "";
                        row.Cells[Column1.Index].Value = false;
                    }
                }
            }
        }

        // ③ リネーム実行ボタン（HEICは拡張子を .jpg に自動変更して保存）
        private void btnRename_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(selectedFolderPath))
            {
                MessageBox.Show("先に写真フォルダを選択してください。", "案内", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string saveDir = Path.Combine(selectedFolderPath, "リネーム");
            if (!System.IO.Directory.Exists(saveDir))
            {
                System.IO.Directory.CreateDirectory(saveDir);
            }

            bool doDownsize = chkDownsize != null && chkDownsize.Checked;
            bool skipExistFiles = false;

            List<string> existFiles = new List<string>();
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if (row.IsNewRow) continue;
                bool isSelected = Convert.ToBoolean(row.Cells[Column1.Index].Value);

                if (isSelected && row.Tag is PhotoTagInfo tagInfo)
                {
                    string newName = Convert.ToString(row.Cells[Column8.Index].Value);
                    if (string.IsNullOrEmpty(newName))
                    {
                        newName = Path.GetFileNameWithoutExtension(tagInfo.FilePath);
                    }

                    string ext = tagInfo.FilePath.EndsWith(".heic", StringComparison.OrdinalIgnoreCase) ? ".jpg" : Path.GetExtension(tagInfo.FilePath);
                    string destPath = Path.Combine(saveDir, newName + ext);

                    if (File.Exists(destPath))
                    {
                        existFiles.Add(newName + ext);
                    }
                }
            }

            if (existFiles.Count > 0)
            {
                DialogResult dialogResult = MessageBox.Show(
                    $"「リネーム」フォルダ内に同名の写真が {existFiles.Count} 件すでに存在します。\n\n上書き保存してよろしいですか？\n（[いいえ] を選ぶと同名ファイルはスキップされます）",
                    "上書き確認",
                    MessageBoxButtons.YesNoCancel,
                    MessageBoxIcon.Warning
                );

                if (dialogResult == DialogResult.No)
                {
                    skipExistFiles = true;
                }
                else if (dialogResult == DialogResult.Cancel)
                {
                    return;
                }
            }

            int successCount = 0;
            int skippedCount = 0;

            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if (row.IsNewRow) continue;
                bool isSelected = Convert.ToBoolean(row.Cells[Column1.Index].Value);

                if (isSelected && row.Tag is PhotoTagInfo tagInfo)
                {
                    string newName = Convert.ToString(row.Cells[Column8.Index].Value);
                    if (string.IsNullOrEmpty(newName))
                    {
                        newName = Path.GetFileNameWithoutExtension(tagInfo.FilePath);
                    }

                    string ext = tagInfo.FilePath.EndsWith(".heic", StringComparison.OrdinalIgnoreCase) ? ".jpg" : Path.GetExtension(tagInfo.FilePath);
                    string destPath = Path.Combine(saveDir, newName + ext);

                    if (skipExistFiles && File.Exists(destPath))
                    {
                        skippedCount++;
                        continue;
                    }

                    try
                    {
                        bool isHeic = tagInfo.FilePath.EndsWith(".heic", StringComparison.OrdinalIgnoreCase);

                        if (isHeic)
                        {
                            ConvertHeicToJpg(tagInfo.FilePath, destPath, doDownsize);
                        }
                        else if (doDownsize)
                        {
                            SaveImageResizedAndCompressed(tagInfo.FilePath, destPath, 150 * 1024);
                        }
                        else
                        {
                            File.Copy(tagInfo.FilePath, destPath, true);
                        }
                        successCount++;
                    }
                    catch { }
                }
            }

            MessageBox.Show($"処理が完了しました！\n成功: {successCount} 件\nスキップ: {skippedCount} 件", "完了", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        // ★HEIC画像をJPG形式へ変換・保存するメソッド（型キャスト型修飾子を追加）
        private void ConvertHeicToJpg(string srcPath, string destPath, bool doDownsize)
        {
            using (var image = new MagickImage(srcPath))
            {
                image.Format = MagickFormat.Jpeg;

                if (doDownsize)
                {
                    uint maxDimension = 1280;
                    if (image.Width > maxDimension || image.Height > maxDimension)
                    {
                        image.Resize(maxDimension, maxDimension);
                    }
                    image.Quality = 50;
                }

                image.Write(destPath);
            }
        }

        // 高精度ダウンサイジング（JPG用：アスペクト比固定＆約150KBターゲット）
        private void SaveImageResizedAndCompressed(string srcPath, string destPath, long targetSizeBytes)
        {
            using (Image srcImg = Image.FromFile(srcPath))
            {
                int newWidth = srcImg.Width;
                int newHeight = srcImg.Height;
                int maxDimension = 1280;

                if (srcImg.Width > maxDimension || srcImg.Height > maxDimension)
                {
                    double aspectRatio = (double)srcImg.Width / srcImg.Height;

                    if (srcImg.Width >= srcImg.Height)
                    {
                        newWidth = maxDimension;
                        newHeight = (int)Math.Round(maxDimension / aspectRatio);
                    }
                    else
                    {
                        newHeight = maxDimension;
                        newWidth = (int)Math.Round(maxDimension * aspectRatio);
                    }
                }

                using (Bitmap resizedBmp = new Bitmap(newWidth, newHeight))
                {
                    using (Graphics g = Graphics.FromImage(resizedBmp))
                    {
                        g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                        g.DrawImage(srcImg, 0, 0, newWidth, newHeight);
                    }

                    ImageCodecInfo jpgEncoder = GetEncoder(ImageFormat.Jpeg);
                    Encoder myEncoder = Encoder.Quality;

                    long[] qualitySteps = new long[] { 65, 50, 40, 30, 20 };
                    long bestQuality = 30;

                    foreach (long q in qualitySteps)
                    {
                        EncoderParameters ep = new EncoderParameters(1);
                        ep.Param[0] = new EncoderParameter(myEncoder, q);

                        using (MemoryStream ms = new MemoryStream())
                        {
                            resizedBmp.Save(ms, jpgEncoder, ep);
                            bestQuality = q;

                            if (ms.Length <= targetSizeBytes)
                            {
                                break;
                            }
                        }
                    }

                    EncoderParameters finalEp = new EncoderParameters(1);
                    finalEp.Param[0] = new EncoderParameter(myEncoder, bestQuality);
                    resizedBmp.Save(destPath, jpgEncoder, finalEp);
                }
            }
        }

        private ImageCodecInfo GetEncoder(ImageFormat format)
        {
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageEncoders();
            foreach (ImageCodecInfo codec in codecs)
            {
                if (codec.FormatID == format.Guid) return codec;
            }
            return null;
        }

        // ④ Googleアース出力ボタン
        private void btnExportKml_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(selectedFolderPath))
            {
                MessageBox.Show("先に写真フォルダを選択してください。", "案内", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            Document kmlDoc = new Document { Name = "写真撮影位置" };

            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if (row.IsNewRow) continue;
                if (row.Tag is PhotoTagInfo tagInfo)
                {
                    if (double.TryParse(tagInfo.Latitude, out double lat) && double.TryParse(tagInfo.Longitude, out double lng))
                    {
                        string finalFileName = Convert.ToString(row.Cells[Column8.Index].Value);
                        string nameText = !string.IsNullOrEmpty(finalFileName) ? finalFileName : tagInfo.TempNo;

                        Placemark placemark = new Placemark
                        {
                            Name = nameText,
                            Geometry = new SharpKml.Dom.Point { Coordinate = new Vector(lat, lng) }
                        };

                        kmlDoc.AddFeature(placemark);
                    }
                }
            }

            string savePath = Path.Combine(selectedFolderPath, "PhotoLocations.kml");
            using (var stream = File.Create(savePath))
            {
                KmlFile.Create(kmlDoc, false).Save(stream);
            }

            isKmlExported = true;

            try
            {
                ProcessStartInfo psi = new ProcessStartInfo
                {
                    FileName = savePath,
                    UseShellExecute = true
                };
                Process.Start(psi);
            }
            catch
            {
                MessageBox.Show($"Googleアース用ファイルを出力しました:\n{savePath}", "完了");
            }
        }

        // 全選択・全解除処理
        private void btnSelectAll_Click(object sender, EventArgs e) => SetCheckAll(true);
        private void btnDeselectAll_Click(object sender, EventArgs e) => SetCheckAll(false);

        private void SetCheckAll(bool check)
        {
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if (!row.IsNewRow) row.Cells[Column1.Index].Value = check;
            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e) { }
    }

    public class PhotoReadData
    {
        public string FilePath { get; set; }
        public string FileName { get; set; }
        public DateTime TakeDateTime { get; set; }
        public string TakeDateTimeStr { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public Image Thumbnail { get; set; }
    }

    public class PhotoTagInfo
    {
        public string FilePath { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string TempNo { get; set; }
    }
}