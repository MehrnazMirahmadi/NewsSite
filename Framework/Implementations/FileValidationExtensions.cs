using Microsoft.AspNetCore.Http;
using System;
using System.Text;

namespace Framework.Implementations
{
    public static class FileValidationExtensions
    {
        public static bool IsValidImageHeader(this IFormFile file)
        {
            using (var fileStream = file.OpenReadStream())
            {
                byte[] buffer = new byte[12]; // Adjust buffer size to accommodate formats like HEIF/AVIF/ICO
                fileStream.Read(buffer, 0, buffer.Length);

                return IsJpeg(buffer) || IsPng(buffer) || IsGif(buffer) || IsBmp(buffer) ||
                       IsTiff(buffer) || IsRiff(buffer) || IsWebP(buffer) || IsVp8(buffer) ||
                       IsHeif(buffer) || IsAvif(buffer) || IsSVG(buffer) || IsIco(buffer) ||
                       IsCur(buffer) || IsPsd(buffer) || IsXcf(buffer) || IsDds(buffer) ||
                       IsJp2(buffer) || IsBpg(buffer) || IsPict(buffer) || IsCr2(buffer) ||
                       IsNef(buffer) || IsRaf(buffer) || IsOrf(buffer) || IsDicom(buffer) || IsRas(buffer);
            }
        }

        private static bool IsJpeg(byte[] buffer)
        {
            return buffer[0] == 0xFF && buffer[1] == 0xD8 && buffer[2] == 0xFF;
        }

        private static bool IsPng(byte[] buffer)
        {
            return buffer[0] == 0x89 && buffer[1] == 0x50 && buffer[2] == 0x4E && buffer[3] == 0x47 &&
                   buffer[4] == 0x0D && buffer[5] == 0x0A && buffer[6] == 0x1A && buffer[7] == 0x0A;
        }

        private static bool IsGif(byte[] buffer)
        {
            return buffer[0] == 0x47 && buffer[1] == 0x49 && buffer[2] == 0x46 && buffer[3] == 0x38;
        }

        private static bool IsBmp(byte[] buffer)
        {
            return buffer[0] == 0x42 && buffer[1] == 0x4D; // 'BM'
        }

        private static bool IsTiff(byte[] buffer)
        {
            return (buffer[0] == 0x49 && buffer[1] == 0x2A) || // 'II*' (Intel)
                   (buffer[0] == 0x4D && buffer[1] == 0x2A);   // 'MM*' (Motorola)
        }

        private static bool IsRiff(byte[] buffer)
        {
            return buffer[0] == 0x52 && buffer[1] == 0x49 && buffer[2] == 0x46 && buffer[3] == 0x46; // 'RIFF'
        }

        private static bool IsWebP(byte[] buffer)
        {
            return buffer[0] == 0x57 && buffer[1] == 0x45 && buffer[2] == 0x42 && buffer[3] == 0x50; // 'WEBP'
        }

        private static bool IsVp8(byte[] buffer)
        {
            return buffer[0] == 0x56 && buffer[1] == 0x50 && buffer[2] == 0x38; // 'VP8'
        }

        private static bool IsHeif(byte[] buffer)
        {
            return buffer[0] == 0x66 && buffer[1] == 0x74 && buffer[2] == 0x79 && buffer[3] == 0x70 && // 'ftyp'
                   Encoding.ASCII.GetString(buffer, 8, 4).Equals("heic", StringComparison.OrdinalIgnoreCase);
        }

        private static bool IsAvif(byte[] buffer)
        {
            return buffer[0] == 0x66 && buffer[1] == 0x74 && buffer[2] == 0x79 && buffer[3] == 0x70 && // 'ftyp'
                   Encoding.ASCII.GetString(buffer, 8, 4).Equals("avif", StringComparison.OrdinalIgnoreCase);
        }

        private static bool IsSVG(byte[] buffer)
        {
            string header = Encoding.UTF8.GetString(buffer);
            return header.StartsWith("<?xml") && header.Contains("<svg");
        }

        private static bool IsIco(byte[] buffer)
        {
            return buffer[0] == 0x00 && buffer[1] == 0x00 && buffer[2] == 0x01 && buffer[3] == 0x00;
        }

        private static bool IsCur(byte[] buffer)
        {
            return buffer[0] == 0x00 && buffer[1] == 0x00 && buffer[2] == 0x02 && buffer[3] == 0x00;
        }

        private static bool IsPsd(byte[] buffer)
        {
            return buffer[0] == 0x38 && buffer[1] == 0x42 && buffer[2] == 0x50 && buffer[3] == 0x53; // '8BPS'
        }

        // Check for GIMP XCF file format.
        private static bool IsXcf(byte[] buffer)
        {
            return buffer[0] == 0x67 && buffer[1] == 0x69 && buffer[2] == 0x6d && buffer[3] == 0x70; // 'gimp'
        }

        // Check for DDS file format.
        private static bool IsDds(byte[] buffer)
        {
            return buffer[0] == 0x44 && buffer[1] == 0x44 && buffer[2] == 0x53 && buffer[3] == 0x20; // 'DDS '
        }

        // Check for JPEG 2000 JP2 file format.
        private static bool IsJp2(byte[] buffer)
        {
            return buffer[0] == 0x00 && buffer[1] == 0x00 && buffer[2] == 0x00 && buffer[3] == 0x0C && // JPEG 2000 signature box
                   buffer[4] == 0x6A && buffer[5] == 0x50 && buffer[6] == 0x20 && buffer[7] == 0x20; // 'jP  '
        }

        // Check for BPG file format.
        private static bool IsBpg(byte[] buffer)
        {
            return buffer[0] == 0x42 && buffer[1] == 0x50 && buffer[2] == 0x47 && buffer[3] == 0xFB; // 'BPG'
        }

        // Check for Apple PICT file format.
        private static bool IsPict(byte[] buffer)
        {
            return buffer[0] == 0x00 && buffer[1] == 0x11 && buffer[2] == 0x02 && buffer[3] == 0xFF; // PICT signature
        }

        // Check for Canon CR2 file format.
        private static bool IsCr2(byte[] buffer)
        {
            return buffer[0] == 0x49 && buffer[1] == 0x49 && buffer[2] == 0x2A && buffer[3] == 0x00 && // 'II*'
                   buffer[8] == 0x43 && buffer[9] == 0x52; // 'CR'
        }

        // Check for Nikon NEF file format.
        private static bool IsNef(byte[] buffer)
        {
            return buffer[0] == 0x49 && buffer[1] == 0x49 && buffer[2] == 0x2A && buffer[3] == 0x00; // 'II*' (TIFF-based)
        }

        // Check for Fujifilm RAF file format.
        private static bool IsRaf(byte[] buffer)
        {
            return buffer[0] == 0x46 && buffer[1] == 0x55 && buffer[2] == 0x4A && buffer[3] == 0x49; // 'FUJI'
        }

        // Check for Olympus ORF file format.
        private static bool IsOrf(byte[] buffer)
        {
            return buffer[0] == 0x49 && buffer[1] == 0x49 && buffer[2] == 0x52 && buffer[3] == 0x4F; // 'IIRO'
        }

        // Check for DICOM file format.
        private static bool IsDicom(byte[] buffer)
        {
            return buffer[0] == 0x44 && buffer[1] == 0x49 && buffer[2] == 0x43 && buffer[3] == 0x4D; // 'DICM'
        }

        // Check for Sun Raster RAS file format.
        private static bool IsRas(byte[] buffer)
        {
            return buffer[0] == 0x59 && buffer[1] == 0xA6 && buffer[2] == 0x6A && buffer[3] == 0x95; // 'Yasj'
        }
    }
}
