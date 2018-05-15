namespace Seed.Api.Models
{
    /// <summary>
    /// Error data transfer object
    /// </summary>
    internal class ErrorDto
    {
        public ErrorDto(string error)
        {
            Error = error;
        }

        /// <summary>
        /// Error message
        /// </summary>
        public string Error { get; }
    }
}