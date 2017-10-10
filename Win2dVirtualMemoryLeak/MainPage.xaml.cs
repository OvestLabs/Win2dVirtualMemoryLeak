using System;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.UI.Xaml;

namespace Win2dVirtualMemoryLeak
{
	public sealed partial class MainPage
	{
		private readonly Random _random = new Random();
		private Image _image;
		private CanvasDevice _device;
		private CanvasVirtualImageSource _source;
		
		public MainPage()
		{
			InitializeComponent();
		}

		private byte CreateRandomByte()
		{
			return (byte)(_random.NextDouble() * 0xFF);
		}

		private Color CreateRandomColor()
		{
			var red = CreateRandomByte();
			var green = CreateRandomByte();
			var blue = CreateRandomByte();
			var color = Color.FromArgb(0x30, red, green, blue);

			return color;
		}

		private void RemoveExistingImage()
		{
			if (_image == null)
			{
				return;
			}

			Container.Children.Remove(_image);

			_source.RegionsInvalidated -= OnRegionsInvalidated;
			_image.Source = null;
			_device.Dispose();

			_image = null;
			_device = null;
			_source = null;
		}

		private void CreateNewImage()
		{
			const float dpi = 96f * 1; // 300% dpi scale;

			var device = new CanvasDevice();
			var source = new CanvasVirtualImageSource(device, 10000, 10000, dpi);
			var image = new Image
			{
				Width = 10000,
				Height = 10000,
				Stretch = Stretch.Fill,
				Source = source.Source,
			};

			source.RegionsInvalidated += OnRegionsInvalidated;

			Container.Children.Add(image);

			_image = image;
			_device = device;
			_source = source;
		}

		private void OnNextClicked(object sender, RoutedEventArgs e)
		{
			RemoveExistingImage();
			CreateNewImage();
		}

		private void OnRegionsInvalidated(CanvasVirtualImageSource sender, CanvasRegionsInvalidatedEventArgs e)
		{
			foreach (var region in e.InvalidatedRegions)
			{
				var randomColor = CreateRandomColor();

				using (sender.CreateDrawingSession(randomColor, region))
				{
					// Just changing colors...
				}
			}
		}
	}
}