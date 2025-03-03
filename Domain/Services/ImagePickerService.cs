using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ShopMate._2._0.Domain.Services
{
    public class ImagePickerService
    {
        private readonly SemaphoreSlim semaphoreSlim = new(1, 1);

        public async Task<string> ImageUploadAsync()
        {
            if (!await semaphoreSlim.WaitAsync(0))
            {
                return null;
            }

            try
            {

               var _imageStream = await MediaPicker.PickPhotoAsync(new MediaPickerOptions { Title = "Select a photo" });

                if (_imageStream == null)
                {
                    return null;
                }

                using (var stream = await _imageStream.OpenReadAsync())
                {
                    byte[] result;
                    using (var streamReader = new MemoryStream())
                    {
                        await stream.CopyToAsync(streamReader);
                        result = streamReader.ToArray();
                    }

                    byte[] resizedImage = ResizeImage(result, 530, 310);

                    var imagePath = Convert.ToBase64String(resizedImage);
                    return string.Format("data:image/png;base64,{0}", imagePath);

                    
                }


            }
            catch (Exception ex)
            {
                return null;
            }
            finally
            {
                semaphoreSlim.Release();
            }
        }
        private byte[] ResizeImage(byte[] imageData, int width, int height)
        {
            using (var inputStream = new MemoryStream(imageData))
            {
                using (var original = SKBitmap.Decode(inputStream))
                {
                    float aspectRatio = Math.Min((float)width / original.Width, (float)height / original.Height);

                    int newWidth = (int)(original.Width * aspectRatio);
                    int newHeight = (int)(original.Height * aspectRatio);

                    var info = new SKImageInfo(newWidth, newHeight);
                    using (var resized = original.Resize(info, SKFilterQuality.Medium))
                    {
                        if (resized == null)
                            return imageData;

                        using (var image = SKImage.FromBitmap(resized))
                        {
                            using (var outputStream = new MemoryStream())
                            {
                                image.Encode(SKEncodedImageFormat.Png, 75).SaveTo(outputStream);
                                return outputStream.ToArray();
                            }
                        }
                    }
                }
            }
        }
    }
}
