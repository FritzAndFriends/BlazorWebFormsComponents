using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorWebFormsComponents
{
	/// <summary>
	/// Blazor component that emulates the ASP.NET Web Forms FileUpload control.
	/// Uses Blazor's InputFile component internally for proper file data handling.
	/// </summary>
	public partial class FileUpload : BaseStyledComponent
	{
		private IBrowserFile _currentFile;
		private readonly List<IBrowserFile> _currentFiles = new List<IBrowserFile>();

		/// <summary>
		/// Gets a value indicating whether the FileUpload control contains a file.
		/// </summary>
		public bool HasFile => _currentFile != null;

		/// <summary>
		/// Gets a value indicating whether more than one file has been selected.
		/// </summary>
		public bool HasFiles => _currentFiles.Count > 1;

		/// <summary>
		/// Gets the name of the file to upload using the FileUpload control.
		/// Returns the first file name if multiple files are selected.
		/// </summary>
		public string FileName => _currentFile?.Name ?? string.Empty;

		/// <summary>
		/// Gets the contents of the uploaded file as a byte array.
		/// For multiple files, returns the first file's content.
		/// </summary>
		public async Task<byte[]> GetFileBytesAsync()
		{
			if (!HasFile) return Array.Empty<byte>();

			using var stream = _currentFile.OpenReadStream(MaxFileSize);
			using var memoryStream = new MemoryStream();
			await stream.CopyToAsync(memoryStream);
			return memoryStream.ToArray();
		}

		/// <summary>
		/// Gets the contents of the uploaded file as a byte array (synchronous).
		/// For multiple files, returns the first file's content.
		/// Note: Prefer GetFileBytesAsync() for better async/await patterns.
		/// </summary>
		public byte[] FileBytes
		{
			get
			{
				if (!HasFile) return Array.Empty<byte>();

				using var stream = _currentFile.OpenReadStream(MaxFileSize);
				using var memoryStream = new MemoryStream();
				stream.CopyTo(memoryStream);
				return memoryStream.ToArray();
			}
		}

		/// <summary>
		/// Gets a Stream object that points to the uploaded file.
		/// For multiple files, returns the first file's stream.
		/// </summary>
		public Stream FileContent
		{
			get
			{
				if (!HasFile) return Stream.Null;

				return _currentFile.OpenReadStream(MaxFileSize);
			}
		}

		/// <summary>
		/// Gets the posted file object for compatibility with Web Forms.
		/// For multiple files, returns the first file.
		/// </summary>
		public PostedFileWrapper PostedFile
		{
			get
			{
				if (!HasFile) return null;

				return new PostedFileWrapper(_currentFile, MaxFileSize);
			}
		}

		/// <summary>
		/// Gets or sets whether the control allows selection of multiple files.
		/// Default is false for Web Forms compatibility.
		/// </summary>
		[Parameter]
		public bool AllowMultiple { get; set; } = false;

		/// <summary>
		/// Gets or sets the accept attribute for the file input.
		/// Specifies the types of files that the server accepts.
		/// Example: ".jpg,.png,.pdf" or "image/*"
		/// </summary>
		[Parameter]
		public string Accept { get; set; }

		/// <summary>
		/// Gets or sets the maximum file size in bytes. Default is 512000 bytes (~500 KiB).
		/// Can be increased for larger files, but be mindful of memory usage.
		/// </summary>
		[Parameter]
		public long MaxFileSize { get; set; } = 512000; // ~500 KiB default

		/// <summary>
		/// Gets or sets the tooltip text displayed when hovering over the control.
		/// </summary>
		[Parameter]
		public string ToolTip { get; set; }

		/// <summary>
		/// Event raised when a file is selected.
		/// </summary>
		[Parameter]
		public EventCallback<InputFileChangeEventArgs> OnFileSelected { get; set; }

		/// <summary>
		/// Saves the contents of the uploaded file to a specified path on the server.
		/// For multiple files, saves only the first file.
		/// </summary>
		/// <param name="filename">The full path of the file to save.</param>
		public async Task SaveAs(string filename)
		{
			if (!HasFile)
			{
				throw new InvalidOperationException("No file has been selected for upload.");
			}

			// Sanitize: ensure the filename cannot escape the intended directory
			var safeFileName = Path.GetFileName(filename);
			var directory = Path.GetDirectoryName(filename);
			var safePath = string.IsNullOrEmpty(directory) ? safeFileName : Path.Combine(directory, safeFileName);

			using var stream = _currentFile.OpenReadStream(MaxFileSize);
			using var fileStream = new FileStream(safePath, FileMode.Create);
			await stream.CopyToAsync(fileStream);
		}

		/// <summary>
		/// Gets all selected files when AllowMultiple is true.
		/// </summary>
		/// <returns>An enumerable collection of browser files.</returns>
		public IEnumerable<IBrowserFile> GetMultipleFiles()
		{
			return _currentFiles.AsReadOnly();
		}

		/// <summary>
		/// Saves all uploaded files to the specified directory.
		/// File names are sanitized to prevent directory traversal attacks.
		/// </summary>
		/// <param name="directory">The directory path where files should be saved.</param>
		/// <returns>A list of saved file paths.</returns>
		public async Task<List<string>> SaveAllFiles(string directory)
		{
			if (!HasFile)
			{
				throw new InvalidOperationException("No files have been selected for upload.");
			}

			if (!Directory.Exists(directory))
			{
				throw new DirectoryNotFoundException($"The directory '{directory}' does not exist.");
			}

			var savedFiles = new List<string>();
			var files = _currentFiles;

			foreach (var file in files)
			{
				// Sanitize filename to prevent directory traversal attacks
				var safeFileName = Path.GetFileName(file.Name);
				var fullPath = Path.GetFullPath(Path.Combine(directory, safeFileName));

				// Verify the resolved path is still within the target directory
				var resolvedDirectory = Path.GetFullPath(directory);
				if (!fullPath.StartsWith(resolvedDirectory, StringComparison.OrdinalIgnoreCase))
				{
					throw new InvalidOperationException($"File name '{file.Name}' would escape the target directory.");
				}

				using var stream = file.OpenReadStream(MaxFileSize);
				using var fileStream = new FileStream(fullPath, FileMode.Create);
				await stream.CopyToAsync(fileStream);
				savedFiles.Add(fullPath);
			}

			return savedFiles;
		}

		private async Task OnFileChangeInternal(InputFileChangeEventArgs e)
		{
			_currentFiles.Clear();

			if (AllowMultiple)
			{
				_currentFiles.AddRange(e.GetMultipleFiles());
				_currentFile = _currentFiles.FirstOrDefault();
			}
			else
			{
				_currentFile = e.File;
				_currentFiles.Add(_currentFile);
			}

			if (OnFileSelected.HasDelegate)
			{
				await OnFileSelected.InvokeAsync(e);
			}
		}

		/// <summary>
		/// Wrapper class to provide compatibility with Web Forms HttpPostedFile.
		/// </summary>
		public class PostedFileWrapper
		{
			private readonly IBrowserFile _file;
			private readonly long _maxFileSize;

			internal PostedFileWrapper(IBrowserFile file, long maxFileSize)
			{
				_file = file;
				_maxFileSize = maxFileSize;
			}

			/// <summary>
			/// Gets the size of the uploaded file in bytes.
			/// </summary>
			public long ContentLength => _file.Size;

			/// <summary>
			/// Gets the MIME content type of the uploaded file.
			/// </summary>
			public string ContentType => _file.ContentType;

			/// <summary>
			/// Gets the fully qualified name of the file on the client.
			/// </summary>
			public string FileName => _file.Name;

			/// <summary>
			/// Gets a Stream object that points to the uploaded file.
			/// Note: The caller is responsible for disposing the returned stream.
			/// </summary>
			public Stream InputStream => _file.OpenReadStream(_maxFileSize);

			/// <summary>
			/// Saves the uploaded file to the specified path.
			/// </summary>
			/// <param name="filename">The full path to save the file.</param>
			public async Task SaveAs(string filename)
			{
				using var stream = _file.OpenReadStream(_maxFileSize);
				using var fileStream = new FileStream(filename, FileMode.Create);
				await stream.CopyToAsync(fileStream);
			}
		}
	}
}
