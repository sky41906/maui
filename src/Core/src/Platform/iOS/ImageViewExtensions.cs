using System.Threading;
using System.Threading.Tasks;
using UIKit;

namespace Microsoft.Maui.Platform
{
	public static class ImageViewExtensions
	{
		public static void Clear(this UIImageView imageView)
		{
			imageView.Image = null;
		}

		public static void UpdateAspect(this UIImageView imageView, IImage image)
		{
			imageView.ContentMode = image.Aspect.ToUIViewContentMode();
			imageView.ClipsToBounds = imageView.ContentMode == UIViewContentMode.ScaleAspectFill;
		}

		public static async Task UpdateIsAnimationPlaying(this UIImageView imageView, IImageSourcePart image)
		{

			if (imageView is MauiImageView mauiImageView)
			{				
				if (image.IsAnimationPlaying)
				{
					if (!imageView.IsAnimating)
					{
						mauiImageView.Animation = await ImageAnimationHelper.CreateAnimationFromImageSource(image.Source);
						mauiImageView.StartAnimating();
					}
				}
				else
				{
					if (imageView.IsAnimating)
					{
						mauiImageView.StopAnimating();
					}
				}
			}
		}

		public static void UpdateSource(this UIImageView imageView, UIImage? uIImage, IImageSourcePart image)
		{
			imageView.Image = uIImage;
			imageView.UpdateIsAnimationPlaying(image).FireAndForget();
		}

		public static Task<IImageSourceServiceResult<UIImage>?> UpdateSourceAsync(
			this UIImageView imageView,
			IImageSourcePart image,
			IImageSourceServiceProvider services,
			CancellationToken cancellationToken = default)
		{
			float scale = imageView.Window?.GetDisplayDensity() ?? 1.0f;

			imageView.Clear();
			return image.UpdateSourceAsync(imageView, services, (uiImage) =>
			{
				imageView.Image = uiImage;
			}, scale, cancellationToken);
		}
	}
}