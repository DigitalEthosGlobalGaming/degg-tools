namespace DeggTools
{
    using System;
    using System.IO;

    /// <summary>
    /// A PNG image. Call <see cref="Open(byte[],IChunkVisitor)"/> to open from file or bytes.
    /// </summary>
    public class Png
    {
        private readonly RawPngData data;
        private readonly bool hasTransparencyChunk;

        /// <summary>
        /// The header data from the PNG image.
        /// </summary>
        public ImageHeader Header { get; }

        /// <summary>
        /// The width of the image in pixels.
        /// </summary>
        public int Width => Header.Width;

        /// <summary>
        /// The height of the image in pixels.
        /// </summary>
        public int Height => Header.Height;

        /// <summary>
        /// Whether the image has an alpha (transparency) layer.
        /// </summary>
        public bool HasAlphaChannel => (Header.ColorType & ColorType.AlphaChannelUsed) != 0 || hasTransparencyChunk;

        internal Png(ImageHeader header, RawPngData data, bool hasTransparencyChunk)
        {
            Header = header;
            this.data = data ?? throw new ArgumentNullException(nameof(data));
            this.hasTransparencyChunk = hasTransparencyChunk;
        }

        /// <summary>
        /// Get the pixel at the given column and row (x, y).
        /// </summary>
        /// <remarks>
        /// Pixel values are generated on demand from the underlying data to prevent holding many items in memory at once, so consumers
        /// should cache values if they're going to be looped over many time.
        /// </remarks>
        /// <param name="x">The x coordinate (column).</param>
        /// <param name="y">The y coordinate (row).</param>
        /// <returns>The pixel at the coordinate.</returns>
        public Pixel GetPixel(int x, int y) => data.GetPixel(x, y);

        /// <summary>
        /// Read the PNG image from the stream.
        /// </summary>
        /// <param name="stream">The stream containing PNG data to be read.</param>
        /// <param name="chunkVisitor">Optional: A visitor which is called whenever a chunk is read by the library.</param>
        /// <returns>The <see cref="Png"/> data from the stream.</returns>
        public static Png Open(Stream stream, IChunkVisitor chunkVisitor = null)
            => PngOpener.Open(stream, chunkVisitor);

        /// <summary>
        /// Read the PNG image from the stream.
        /// </summary>
        /// <param name="stream">The stream containing PNG data to be read.</param>
        /// <param name="settings">Settings to apply when opening the PNG.</param>
        /// <returns>The <see cref="Png"/> data from the stream.</returns>
        public static Png Open(Stream stream, PngOpenerSettings settings)
            => PngOpener.Open(stream, settings);

        /// <summary>
        /// Read the PNG image from the bytes.
        /// </summary>
        /// <param name="bytes">The bytes of the PNG data to be read.</param>
        /// <param name="chunkVisitor">Optional: A visitor which is called whenever a chunk is read by the library.</param>
        /// <returns>The <see cref="Png"/> data from the bytes.</returns>
        public static Png Open(byte[] bytes, IChunkVisitor chunkVisitor = null)
        {
            using (var memoryStream = new MemoryStream(bytes))
            {
                return PngOpener.Open(memoryStream, chunkVisitor);
            }
        }

        /// <summary>
        /// Read the PNG image from the bytes.
        /// </summary>
        /// <param name="bytes">The bytes of the PNG data to be read.</param>
        /// <param name="settings">Settings to apply when opening the PNG.</param>
        /// <returns>The <see cref="Png"/> data from the bytes.</returns>
        public static Png Open(byte[] bytes, PngOpenerSettings settings)
        {
            using (var memoryStream = new MemoryStream(bytes))
            {
                return PngOpener.Open(memoryStream, settings);
            }
        }

        /// <summary>
        /// Read the PNG from the file path.
        /// </summary>
        /// <param name="filePath">The path to the PNG file to open.</param>
        /// <param name="chunkVisitor">Optional: A visitor which is called whenever a chunk is read by the library.</param>
        /// <remarks>This will open the file to obtain a <see cref="FileStream"/> so will lock the file during reading.</remarks>
        /// <returns>The <see cref="Png"/> data from the file.</returns>
        public static Png Open(string filePath, IChunkVisitor chunkVisitor = null)
        {
            using (var fileStream = File.OpenRead(filePath))
            {
                return Open(fileStream, chunkVisitor);
            }
        }

        /// <summary>
        /// Read the PNG from the file path.
        /// </summary>
        /// <param name="filePath">The path to the PNG file to open.</param>
        /// <param name="settings">Settings to apply when opening the PNG.</param>
        /// <remarks>This will open the file to obtain a <see cref="FileStream"/> so will lock the file during reading.</remarks>
        /// <returns>The <see cref="Png"/> data from the file.</returns>
        public static Png Open(string filePath, PngOpenerSettings settings)
        {
            using (var fileStream = File.OpenRead(filePath))
            {
                return Open(fileStream, settings);
            }
        }


		public Png CreateSquare(string path)
		{
			int xOffset = 0;
			int yOffset = 0;
			var max = Height;
			if (Width > Height)
			{
				max = Width;
			}

			if ( max < 2 )
			{
				max = 1;
			}

			if ( max > Height )
			{
				yOffset = (max - Height) / 2;
			}
			if ( max > Width ) {
				xOffset = (max - Width) / 2;
			}

			var builder = PngBuilder.Create( max, max, true);
			for ( int x = 0; x < this.Width; x++ )
			{
				for ( int y = 0; y < this.Height; y++ )
				{
					try
					{
						var xPixel = x + xOffset;
						var yPixel = y + yOffset;
						var pixel = this.GetPixel( x, y );
						builder.SetPixel( pixel.R, pixel.G, pixel.B, xPixel, yPixel );
					} catch(Exception e)
					{
					}		
				}
			}

			byte[] data;

			using ( var memory = new MemoryStream() )
			{
				builder.Save( memory );

				data = memory.ToArray();
			}

			System.IO.File.WriteAllBytes( path, data );


			return Png.Open( path );
			
		}

		public Pixel GetAveragePixel( int xStart, int yStart, int xEnd, int yEnd )
		{
			float totalRed = 0;
			float totalGreen = 0;
			float totalBlue = 0;
			var amount = 0;
			for ( int x = xStart; x < xEnd; x++ )
			{
				for ( int y = yStart; y < yEnd; y++ )
				{
					var myP = GetPixel( x, y );
					totalRed = totalRed + myP.R;
					totalGreen = totalGreen + myP.G;
					totalBlue = totalBlue + myP.B;
					amount = amount + 1;
				}
			}
			totalBlue = totalBlue / amount;
			totalRed = totalRed / amount;
			totalGreen = totalGreen / amount;

			return new Pixel( (byte)totalRed, (byte)totalGreen, (byte)totalBlue );
		}

		

		public Png ToPowerOf2( string path )
		{
			var square = this.CreateSquare( path + ".temp1" );
			File.Delete( path + ".temp1" );
			var width = square.Width;
			var height = square.Height;

			var max = height;
			if ( width > height )
			{
				max = width;
			}

			if ( max < 2 )
			{
				max = 1;
			}

			max = (int)Math.Pow( 2, (int)Math.Log( max - 1, 2 ) + 1 );

			float xDifference = (float)width / max;
			float yDifference = (float)height / max;

			var builder = PngBuilder.Create( max, max, true );
			for ( int x = 0; x < max; x++ )
			{
				for ( int y = 0; y < max; y++ )
				{
					try
					{
						int pixelX = (int)(x * xDifference);
						int pixelY = (int)(y * yDifference);
						var pixel = square.GetPixel( pixelX, pixelY );
						builder.SetPixel( pixel.R, pixel.G, pixel.B, x, y );
					}
					catch ( Exception e )
					{
					}
				}
			}

			byte[] data;

			using ( var memory = new MemoryStream() )
			{
				builder.Save( memory );

				data = memory.ToArray();
			}

			System.IO.File.WriteAllBytes( path, data );


			return Png.Open( path );

		}
	}
}
