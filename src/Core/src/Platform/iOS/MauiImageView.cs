using System;
using System.Diagnostics.CodeAnalysis;
using CoreAnimation;
using CoreGraphics;
using Microsoft.Maui.Graphics;
using UIKit;

namespace Microsoft.Maui.Platform
{
	public class MauiImageView : UIImageView, IUIViewLifeCycleEvents
	{
		bool _isDisposed;
		const string AnimationLayerName = "MauiUIImageViewAnimation";
		WeakReference<MauiCAKeyFrameAnimation>? _animation;

		readonly WeakReference<IImageHandler>? _handler;

		public MauiImageView(IImageHandler handler) => _handler = new(handler);

		[Obsolete("Use MauiImageView(IImageHandler handler) instead.")]
		public MauiImageView()
		{
		}

		[Obsolete("Use MauiImageView(IImageHandler handler) instead.")]
		public MauiImageView(CGRect frame)
			: base(frame)
		{
		}

		[UnconditionalSuppressMessage("Memory", "MA0002", Justification = IUIViewLifeCycleEvents.UnconditionalSuppressMessage)]
		EventHandler? _movedToWindow;
		event EventHandler IUIViewLifeCycleEvents.MovedToWindow
		{
			add => _movedToWindow += value;
			remove => _movedToWindow -= value;
		}

		public override void MovedToWindow()
		{
			if (_handler is not null && _handler.TryGetTarget(out var handler))
			{
				handler.OnWindowChanged();
			}
			_movedToWindow?.Invoke(this, EventArgs.Empty);
		}

		[Obsolete("Use IImageHandler.OnWindowChanged() instead.")]
		public event EventHandler? WindowChanged
		{
			add { }
			remove { }
		}

		public override UIImage? Image
		{
			get
			{
				return base.Image;
			}
			set
			{
				base.Image = value;
			}
		}

		public override CGSize SizeThatFits(CGSize size)
		{
			if (Image == null && Animation != null)
			{
				return new CGSize(Animation.Width, Animation.Height);
			}

			return base.SizeThatFits(size);
		}

		public MauiCAKeyFrameAnimation? Animation
		{
			get { return _animation?.GetTargetOrDefault(); }
			set
			{
				if (_animation is not null && _animation.TryGetTarget(out var animation))
				{
					Layer.RemoveAnimation(AnimationLayerName);
					animation.Dispose();
				}

				_animation = value is null ? null : new WeakReference<MauiCAKeyFrameAnimation>(value);

				if (_animation is not null && _animation.TryGetTarget(out var newAnimation))
				{
					Layer.AddAnimation(newAnimation, AnimationLayerName);
				}

				Layer.SetNeedsDisplay();
			}
		}

		public override bool IsAnimating
		{
			get
			{
				if (_animation is not null)
					return Layer.Speed != 0.0f;
				else
					return base.IsAnimating;
			}
		}

		public override void StartAnimating()
		{
			if (_animation is not null && _animation.TryGetTarget(out var animation) && Layer.Speed == 0.0f)
			{
				Layer.RemoveAnimation(AnimationLayerName);
				Layer.AddAnimation(animation, AnimationLayerName);
				Layer.Speed = 1.0f;
			}
			else
			{
				base.StartAnimating();
			}
		}

		public override void StopAnimating()
		{
			if (_animation is not null && _animation.TryGetTarget(out var animation) && Layer.Speed != 0.0f)
			{
				Layer.RemoveAnimation(AnimationLayerName);
				Layer.AddAnimation(animation, AnimationLayerName);
				Layer.Speed = 0.0f;
			}
			else
			{
				base.StopAnimating();
			}
		}

		protected override void Dispose(bool disposing)
		{
			if (_isDisposed)
				return;

			_isDisposed = true;

			if (disposing && _animation?.TryGetTarget(out var animation) != null)
			{
				Layer.RemoveAnimation(AnimationLayerName);
				animation?.Dispose();
				_animation = null;
			}

			base.Dispose(disposing);
		}
	}
}