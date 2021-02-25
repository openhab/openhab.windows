using System;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using OpenHAB.Core.Common;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using ColorChangedEventArgs = OpenHAB.Core.Common.ColorChangedEventArgs;
using ColorHelper = OpenHAB.Core.Common.ColorHelper;

namespace OpenHAB.Windows.Controls
{
    /// <summary>
    /// Colorpicker control.
    /// </summary>
    public sealed partial class ColorMap : UserControl
    {
        private readonly GradientStop _lightnessStart;
        private readonly GradientStop _lightnessMid;
        private readonly GradientStop _lightnessEnd;

        private PointerPoint _lastPoint;
        private WriteableBitmap _bitmap;
        private double _colorX;
        private double _colorY;
        private bool _settingColor;
        private bool _isloaded;
        private double _angle;
        private double _radialpos;

        /// <summary>
        /// Event that fires whenever a new color is selected
        /// </summary>
        public event EventHandler<ColorChangedEventArgs> ColorChanged;

        /// <summary>
        /// Initializes a new instance of the <see cref="ColorMap"/> class.
        /// </summary>
        public ColorMap()
        {
            InitializeComponent();

            Loaded += MeshCanvas_Loaded;

            ellipse.PointerMoved += Image3_PointerMoved;
            ellipse.PointerPressed += Image3_PointerPressed;
            ellipse.PointerReleased += Image3_PointerReleased;

            var lightnessGradient = new LinearGradientBrush
            {
                StartPoint = new Point(0, 0),
                EndPoint = new Point(0, 1),
            };

            _lightnessStart = new GradientStop();
            _lightnessMid = new GradientStop { Offset = 0.5 };
            _lightnessEnd = new GradientStop { Offset = 1 };
            lightnessGradient.GradientStops = new GradientStopCollection()
            {
                _lightnessStart, _lightnessMid, _lightnessEnd,
            };
        }

        /// <summary>
        /// Gets or sets the Color property.
        /// </summary>
        public Color Color
        {
            get { return (Color)GetValue(ColorProperty); }
            set { SetValue(ColorProperty, value); }
        }
        public double Angle {
            get { return _angle; }
            set { _angle = value; }
        }
        public double RadialPos {
            get { return _radialpos; }
            set { _radialpos = value; }
        }

        /// <summary>
        /// Bindable property for the Color property.
        /// </summary>
        public static readonly DependencyProperty ColorProperty =
            DependencyProperty.Register("Color", typeof(Color), typeof(ColorMap), new PropertyMetadata(default(Color), OnColorChanged));

        private static void OnColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var map = d as ColorMap;
            map?.ColorChanged?.Invoke(map, new ColorChangedEventArgs((Color)e.NewValue));

            if (map == null || map._settingColor)
            {
                return;
            }

            var col = (Color)e.NewValue;
            var hsl = ColorHelper.ToHSL(col);

            map._lightnessMid.Color = ColorHelper.FromHSL(new Vector4(hsl.X, 1, 0.5f, 1));

            double angle = Math.PI * 2 * hsl.X;
            Vector2 normalized = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));
            Vector2 halfSize = new Vector2(
                (float)map.ellipse.ActualWidth / 2,
                (float)map.ellipse.ActualHeight / 2);

            Vector2 pos = ((hsl.Y / 2) * normalized * halfSize * new Vector2(1, -1)) + halfSize;

            map._colorX = pos.X;
            map._colorY = pos.Y;
            map.UpdateThumb();
        }

        private void Image3_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            ellipse.CapturePointer(e.Pointer);
            _lastPoint = e.GetCurrentPoint(ellipse);
            _colorX = _lastPoint.Position.X;
            _colorY = _lastPoint.Position.Y;
            UpdateColor();
            UpdateAnglePos();
            UpdateThumb();
            e.Handled = true;
        }

        private void Image3_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            ellipse.ReleasePointerCapture(e.Pointer);
            _lastPoint = null;
            e.Handled = true;
        }

        private void Image3_PointerMoved(object sender, PointerRoutedEventArgs e)
        {
            if (ellipse.PointerCaptures?.Any(p => p.PointerId == e.Pointer.PointerId) != true)
            {
                return;
            }

            _lastPoint = e.GetCurrentPoint(ellipse);
            _colorX = _lastPoint.Position.X;
            _colorY = _lastPoint.Position.Y;

            var bounds = new Rect(0, 0, ellipse.ActualWidth, ellipse.ActualHeight);
            if (!bounds.Contains(_lastPoint.Position) || !UpdateColor())
            {
                return;
            }

            UpdateThumb();
            e.Handled = true;
        }

        private void UpdateThumb()
        {
            Canvas.SetLeft(thumb, _colorX - (thumb.ActualWidth / 2));
            Canvas.SetTop(thumb, _colorY - (thumb.ActualHeight / 2));
            thumb.Visibility = Visibility.Visible;
        }

        private void UpdateAnglePos()
        {
            var sizeX = ellipse.ActualWidth;
            var sizeY = ellipse.ActualHeight;
            var x = _colorX / sizeX;
            var y = 1 - (_colorY / sizeY);
            var _intangle = Math.Abs( (Math.Atan((y - 0.5) / (x - 0.5)) * 180) / Math.PI); // Absolute Value

            //Check Quadrant
            if (x >= 0.5 && y >= 0.5)
            {
                _angle = 270 - _intangle;
            }
            else if (x < 0.5 && y >= 0.5)
            {
                _angle = 180 + _intangle;
            }
            else if (x < 0.5 && y < 0.5)
            {
                _angle = 180 - _intangle;
            }
            else
            {
                _angle = _intangle;
            }

            _radialpos = Math.Sqrt(Math.Pow(x - 0.5, 2) + Math.Pow(y - 0.5, 2));
            _radialpos = _radialpos * 2.2; // 2times because we are working with 0.5 as max. .2 to give a bit more value and make it easier to catch max value;
            if (_radialpos > 1)
            {
                _radialpos = 1;
            }

            _radialpos = _radialpos * 100;
        }

        private bool UpdateColor()
        {
            if (!_isloaded)
            {
                return false;
            }

            var x = _colorX / ellipse.ActualWidth;
            var y = 1 - (_colorY / ellipse.ActualHeight);
            var selectedColor = CalcWheelColor((float)x, 1 - (float)y, 100);

            if (selectedColor.A <= 0)
            {
                return false;
            }

            SetColor(selectedColor);
            _lightnessStart.Color = Colors.White;
            _lightnessMid.Color = CalcWheelColor((float)x, 1 - (float)y, 0.5f);
            _lightnessEnd.Color = Colors.Black;
            return true;
        }

        private void SetColor(Color color)
        {
            _settingColor = true;
            Color = color;
            _settingColor = false;
        }

        private async void MeshCanvas_Loaded(object sender, RoutedEventArgs e)
        {
            _bitmap = new WriteableBitmap(1000, 1000);
            await CreateHueCircle(0.5f);
            image3.ImageSource = _bitmap;
            _isloaded = true;
        }

        private Task CreateHueCircle(float lightness)
        {
            return FillBitmap(_bitmap, (x, y) => CalcWheelColor(x, y, lightness));
        }

        private Color CalcWheelColor(float x, float y, float lightness)
        {
            x = x - 0.5f;
            y = (1 - y) - 0.5f;
            float saturation = 2 * (float)Math.Sqrt((x * x) + (y * y));
            float hue = y < 0 ?
                (float)Math.Atan2(-y, -x) + (float)Math.PI :
                (float)Math.Atan2(y, x);

            if (saturation > 1)
            {
                saturation = 1;
            }

            return ColorHelper.FromHSL(new Vector4(hue / ((float)Math.PI * 2), saturation, lightness, 1));
        }

        private async Task FillBitmap(WriteableBitmap bmp, Func<float, float, Color> fillPixel)
        {
            var stream = bmp.PixelBuffer.AsStream();
            int width = bmp.PixelWidth;
            int height = bmp.PixelHeight;
            await Task.Run(() =>
            {
                for (int y = 0; y < width; y++)
                {
                    for (int x = 0; x < height; x++)
                    {
                        var color = fillPixel((float)x / width, (float)y / height);
                        WriteBgra(stream, color);
                    }
                }
            });
            stream.Dispose();
            bmp.Invalidate();
        }

        private void WriteBgra(Stream stream, Color color)
        {
            stream.WriteByte(color.B);
            stream.WriteByte(color.G);
            stream.WriteByte(color.R);
            stream.WriteByte(color.A);
        }
    }
}
