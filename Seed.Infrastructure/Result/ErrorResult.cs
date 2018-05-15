namespace Seed.Infrastructure.Result
{
    /// <summary>
    /// Multi-purpose wrapper for generic error response with code+description structure
    /// </summary>
    public class ErrorResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorResult"/> class.
        /// </summary>
        /// <param name="code"> Error Code </param>
        /// <param name="description"> Error Description </param>
        public ErrorResult(string code, string description)
        {
            Code = code;
            Description = description;
        }

        /// <summary> Error Code </summary>
        public string Code { get; }

        /// <summary> Error Description </summary>
        public string Description { get; }
    }
}
